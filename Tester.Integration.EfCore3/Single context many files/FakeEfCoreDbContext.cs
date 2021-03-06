// <auto-generated>

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tester.Integration.EfCore3.Single_context_many_files
{
    public class FakeEfCoreDbContext : IEfCoreDbContext
    {
        public DbSet<ColumnName> ColumnNames { get; set; } // ColumnNames
        public DbSet<Stafford_Boo> Stafford_Boos { get; set; } // Boo
        public DbSet<Stafford_ComputedColumn> Stafford_ComputedColumns { get; set; } // ComputedColumns
        public DbSet<Stafford_Foo> Stafford_Foos { get; set; } // Foo
        public DbSet<Synonyms_Child> Synonyms_Children { get; set; } // Child
        public DbSet<Synonyms_Parent> Synonyms_Parents { get; set; } // Parent
        public DbSet<UserInfo> UserInfoes { get; set; } // UserInfo
        public DbSet<UserInfoAttribute> UserInfoAttributes { get; set; } // UserInfoAttributes

        public FakeEfCoreDbContext()
        {
            _database = null;

            ColumnNames = new FakeDbSet<ColumnName>("C36");
            Stafford_Boos = new FakeDbSet<Stafford_Boo>("Id");
            Stafford_ComputedColumns = new FakeDbSet<Stafford_ComputedColumn>("Id");
            Stafford_Foos = new FakeDbSet<Stafford_Foo>("Id");
            Synonyms_Children = new FakeDbSet<Synonyms_Child>("ChildId");
            Synonyms_Parents = new FakeDbSet<Synonyms_Parent>("ParentId");
            UserInfoes = new FakeDbSet<UserInfo>("Id");
            UserInfoAttributes = new FakeDbSet<UserInfoAttribute>("Id");

        }

        public int SaveChangesCount { get; private set; }
        public virtual int SaveChanges()
        {
            ++SaveChangesCount;
            return 1;
        }

        public virtual int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            return SaveChanges();
        }

        public virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            ++SaveChangesCount;
            return Task<int>.Factory.StartNew(() => 1, cancellationToken);
        }
        public virtual Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken)
        {
            ++SaveChangesCount;
            return Task<int>.Factory.StartNew(x => 1, acceptAllChangesOnSuccess, cancellationToken);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private DatabaseFacade _database;
        public DatabaseFacade Database { get { return _database; } }

        public DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }

        public virtual EntityEntry Add(object entity)
        {
            throw new NotImplementedException();
        }

        public virtual EntityEntry<TEntity> Add<TEntity>(TEntity entity) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public virtual Task AddRangeAsync(params object[] entities)
        {
            throw new NotImplementedException();
        }

        public virtual async Task AddRangeAsync(IEnumerable<object> entities, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }

        public virtual async ValueTask<EntityEntry<TEntity>> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }

        public virtual async ValueTask<EntityEntry> AddAsync(object entity, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }

        public virtual void AddRange(IEnumerable<object> entities)
        {
            throw new NotImplementedException();
        }

        public virtual void AddRange(params object[] entities)
        {
            throw new NotImplementedException();
        }

        public virtual EntityEntry Attach(object entity)
        {
            throw new NotImplementedException();
        }

        public virtual EntityEntry<TEntity> Attach<TEntity>(TEntity entity) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public virtual void AttachRange(IEnumerable<object> entities)
        {
            throw new NotImplementedException();
        }

        public virtual void AttachRange(params object[] entities)
        {
            throw new NotImplementedException();
        }

        public virtual EntityEntry Entry(object entity)
        {
            throw new NotImplementedException();
        }

        public virtual EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public virtual TEntity Find<TEntity>(params object[] keyValues) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public virtual ValueTask<TEntity> FindAsync<TEntity>(object[] keyValues, CancellationToken cancellationToken) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public virtual ValueTask<TEntity> FindAsync<TEntity>(params object[] keyValues) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public virtual ValueTask<object> FindAsync(Type entityType, object[] keyValues, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public virtual ValueTask<object> FindAsync(Type entityType, params object[] keyValues)
        {
            throw new NotImplementedException();
        }

        public virtual object Find(Type entityType, params object[] keyValues)
        {
            throw new NotImplementedException();
        }

        public virtual EntityEntry Remove(object entity)
        {
            throw new NotImplementedException();
        }

        public virtual EntityEntry<TEntity> Remove<TEntity>(TEntity entity) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public virtual void RemoveRange(IEnumerable<object> entities)
        {
            throw new NotImplementedException();
        }

        public virtual void RemoveRange(params object[] entities)
        {
            throw new NotImplementedException();
        }

        public virtual EntityEntry Update(object entity)
        {
            throw new NotImplementedException();
        }

        public virtual EntityEntry<TEntity> Update<TEntity>(TEntity entity) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public virtual void UpdateRange(IEnumerable<object> entities)
        {
            throw new NotImplementedException();
        }

        public virtual void UpdateRange(params object[] entities)
        {
            throw new NotImplementedException();
        }


        // Stored Procedures

        public DbSet<Synonyms_SimpleStoredProcReturnModel> Synonyms_SimpleStoredProcReturnModel { get; set; }
        public List<Synonyms_SimpleStoredProcReturnModel> Synonyms_SimpleStoredProc(int? inputInt)
        {
            int procResult;
            return Synonyms_SimpleStoredProc(inputInt, out procResult);
        }

        public List<Synonyms_SimpleStoredProcReturnModel> Synonyms_SimpleStoredProc(int? inputInt, out int procResult)
        {
            procResult = 0;
            return new List<Synonyms_SimpleStoredProcReturnModel>();
        }

        public Task<List<Synonyms_SimpleStoredProcReturnModel>> Synonyms_SimpleStoredProcAsync(int? inputInt)
        {
            int procResult;
            return Task.FromResult(Synonyms_SimpleStoredProc(inputInt, out procResult));
        }

        // Table Valued Functions

        // dbo.CsvToInt
        public IQueryable<CsvToIntReturnModel> CsvToInt(string array, string array2)
        {
            return new List<CsvToIntReturnModel>().AsQueryable();
        }

        // CustomSchema.CsvToIntWithSchema
        public IQueryable<CustomSchema_CsvToIntWithSchemaReturnModel> CustomSchema_CsvToIntWithSchema(string array, string array2)
        {
            return new List<CustomSchema_CsvToIntWithSchemaReturnModel>().AsQueryable();
        }

        // Scalar Valued Functions

        // dbo.udfNetSale
        public decimal UdfNetSale(int? quantity, decimal? listPrice, decimal? discount)
        {
            return default(decimal);
        }
    }
}
// </auto-generated>
