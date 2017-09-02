using System.Collections.Generic;
using Logic.Entitites;

namespace Logic.Interfaces
{
    /// <summary>
    /// Хранилище для некоторой сущности
    /// </summary>
    public interface IEntityStorage<TEntity> where TEntity : class, IIdentificable
    {
        void Save(TEntity entity);

        TEntity GetEntity(long id);

        TEntity GetLastEntity();

        IEnumerable<TEntity> GetAll();
    }
}