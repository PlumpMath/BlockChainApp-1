using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SQLite;
using System.Linq;
using Dal.Entitites;
using Dal.Interfaces;

namespace Dal.Dal
{
    [DbConfigurationType(typeof(SqLiteConfiguration))]
    public class SqLiteContext : DbContext, IDbContext
    {

        private static readonly SQLiteConnection connection = new SQLiteConnection
        {
            ConnectionString = new SQLiteConnectionStringBuilder
            {
                DataSource = Environment.CurrentDirectory+ "\\Data\\Database.db", ForeignKeys = true
            }.ConnectionString
        };

        public SqLiteContext() 
            : base(connection, true)
        {
        }

        public DbSet<Chain> Chains { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            
        }

        #region IDbContext

        // https://habrahabr.ru/post/219947/

        public IQueryable<TEntity> GetAll<TEntity>() where TEntity : class
        {
            return Set<TEntity>();
        }

        public void MarkAsAdded<TEntity>(TEntity entity) where TEntity : class
        {
            Entry(entity).State = EntityState.Added;
            Set<TEntity>().Add(entity);
        }

        public void MarkAsDeleted<TEntity>(TEntity entity) where TEntity : class
        {
            Attach(entity);
            Entry(entity).State = EntityState.Deleted;
            Set<TEntity>().Remove(entity);
        }

        public void MarkAsModified<TEntity>(TEntity entity) where TEntity : class
        {
            Attach(entity);
            Entry(entity).State = EntityState.Modified;
        }

        public void Attach<TEntity>(TEntity entity) where TEntity : class
        {
            if (Entry(entity).State == EntityState.Detached)
            {
                Set<TEntity>().Attach(entity);
            }
        }

        public void Commit()
        {
            BeforeCommit();
            SaveChanges();
        }

        private void BeforeCommit()
        {
            UndoExistAddedEntitys();
        }

        //исправление ситуации, когда есть объекты помеченные как новые, но при этом существующие в базе данных
        private void UndoExistAddedEntitys()
        {
            IEnumerable<DbEntityEntry> dbEntityEntries = GetChangeTracker()
                .Entries()
                .Where(x => x.State == EntityState.Added);
            foreach (var dbEntityEntry in dbEntityEntries)
            {
                if (GetKeyValue(dbEntityEntry.Entity) > 0)
                {
                    SetEntityState(dbEntityEntry.Entity, EntityState.Unchanged);
                }
            }
        }

        public static long GetKeyValue<T>(T entity) where T : class
        {
            var dbEntity = entity as IDbEntity;
            if (dbEntity == null)
                throw new ArgumentException("Entity should be IDbEntity type - " + entity.GetType().Name);

            return dbEntity.Id;
        }

        public void Rollback()
        {
            ChangeTracker
                .Entries()
                .ToList()
                .ForEach(x => x.Reload());
        }

        public void EnableTracking(bool isEnable)
        {
            Configuration.AutoDetectChangesEnabled = isEnable;
        }

        public EntityState GetEntityState<TEntity>(TEntity entity) where TEntity : class
        {
            return Entry(entity).State;
        }

        public void SetEntityState<TEntity>(TEntity entity, EntityState state) where TEntity : class
        {
            Entry(entity).State = state;
        }

        public DbChangeTracker GetChangeTracker()
        {
            return ChangeTracker;
        }

        public DbEntityEntry GetDbEntry<TEntity>(TEntity entity) where TEntity : class
        {
            return Entry(entity);
        }

        #endregion

        #region Вспомогательные методы

        static SqLiteContext()
        {
            //устанавливаем инициализатор
            Database.SetInitializer(new CreateDatabaseIfNotExists<SqLiteContext>());
        }

        #endregion
    }
}