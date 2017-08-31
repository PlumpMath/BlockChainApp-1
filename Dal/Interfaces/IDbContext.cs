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

        IQueryable<T> GetAll<T>() where T : class;

        void MarkAsAdded<T>(T entity) where T : class;

        void MarkAsDeleted<T>(T entity) where T : class;

        void MarkAsModified<T>(T entity) where T : class;

        void Attach<T>(T entity) where T : class;

        void Commit();

        //откатывает изменения во всех модифицированных объектах
        void Rollback();

        // включает или отключает отслеживание изменений объектов
        void EnableTracking(bool isEnable);

        EntityState GetEntityState<T>(T entity) where T : class;

        void SetEntityState<T>(T entity, EntityState state) where T : class;

        // возвращает объект содержащий список объектов с их состоянием
        DbChangeTracker GetChangeTracker();

        DbEntityEntry GetDbEntry<T>(T entity) where T : class;

        #endregion
    }
}