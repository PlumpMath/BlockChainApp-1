using System;
using System.Collections.Generic;
using System.Linq;
using Logic.Interfaces;
using Logic.Storages;
using Utilities.Common;

namespace Logic.Entitites
{
    public class Exchange : IExchange
    {
        private const double MaxTransactionPrice = 5000;

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
                IExchangeUser user = _exchangeUsers[index];
                IExchangeUser contrAgent = GetContrAgent(user.Id);
                double invoice = MiscUtils.GetRandomNumber(MaxTransactionPrice, seed: index);

                if (!_bank.TransferMoney(user, contrAgent, invoice, out double comission))
                {
                    continue;
                }

                Chain chain = new Chain(
                    sellerId: user.Id,
                    buyerId: contrAgent.Id,
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