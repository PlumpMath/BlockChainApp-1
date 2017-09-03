using System.Collections.Generic;
using Logic.Interfaces;

namespace Logic.Storages
{
    /// <summary>
    /// Хранилище для некоторой сущности
    /// </summary>
    public interface IEntityStorage<TEntity> where TEntity : class, IIdentificable
    {
        void Save(TEntity entity);

        void Save(params TEntity[] entities);

        void Save(IEnumerable<TEntity> entities);

        TEntity GetEntity(long id);

        TEntity GetEntity(string uniqueId);

        TEntity GetLastEntity();

        IEnumerable<TEntity> GetAll();
    }
}