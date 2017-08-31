using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using Dal.Dal;

namespace Dal.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IQueryable GetAll();

        TEntity Find(long entityId);

        TEntity SaveOrUpdate(TEntity entity);

        TEntity Add(TEntity entity);

        TEntity Update(TEntity entity);

        void Delete(TEntity entity);
    }
}