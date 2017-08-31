using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using Dal.Entitites;

namespace Dal.Interfaces
{
    // https://stackoverflow.com/a/20228695
    public interface IDbContext
    {
        DbSet<Chain> Chains { get; set; }

        #region Функции

        IQueryable<TEntity> GetAll<TEntity>() where TEntity : class;

        void MarkAsAdded<TEntity>(TEntity entity) where TEntity : class;

        void MarkAsDeleted<TEntity>(TEntity entity) where TEntity : class;

        void MarkAsModified<TEntity>(TEntity entity) where TEntity : class;

        void Attach<TEntity>(TEntity entity) where TEntity : class;

        void Commit();

        //откатывает изменения во всех модифицированных объектах
        void Rollback();

        // включает или отключает отслеживание изменений объектов
        void EnableTracking(bool isEnable);

        EntityState GetEntityState<TEntity>(TEntity entity) where TEntity : class;

        void SetEntityState<TEntity>(TEntity entity, EntityState state) where TEntity : class;

        // возвращает объект содержащий список объектов с их состоянием
        DbChangeTracker GetChangeTracker();

        DbEntityEntry GetDbEntry<TEntity>(TEntity entity) where TEntity : class;

        #endregion
    }
}