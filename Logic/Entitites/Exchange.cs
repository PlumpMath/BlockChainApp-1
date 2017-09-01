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
        private readonly List<IExchangeUser> _exchangeUsers;

        private readonly IBank _bank;

        private readonly IChainStorage _chainStorage;

        private IChainChangeListener _chainChangeListener;

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
            foreach (IExchangeUser user in _exchangeUsers)
            {
                IExchangeUser contrAgent = GetContrAgent(user.Id);
                double invoice = MiscUtils.GetRandomNumber(1000.0);

                if (!_bank.TransferMoney(user, contrAgent, invoice)) continue;

                Chain chain = new Chain(
                    sellerId: user.Id, 
                    buyerId: contrAgent.Id,
                    transactionValue: invoice,
                    transactionComment: $"Invoice: {invoice}");

                _chainStorage.Save(chain);
                _chainChangeListener?.NewChainAdded(chain);
            }
        }

        public void IncreaseAccountValues()
        {
            _bank.IncreaseAccountMoneyValue();
        }


        private IExchangeUser GetContrAgent(long excludeId)
        {
            return _exchangeUsers
                .Where(user => user.Id != excludeId)
                .GetRandomEntity();
        }

        public void SetChainChangeListener(IChainChangeListener listener)
        {
            _chainChangeListener = listener;
        }
    }
}