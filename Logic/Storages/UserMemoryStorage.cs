using Logic.Bank;
using Logic.DependencyInjector;
using Logic.Entitites;
using Logic.Interfaces;
using Logic.Participants;

namespace Logic.Storages
{
    public class ExchangeUserMemoryStorage : EntityMemoryStorageBase<ExchangeUserBase>, IExchangeUserStorage
    {
        public override ExchangeUserBase GetEntity(long id)
        {
            if (id == long.MaxValue)
            {
                // под максимальным id "прячется" банк
                return (ExchangeUserBase) Injector.Get<IBank>().GetExchangeUser();
            }
            return base.GetEntity(id);
        }
    }

    public interface IExchangeUserStorage : IEntityStorage<ExchangeUserBase> { }
}