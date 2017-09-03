using System.Collections.Generic;
using Logic.Entitites;
using Logic.Participants;

namespace Logic.Interfaces
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

        TEntity GetLastEntity();

        IEnumerable<TEntity> GetAll();
    }
}