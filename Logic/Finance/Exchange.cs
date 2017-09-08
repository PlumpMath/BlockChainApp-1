using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Logic.Bank;
using Logic.DependencyInjector;
using Logic.Extensions;
using Logic.Interfaces;
using Logic.Observation;
using Logic.Participants;
using Logic.Storages;
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

        public ExchangeStepResult ExecuteExchanging()
        {
            var result = new ExchangeStepResult();
            foreach (IExchangeUser user in _exchangeUsers)
            {
                
                IExchangeUser buyer = user;
                IExchangeUser seller = GetContrAgent(buyer.Id);
                if (buyer.WannaMissTurn())
                {
                    // Если участник-покупатель не захотел торговать сейчас
                    //continue;
                }
                ExchangeStepResult dealResult = TryMakeDeal(buyer, seller);
                if (dealResult == null) continue;
                result.StepDealSumm += dealResult.StepDealSumm;
                result.StepDealBankComission += dealResult.StepDealBankComission;
                result.StepDealCount++;
            }
            return result.StepDealCount == 0 ? null : result;
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

        private ExchangeStepResult TryMakeDeal(IExchangeUser buyer, IExchangeUser seller)
        {
            ICollection<Share> sellerShares = seller.GetOwnedShares();
            if (!sellerShares.Any())
            {
                // если у продавца нет акций
                return null;
            }

            ShareInvoiceInfo invoice = sellerShares.GetRandomShareInvoiceInfo(buyer.MakeInvoiceOffer());
            if (invoice == null)
            {
                return null;
            }

            if (!buyer.WannaBuyShares(invoice))
            {
                // Если покупатель не захотел покупать акции по некоторой причине
                seller.DecreaseSharePriceIfWantTo(invoice.CompanyId);
                return null;
            }

            seller.DeattachShares(invoice);
            buyer.TakeShares(invoice);
            // Кол-во денег вычисляется рандомно
            // double invoice = MiscUtils.GetRandomNumber(MaxTransactionPrice);

            bool success = _bank.TransferMoney(buyer, seller, invoice.Cost, out double comission);

            // Возвращается единичный результат, чтобы быть сложенным с остальными на уровень выше
            if (!success) return null;
            return new ExchangeStepResult
            {
                StepDealSumm = invoice.Cost,
                StepDealBankComission = comission,
                StepDealCount = 1
            };
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