using Logic.Entitites;
using Logic.Interfaces;
using Logic.Observation;
using Logic.Participants;

namespace Logic.Bank
{
    /// <summary>
    /// Банк, осуществляющий операции
    /// </summary>
    public interface IBank : IObserverable
    {
        /// <summary>
        /// Вычисление комиссии банковского обсулживания транзакции
        /// </summary>
        /// <param name="invoice"></param>
        /// <returns></returns>
        double CalculateComission(double invoice);

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
        double GetAccountValue(long userId);

        /// <summary>
        /// Получение количества денег у указанного пользователя
        /// </summary>
        BankAccount GetBankAccount(long userId);

        /// <summary>
        /// Осуществление перевода денег
        /// </summary>
        void TransferMoney(IExchangeUser agent, IExchangeUser contrAgent, double value);

        IExchangeUser GetExchangeUser();
    }
}