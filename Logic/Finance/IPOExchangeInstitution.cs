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
    public class IPOExchangeInstitution
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

        public delegate void IPOExchangeStepExecuted(ExchangeStepResult result);

        /// <summary>
        /// Событие, происходящее по исполнению торгов
        /// </summary>
        public event IPOExchangeStepExecuted ExchangeStepExecuted;

        public IPOExchangeInstitution(IBank bank, IEnumerable<IExchangeUser> users, IEnumerable<Company> companies)
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
            var result = new ExchangeStepResult();
            foreach (IExchangeUser user in _exchangeUsers)
            {
                
                IExchangeUser buyer = user;
                IExchangeUser seller = GetContrAgent(buyer.Id);
                if (buyer.WannaMissTurn())
                {
                    // Если участник-покупатель не захотел торговать сейчас
                    continue;
                }
                if (!TryMakeDeal(buyer, seller, out ShareInvoiceInfo invoice))
                {
                    continue;
                }
                result.StepDealSumm += invoice.Cost;
                result.StepDealBankComission += invoice.Comission;
                result.StepDealCount++;
            }
            if (result.StepDealCount != 0)
            {
                // Если сделка состоялась, то сообщаем слушателям
                ExchangeStepExecuted?.Invoke(result);
            }
        }

        private bool TryMakeDeal(IExchangeUser buyer, IExchangeUser seller, out ShareInvoiceInfo invoice)
        {
            if (!seller.WannaSellShares(buyer.MakeInvoiceOffer(), out invoice))
            {
                return false;
            }

            if (!buyer.WannaBuyShares(invoice))
            {
                // Если покупатель не захотел покупать акции по некоторой причине
                seller.DecreaseSharePriceIfWantTo(invoice.CompanyId);
                return false;
            }

            seller.DeattachShares(invoice);
            buyer.TakeShares(invoice);

            if (!_bank.TransferMoney(buyer, seller, invoice.Cost, out double comission))
            {
                // Если по какой-то причине не получилось трансферинга денег
                return false;
            }
            invoice.Comission = comission;
            return true;
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
    }
}