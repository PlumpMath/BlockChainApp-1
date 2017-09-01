using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using Dal.Interfaces;

namespace Dal.Dal.Parent
{
    // https://habrahabr.ru/post/219947/
    public abstract class BaseRepository<TEntity> 
        : IRepository<TEntity> where TEntity : class, IDbEntity
    {
        private readonly IDbContext _context;

        protected BaseRepository(IDbContext context)
        {
            _context = context;
        }

        public IQueryable<TEntity> GetAll()
        {
            return _context.GetAll<TEntity>();
        }

        public TEntity Find(long entityId)
        {
            return _context
                .GetAll<TEntity>()
                .SingleOrDefault(en => en.Id == entityId);
        }

        public TEntity SaveOrUpdate(TEntity entity)
        {
            var iDbEntity = entity as IDbEntity;

            if (iDbEntity == null)
                throw new ArgumentException("entity should be IDbEntity type", nameof(entity));

            return iDbEntity.Id == 0 
                ? Add(entity) 
                : Update(entity);
        }

        public TEntity Add(TEntity entity)
        {
            BeforeSave(entity);
            _context.MarkAsAdded(entity);
            _context.Commit();
            return entity;
        }

        public TEntity Update(TEntity entity)
        {
            var iDbEntity = entity as IDbEntity;
            if (iDbEntity == null)
                throw new ArgumentException("entity should be IDbEntity type", nameof(entity));

            TEntity attachedEntity = Find(iDbEntity.Id);
            _context.GetDbEntry(attachedEntity).CurrentValues.SetValues(entity);

            BeforeSave(attachedEntity);
            _context.Commit();
            return entity;
        }

        public void Delete(TEntity entity)
        {
            _context.MarkAsDeleted(entity);
            _context.Commit();
        }

        // виртуальный метод. вызывает перед сохранением объектов, может быть определен в дочерних классах
        protected virtual void BeforeSave(TEntity entity)
        {

        }

        public DbEntityValidationResult Validate(TEntity entity)
        {
            return _context.GetDbEntry(entity).GetValidationResult();
        }
    }
}