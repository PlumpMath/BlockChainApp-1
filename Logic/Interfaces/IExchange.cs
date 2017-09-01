using System.Collections.Generic;
using Logic.Storages;

namespace Logic.Interfaces
{
    public interface IExchange
    {
        IEnumerable<IExchangeUser> GetExchangeUsers();

        void ExecuteExchanging();

        void IncreaseAccountValues();

        void SetChainChangeListener(IChainChangeListener listener);
    }
}