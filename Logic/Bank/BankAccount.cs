using Logic.Interfaces;

namespace Logic.Bank
{
    public class BankAccount
    {
        /// <summary>
        /// Уникальный идентификатор, совпадает с идентификатором владельца
        /// </summary>
        public string UniqueUserId { get; set; }

        /// <summary>
        /// Кол-во денег на счету
        /// </summary>
        public double AccountValue { get; set; }

        /// <summary>
        /// Процентная ставка по депозитному счету
        /// </summary>
        public double DepositPercent { get; set; }

        /// <summary>
        /// Размер ставки комиссии, распространяемой на транзакции
        /// </summary>
        public double TransactionComissionRate { get; set; }

        /// <summary>
        /// Пользователь, которому принадлежит счет
        /// </summary>
        public IExchangeUser ExchangeUser { get; set; }
    }
}