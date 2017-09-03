using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Logic.Bank;
using Logic.Interfaces;
using Logic.Observation;
using Logic.Participants;
using Utilities.Common;
using Utilities.Convert;

namespace Logic.Finance
{
    public class Exchange : IExchange
    {
        private static readonly double MaxTransactionPrice
            = ConfigurationManager.AppSettings["MaxTransactionPrice"].ParseAsDouble();

        /// <summary>
        /// Список участников
        /// </summary>
        private readonly List<IExchangeUser> _exchangeUsers;

        private readonly List<Company> _companies;

        /// <summary>
        /// Управляющий транзакциями банк
        /// </summary>
        private readonly IBank _bank;

        private IObserver _observer;

        public Exchange(IBank bank, IEnumerable<IExchangeUser> users, IEnumerable<Company> companies)
        {
            _bank = bank;
            _exchangeUsers = new List<IExchangeUser>();
            _exchangeUsers.AddRange(users);

            _companies = new List<Company>();
            _companies.AddRange(companies);
        }

        public IEnumerable<IExchangeUser> GetExchangeUsers() => _exchangeUsers;

        public void ExecuteExchanging()
        {
            foreach (IExchangeUser user in _exchangeUsers)
            {
                
                IExchangeUser seller = user;
                IExchangeUser buyer = GetContrAgent(seller.Id);
                if (user.WannaMissTurn() || buyer.WannaMissTurn())
                {
                    // участники просто не захотели торговаться на этот ход
                    continue;
                }

                // Кол-во денег вычисляется рандомно
                double invoice = MiscUtils.GetRandomNumber(MaxTransactionPrice);

                _bank.TransferMoney(buyer, seller, invoice);
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