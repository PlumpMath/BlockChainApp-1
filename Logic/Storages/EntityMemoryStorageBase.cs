using System;
using System.Collections.Generic;
using System.Linq;
using Logic.Entitites;
using Logic.Interfaces;
using Logic.Participants;

namespace Logic.Storages
{
    public class EntityMemoryStorageBase<TEntity> : IEntityStorage<TEntity>
        where TEntity : class, IIdentificable
    {
        private readonly List<TEntity> _list;

        protected EntityMemoryStorageBase()
        {
            _list = new List<TEntity>();
        }

        protected int EntityCount() => _list.Count;

        public virtual void Save(TEntity entity)
        {
            if (entity.Id != 0 
                && _list.All(item => item.Id != entity.Id))
            {
                int key = _list.FindIndex(item => item.Id == entity.Id);
                _list[key] = entity;
            }
            else
            {
                int id = _list.Count+1;
                entity.Id = id;
                _list.Add(entity);
            }
        }

        public void Save(params TEntity[] entities)
        {
            foreach (TEntity entity in entities)
            {
                Save(entity);
            }
        }

        public void Save(IEnumerable<TEntity> entities)
        {
            Save(entities.ToArray());
        }

        public virtual TEntity GetLastEntity() => GetAll().Last();

        public virtual IEnumerable<TEntity> GetAll() =>_list.OrderBy(item => item.Id);

        public virtual TEntity GetEntity(long id) => GetAll().SingleOrDefault(item => item.Id == id);
    }
}