﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Efrpg.Filtering;

namespace Efrpg
{
    public class Table : EntityName
    {
        public Schema Schema;
        public string Type;
        public string Suffix;
        public List<string> ExtendedProperty;
        public bool IsMapping;
        public bool IsView;
        public bool HasForeignKey;
        public bool HasNullableColumns;
        public bool HasPrimaryKey;
        public string AdditionalComment;
        public string PluralNameOverride;
        public string DbSetModifier = "public";
        public string BaseClasses;

        public List<Column> Columns;
        public List<PropertyAndComments> ReverseNavigationProperty;
        public List<string> MappingConfiguration;
        public List<string> ReverseNavigationCtor;
        public List<string> ReverseNavigationUniquePropName;
        public List<string> ReverseNavigationUniquePropNameClashes;
        public List<RawIndex> Indexes;
        public List<string> Attributes = new List<string>(); // List of attributes to add to this table

        private readonly IDbContextFilter _filter;

        public Table(IDbContextFilter filter, Schema schema, string dbName, bool isView)
        {
            _filter = filter;
            Schema  = schema;
            DbName  = dbName;
            IsView  = isView;
            Columns = new List<Column>();

            ResetNavigationProperties();
            ReverseNavigationUniquePropNameClashes = new List<string>();
            ExtendedProperty = new List<string>();
        }

        internal static string GetLazyLoadingMarker()
        {
            return Settings.UseLazyLoading ? "virtual " : string.Empty;
        }

        public string NameHumanCaseWithSuffix()
        {
            return NameHumanCase + Suffix;
        }

        public void ResetNavigationProperties()
        {
            MappingConfiguration = new List<string>();
            ReverseNavigationProperty = new List<PropertyAndComments>();
            ReverseNavigationCtor = new List<string>();
            ReverseNavigationUniquePropName = new List<string>();
            foreach (var col in Columns)
                col.ResetNavigationProperties();
        }

        public void SetPrimaryKeys()
        {
            HasPrimaryKey = Columns.Any(x => x.IsPrimaryKey);
            if (HasPrimaryKey)
                return; // Table has at least one primary key

            if (IsView && Settings.IsEfCore3())
                return; // EfCore 3 supports views by use of .HasNoKey() and .ToView("view name");

            // This table is not allowed in EntityFramework v6 / EfCore 2 as it does not have a primary key.
            // Therefore generate a composite key from all non-null fields.
            foreach (var col in Columns.Where(x => !x.IsNullable && !x.Hidden))
            {
                col.IsPrimaryKey = true;
                HasPrimaryKey = true;
            }
        }

        public IEnumerable<Column> PrimaryKeys
        {
            get
            {
                return Columns
                    .Where(x => x.IsPrimaryKey)
                    .OrderBy(x => x.PrimaryKeyOrdinal)
                    .ThenBy(x => x.Ordinal)
                    .ToList();
            }
        }

        public string PrimaryKeyNameHumanCase()
        {
            var data = PrimaryKeys.Select(x => "x." + x.NameHumanCase).ToList();
            var n = data.Count;
            if (n == 0)
                return string.Empty;
            if (n == 1)
                return "x => " + data.First();
            // More than one primary key
            return string.Format("x => new {{ {0} }}", string.Join(", ", data));
        }

        public Column this[string columnName]
        {
            get { return GetColumn(columnName); }
        }

        public Column GetColumn(string columnName)
        {
            return Columns.SingleOrDefault(x =>
                string.Compare(x.DbName, columnName, StringComparison.OrdinalIgnoreCase) == 0);
        }

        public string GetUniqueColumnName(bool isParent, string tableNameHumanCase, ForeignKey foreignKey, bool checkForFkNameClashes,
            bool makeSingular, Relationship relationship)
        {
            // User specified name
            if (isParent && !string.IsNullOrEmpty(foreignKey.ParentName))
                return foreignKey.ParentName;

            // User specified name
            if (!isParent && !string.IsNullOrEmpty(foreignKey.ChildName))
                return foreignKey.ChildName;

            // Generate name
            var addReverseNavigationUniquePropName = checkForFkNameClashes && (DbName == foreignKey.FkTableName || (DbName == foreignKey.PkTableName && foreignKey.IncludeReverseNavigation));
            if (ReverseNavigationUniquePropName.Count == 0)
            {
                ReverseNavigationUniquePropName.Add(NameHumanCase);
                ReverseNavigationUniquePropName.AddRange(Columns.Select(c => c.NameHumanCase));
            }

            if (!makeSingular)
                tableNameHumanCase = Inflector.MakePlural(tableNameHumanCase);

            if (checkForFkNameClashes && ReverseNavigationUniquePropName.Contains(tableNameHumanCase) &&
                !ReverseNavigationUniquePropNameClashes.Contains(tableNameHumanCase))
                ReverseNavigationUniquePropNameClashes.Add(tableNameHumanCase); // Name clash

            // Attempt 1
            var fkName = (Settings.UsePascalCase ? Inflector.ToTitleCase(foreignKey.FkColumn) : foreignKey.FkColumn).Replace(" ", string.Empty).Replace("$", string.Empty);
            var name = Settings.ForeignKeyName(tableNameHumanCase, foreignKey, fkName, relationship, 1);
            string col;
            if (!ReverseNavigationUniquePropNameClashes.Contains(name) && !ReverseNavigationUniquePropName.Contains(name))
            {
                if (addReverseNavigationUniquePropName || !checkForFkNameClashes)
                {
                    ReverseNavigationUniquePropName.Add(name);
                }

                return name;
            }

            if (DbName == foreignKey.FkTableName)
            {
                // Attempt 2
                if (fkName.ToLowerInvariant().EndsWith("id"))
                {
                    col = Settings.ForeignKeyName(tableNameHumanCase, foreignKey, fkName, relationship, 2);
                    if (checkForFkNameClashes && ReverseNavigationUniquePropName.Contains(col) &&
                        !ReverseNavigationUniquePropNameClashes.Contains(col))
                        ReverseNavigationUniquePropNameClashes.Add(col); // Name clash

                    if (!ReverseNavigationUniquePropNameClashes.Contains(col) &&
                        !ReverseNavigationUniquePropName.Contains(col))
                    {
                        if (addReverseNavigationUniquePropName || !checkForFkNameClashes)
                        {
                            ReverseNavigationUniquePropName.Add(col);
                        }

                        return col;
                    }
                }

                // Attempt 3
                col = Settings.ForeignKeyName(tableNameHumanCase, foreignKey, fkName, relationship, 3);
                if (checkForFkNameClashes && ReverseNavigationUniquePropName.Contains(col) &&
                    !ReverseNavigationUniquePropNameClashes.Contains(col))
                    ReverseNavigationUniquePropNameClashes.Add(col); // Name clash

                if (!ReverseNavigationUniquePropNameClashes.Contains(col) &&
                    !ReverseNavigationUniquePropName.Contains(col))
                {
                    if (addReverseNavigationUniquePropName || !checkForFkNameClashes)
                    {
                        ReverseNavigationUniquePropName.Add(col);
                    }

                    return col;
                }
            }

            // Attempt 4
            col = Settings.ForeignKeyName(tableNameHumanCase, foreignKey, fkName, relationship, 4);
            if (checkForFkNameClashes && ReverseNavigationUniquePropName.Contains(col) &&
                !ReverseNavigationUniquePropNameClashes.Contains(col))
                ReverseNavigationUniquePropNameClashes.Add(col); // Name clash

            if (!ReverseNavigationUniquePropNameClashes.Contains(col) && !ReverseNavigationUniquePropName.Contains(col))
            {
                if (addReverseNavigationUniquePropName || !checkForFkNameClashes)
                {
                    ReverseNavigationUniquePropName.Add(col);
                }

                return col;
            }

            // Attempt 5
            for (int n = 1; n < 99; ++n)
            {
                col = Settings.ForeignKeyName(tableNameHumanCase, foreignKey, fkName, relationship, 5) + n;

                if (ReverseNavigationUniquePropName.Contains(col))
                    continue;

                if (addReverseNavigationUniquePropName || !checkForFkNameClashes)
                {
                    ReverseNavigationUniquePropName.Add(col);
                }

                return col;
            }

            // Give up
            return Settings.ForeignKeyName(tableNameHumanCase, foreignKey, fkName, relationship, 6);
        }

        public void AddReverseNavigation(Relationship relationship, Table fkTable, string propName,
            string constraint, List<ForeignKey> fks, Table mappingTable = null)
        {
            var fkNames = "";
            switch (relationship)
            {
                case Relationship.OneToOne:
                case Relationship.OneToMany:
                case Relationship.ManyToOne:
                    fkNames = (fks.Count > 1 ? "(" : "") + string.Join(", ", fks.Select(x => "[" + x.FkColumn + "]").Distinct().ToArray()) + (fks.Count > 1 ? ")" : "");
                    break;
                case Relationship.ManyToMany:
                    break;
            }
            var accessModifier = fks != null && fks.FirstOrDefault() != null ? (fks.FirstOrDefault().AccessModifier ?? "public") : "public";
            switch (relationship)
            {
                case Relationship.OneToOne:
                    ReverseNavigationProperty.Add(
                        new PropertyAndComments()
                        {
                            AdditionalDataAnnotations = _filter.ForeignKeyAnnotationsProcessing(fkTable, this, propName, string.Empty),
                            Definition = string.Format("{0} {1}{2} {3} {{ get; set; }}{4}", accessModifier, GetLazyLoadingMarker(), fkTable.NameHumanCaseWithSuffix(), propName, Settings.IncludeComments != CommentsStyle.None ? " // " + constraint : string.Empty),
                            Comments = string.Format("Parent (One-to-One) {0} pointed by [{1}].{2} ({3})", NameHumanCaseWithSuffix(), fkTable.DbName, fkNames, fks.First().ConstraintName)
                        }
                    );
                    break;

                case Relationship.OneToMany:
                    ReverseNavigationProperty.Add(
                        new PropertyAndComments()
                        {
                            AdditionalDataAnnotations = _filter.ForeignKeyAnnotationsProcessing(fkTable, this, propName, string.Empty),
                            Definition = string.Format("{0} {1}{2} {3} {{ get; set; }}{4}", accessModifier, GetLazyLoadingMarker(), fkTable.NameHumanCaseWithSuffix(), propName, Settings.IncludeComments != CommentsStyle.None ? " // " + constraint : string.Empty),
                            Comments = string.Format("Parent {0} pointed by [{1}].{2} ({3})", NameHumanCaseWithSuffix(), fkTable.DbName, fkNames, fks.First().ConstraintName)
                        }
                    );
                    break;

                case Relationship.ManyToOne:
                    var initialisation1 = string.Empty;
                    if (Settings.UsePropertyInitialisers)
                        initialisation1 = string.Format(" = new {0}<{1}>();", Settings.CollectionType, fkTable.NameHumanCaseWithSuffix());
                    ReverseNavigationProperty.Add(
                        new PropertyAndComments()
                        {
                            AdditionalDataAnnotations = _filter.ForeignKeyAnnotationsProcessing(fkTable, this, propName, string.Empty),
                            Definition = string.Format("{0} {1}{2}<{3}> {4} {{ get; set; }}{5}{6}", accessModifier, GetLazyLoadingMarker(), Settings.CollectionInterfaceType, fkTable.NameHumanCaseWithSuffix(), propName, initialisation1, Settings.IncludeComments != CommentsStyle.None ? " // " + constraint : string.Empty),
                            Comments = string.Format("Child {0} where [{1}].{2} point to this entity ({3})", Inflector.MakePlural(fkTable.NameHumanCase), fkTable.DbName, fkNames, fks.First().ConstraintName)
                        }
                    );
                    ReverseNavigationCtor.Add(string.Format("{0} = new {1}<{2}>();", propName, Settings.CollectionType, fkTable.NameHumanCaseWithSuffix()));
                    break;

                case Relationship.ManyToMany:
                    var initialisation2 = string.Empty;
                    if (Settings.UsePropertyInitialisers)
                        initialisation2 = string.Format(" = new {0}<{1}>();", Settings.CollectionType, fkTable.NameHumanCaseWithSuffix());
                    ReverseNavigationProperty.Add(
                        new PropertyAndComments()
                        {
                            AdditionalDataAnnotations = _filter.ForeignKeyAnnotationsProcessing(fkTable, this, propName, string.Empty),
                            Definition = string.Format("{0} {1}{2}<{3}> {4} {{ get; set; }}{5}{6}", accessModifier, GetLazyLoadingMarker(), Settings.CollectionInterfaceType, fkTable.NameHumanCaseWithSuffix(), propName, initialisation2, Settings.IncludeComments != CommentsStyle.None ? " // Many to many mapping" : string.Empty),
                            Comments = string.Format("Child {0} (Many-to-Many) mapped by table [{1}]", Inflector.MakePlural(fkTable.NameHumanCase), mappingTable == null ? string.Empty : mappingTable.DbName)
                        }
                    );

                    ReverseNavigationCtor.Add(string.Format("{0} = new {1}<{2}>();", propName, Settings.CollectionType, fkTable.NameHumanCaseWithSuffix()));
                    break;

                default:
                    throw new ArgumentOutOfRangeException("relationship");
            }
        }

        public void IdentifyMappingTable(List<ForeignKey> fkList, Tables tables, bool checkForFkNameClashes, bool includeSchema)
        {
            IsMapping = false;

            var nonReadOnlyColumns = Columns
                .Where(c => !c.IsIdentity && !c.IsRowVersion && !c.IsStoreGenerated && !c.Hidden)
                .ToList();

            // Ignoring read-only columns, it must have only 2 columns to be a mapping table
            if (nonReadOnlyColumns.Count != 2)
                return;

            // Must have 2 primary keys
            if (nonReadOnlyColumns.Count(x => x.IsPrimaryKey) != 2)
                return;

            // No columns should be nullable
            if (nonReadOnlyColumns.Any(x => x.IsNullable))
                return;

            // Find the foreign keys for this table
            var foreignKeys = fkList.Where(x =>
                    string.Compare(x.FkTableName, DbName, StringComparison.OrdinalIgnoreCase) == 0 &&
                    string.Compare(x.FkSchema, Schema.DbName, StringComparison.OrdinalIgnoreCase) == 0)
                .ToList();

            // Each column must have a foreign key, therefore check column and foreign key counts match
            if (foreignKeys.Select(x => x.FkColumn).Distinct().Count() != 2)
                return;

            var left = foreignKeys[0];
            var right = foreignKeys[1];
            if (!left.IncludeReverseNavigation || !right.IncludeReverseNavigation)
                return;

            var leftTable = tables.GetTable(left.PkTableName, left.PkSchema);
            if (leftTable == null)
                return;

            var rightTable = tables.GetTable(right.PkTableName, right.PkSchema);
            if (rightTable == null)
                return;

            var leftPropName = leftTable.GetUniqueColumnName(true, rightTable.NameHumanCase, right, checkForFkNameClashes, false, Relationship.ManyToOne); // relationship from the mapping table to each side is Many-to-One
            leftPropName = _filter.MappingTableRename(DbName, leftTable.NameHumanCase, leftPropName);
            var rightPropName = rightTable.GetUniqueColumnName(false, leftTable.NameHumanCase, left, checkForFkNameClashes, false, Relationship.ManyToOne); // relationship from the mapping table to each side is Many-to-One
            rightPropName = _filter.MappingTableRename(DbName, rightTable.NameHumanCase, rightPropName);

            leftTable.AddMappingConfiguration(left, right, leftPropName, rightPropName, includeSchema);

            IsMapping = true;
            rightTable.AddReverseNavigation(Relationship.ManyToMany, leftTable,  rightPropName, null, null, this);
            leftTable.AddReverseNavigation (Relationship.ManyToMany, rightTable, leftPropName,  null, null, this);
        }

        private void AddMappingConfiguration(ForeignKey left, ForeignKey right, string leftPropName, string rightPropName, bool includeSchema)
        {
            MappingConfiguration.Add(string.Format(@"HasMany(t => t.{0}).WithMany(t => t.{1}).Map(m =>
        {{
            m.ToTable(""{2}""{5});
            m.MapLeftKey(""{3}"");
            m.MapRightKey(""{4}"");
        }});", leftPropName, rightPropName, left.FkTableName, left.FkColumn, right.FkColumn,
                !includeSchema ? string.Empty : ", \"" + left.FkSchema + "\""));
        }

        // This method will be called right before we write the POCO class
        public string WriteClassAttributes()
        {
            var sb = new StringBuilder();

            // Example:
            //if (ClassName.StartsWith("Order"))
            //    sb.AppendLine("[YourAttribute]");

            if (Attributes != null && Attributes.Any())
            {
                foreach (var attribute in Attributes)
                {
                    sb.AppendLine(attribute);
                }
            }

            return sb.ToString();
        }

        // This method will be called right before we write the POCO class
        public string WriteComments()
        {
            if(Settings.IncludeComments == CommentsStyle.None)
                return string.Empty;

            var comment = "// " + DbName + Environment.NewLine;
            if (string.IsNullOrWhiteSpace(AdditionalComment))
                return comment;

            return comment + "// " + AdditionalComment + Environment.NewLine;
        }
        
        // This method will be called right before we write the POCO class
        public string WriteExtendedComments()
        {
            if (Settings.IncludeExtendedPropertyComments == CommentsStyle.None || !ExtendedProperty.Any())
                return string.Empty;

            var lines = ExtendedProperty
                .SelectMany(x => x.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                .ToList();

            var sb = new StringBuilder(255);
            sb.AppendLine("/// <summary>");
            foreach (var line in lines.Select(x => x.Replace("///", string.Empty).Trim()))
            {
                sb.Append("/// ");
                sb.AppendLine(System.Security.SecurityElement.Escape(line));
            }
            sb.AppendLine("/// </summary>");
            return sb.ToString();
        }
        
        // Writes any boilerplate stuff inside the POCO class body
        public string WriteInsideClassBody()
        {
            // Example:
            //return "    // " + ClassName + Environment.NewLine;

            // Do nothing by default
            return string.Empty;
        }
    }
}