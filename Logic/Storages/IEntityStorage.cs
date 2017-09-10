using System.Collections.Generic;
using Logic.Interfaces;

namespace Logic.Storages
{
    /// <summary>
    /// Хранилище для некоторой сущности
    /// </summary>
    public interface IEntityStorage<TEntity> where TEntity : class, IId
    {
        void Save(TEntity entity);

        void Save(params TEntity[] entities);

        void Save(IEnumerable<TEntity> entities);

        TEntity GetEntity(long id);

        TEntity GetLastEntity();

        ICollection<TEntity> GetAll();
    }
}