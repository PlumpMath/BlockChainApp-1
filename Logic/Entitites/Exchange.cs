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

        private readonly List<IExchangeUser> _exchangeUsers;

        private readonly IBank _bank;

        private readonly IChainStorage _chainStorage;

        private IExchangeEventListener _exchangeEventListener;

        public Exchange(IBank bank, IChainStorage chainStorage, IEnumerable<IExchangeUser> users)
        {
            _bank = bank;
            _chainStorage = chainStorage;
            _exchangeUsers = new List<IExchangeUser>();

            foreach (IExchangeUser user in users)
            {
                _exchangeUsers.Add(user);
            }
        }

        public IEnumerable<IExchangeUser> GetExchangeUsers() => _exchangeUsers;

        public void ExecuteExchanging()
        {
            for (var index = 0; index < _exchangeUsers.Count; index++)
            {
                if (MiscUtils.ContinueByRandom(index))
                {
                    continue;
                } 
                IExchangeUser seller = _exchangeUsers[index];
                IExchangeUser buyer = GetContrAgent(seller.Id);


                double invoice = MiscUtils.GetRandomNumber(MaxTransactionPrice);
                double buyerMoney = buyer.GetBankAccountValue();
                double sellerMoney = seller.GetBankAccountValue();

                if (buyerMoney > sellerMoney)
                {
                    //invoice = invoice + (buyerMoney - sellerMoney) * MaxTransactionDifferenceRate;
                }

                if (!_bank.TransferMoney(seller, buyer, invoice, out double comission))
                {
                    continue;
                }

                Chain chain = new Chain(
                    sellerId: seller.Id,
                    buyerId: buyer.Id,
                    transactionValue: invoice,
                    transactionComission: comission);

                _chainStorage.Save(chain);
                _exchangeEventListener?.Transaction(chain);
            }
        }

        public void PayoutDepositPercents()
        {
            double allPercents = _bank.PayoutDepositPercent();
            var text = $"Выплачены проценты по депозитам {MiscUtils.FormatDouble(allPercents)}";
            _exchangeEventListener?.CommonMessage(text);
        }


        private IExchangeUser GetContrAgent(long excludeId)
        {
            return _exchangeUsers
                .Where(user => user.Id != excludeId)
                .GetRandomEntity();
        }

        public void SetChainChangeListener(IExchangeEventListener listener)
        {
            _exchangeEventListener = listener;
        }
    }
}