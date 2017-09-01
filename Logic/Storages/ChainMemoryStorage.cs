using System.Collections.Generic;
using Logic.Entitites;
using Logic.Interfaces;

namespace Logic.Storages
{
    public class ChainMemoryStorage : EntityMemoryStorageBase<Chain>, IChainStorage
    {
        public override void Save(Chain entity)
        {
            if (EntityCount() > 0)
            {
                Chain lastChain = GetLastEntity();
                entity.PreviousHash = lastChain.CurrentHash;
                entity.ComputeThisHash();
            }
            base.Save(entity);
        }
    }

    public interface IChainStorage : IEntityStorage<Chain> { }
}