using System;
using System.Collections.Generic;
using System.Linq;
using Logic.Entitites;
using Logic.Interfaces;

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

        public TEntity GetLastEntity() => GetAll().Last();

        public IEnumerable<TEntity> GetAll() =>_list.OrderBy(item => item.Id);

        public TEntity GetEntity(long id) => GetAll().SingleOrDefault(item => item.Id == id);
    }
}