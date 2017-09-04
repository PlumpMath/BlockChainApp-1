using Logic.Bank;
using Logic.Observation;
using Logic.Participants;

namespace Logic.Interfaces
{
    /// <summary>
    /// Банк, осуществляющий операции
    /// </summary>
    public interface IBank : IObserverable, IHasExchangeUser
    {
        /// <summary>
        /// Открытие счета в банке
        /// </summary>
        void CreateAccount(IExchangeUser user);

        /// <summary>
        /// Указание банку выплатить проценты по депозитам
        /// </summary>
        double PayoutDepositPercent();

        /// <summary>
        /// Общее количество денег в системе
        /// </summary>
        double GetMoneyAmount();

        /// <summary>
        /// Получение количества денег у указанного пользователя
        /// </summary>
        BankAccount GetBankAccount(string uniqueUserId);

        /// <summary>
        /// Получение количества денег у указанного пользователя
        /// </summary>
        IExchangeUser GetOwnerByBankAccount(string accountUniqueId);

        /// <summary>
        /// Осуществление перевода денег
        /// </summary>
        bool TransferMoney(IExchangeUser sender, IExchangeUser receiver, double invoice);
    }
}