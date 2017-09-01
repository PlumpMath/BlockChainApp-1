using System.Linq;

namespace Dal.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> GetAll();

        TEntity Find(long entityId);

        TEntity SaveOrUpdate(TEntity entity);

        TEntity Add(TEntity entity);

        TEntity Update(TEntity entity);

        void Delete(TEntity entity);
    }
}