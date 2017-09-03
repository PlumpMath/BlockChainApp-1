using System.Collections.Generic;

namespace Logic.Fabrics
{
    public interface IFabricBase<TEntity>
    {
        TEntity GetEntity();

        IEnumerable<TEntity> GetEntities(int count);
    }
}