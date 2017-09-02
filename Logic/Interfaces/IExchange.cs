using System.Collections.Generic;
using Logic.Storages;

namespace Logic.Interfaces
{
    /// <summary>
    /// Биржа, где проводятся торги
    /// </summary>
    public interface IExchange : IObserverable
    {
        /// <summary>
        /// Участники торгов
        /// </summary>
        /// <returns></returns>
        IEnumerable<IExchangeUser> GetExchangeUsers();

        /// <summary>
        /// Осуществление торгов. Должно выполняться периодически
        /// </summary>
        void ExecuteExchanging();

        /// <summary>
        /// Указание банковскому регулятору выплатить проценты по депозитам
        /// </summary>
        void PayoutDepositPercents();
    }
}