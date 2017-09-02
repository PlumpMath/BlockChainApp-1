using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Logic.Extensions;
using Logic.Interfaces;
using Logic.Storages;
using Utilities.Common;
using Utilities.Convert;

namespace Logic.Entitites
{
    public class Exchange : IExchange
    {
        private static readonly double MaxTransactionPrice
            = ConfigurationManager.AppSettings["MaxTransactionPrice"].ParseAsDouble();

        private static readonly double MaxTransactionDifferenceRate
            = ConfigurationManager.AppSettings["MaxTransactionDifferenceRate"].ParseAsDouble();

        /// <summary>
        /// Список участников
        /// </summary>
        private readonly List<IExchangeUser> _exchangeUsers;

        /// <summary>
        /// Управляющий транзакциями банк
        /// </summary>
        private readonly IBank _bank;

        private IObserver _observer;

        public Exchange(IBank bank, IEnumerable<IExchangeUser> users)
        {
            _bank = bank;
            _exchangeUsers = new List<IExchangeUser>();
            _exchangeUsers.AddRange(users);
        }

        public IEnumerable<IExchangeUser> GetExchangeUsers() => _exchangeUsers;

        public void ExecuteExchanging()
        {
            foreach (IExchangeUser user in _exchangeUsers)
            {
                if (user.WannaMissTurn())
                {
                    // участник просто не захотел торговаться на этот ход
                    continue;
                } 
                IExchangeUser seller = user;
                IExchangeUser buyer = GetContrAgent(seller.Id);

                // Кол-во денег вычисляется рандомно
                double invoice = MiscUtils.GetRandomNumber(MaxTransactionPrice);

                _bank.TransferMoney(seller, buyer, invoice);
            }
        }

        /// <summary>
        /// Выплата процентов по депозитам
        /// </summary>
        public void PayoutDepositPercents()
        {
            double allPercents = _bank.PayoutDepositPercent();
            var text = $"Выплачены проценты по депозитам {MiscUtils.FormatDouble(allPercents)}";
            _observer?.CommonMessage(text);
        }

        /// <summary>
        /// Возвращает рандомного контр-агента, но при этом исключается сам участник
        /// </summary>
        private IExchangeUser GetContrAgent(long excludeId)
        {
            return _exchangeUsers
                .Where(user => user.Id != excludeId)
                .GetRandomEntity();
        }

        /// <summary>
        /// Сложно передавать в конструктор
        /// </summary>
        public void SetChainChangeListener(IObserver listener)
        {
            _observer = listener;
        }
    }
}