using Logic.DependencyInjector;
using Logic.Entitites;
using Logic.Interfaces;

namespace Logic.Storages
{
    public class ExchangeUserMemoryStorage : EntityMemoryStorageBase<ExchangeUserBase>, IExchangeUserStorage
    {
        public override ExchangeUserBase GetEntity(long id)
        {
            if (id == long.MaxValue)
            {
                // под максимальным id "прячется" банк
                return (ExchangeUserBase) DI.Get<IBank>();
            }
            return base.GetEntity(id);
        }
    }

    public interface IExchangeUserStorage : IEntityStorage<ExchangeUserBase> { }
}