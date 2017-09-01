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

                if (!contrAgent.WithdrawMoney(invoice)) continue;

                double comission = _bank.CalculateComission(invoice);

                invoice -= comission;
                user.TakeMoney(invoice);

                Chain chain = new Chain(
                    sellerId: user.Id, 
                    buyerId: contrAgent.Id,
                    transactionValue: invoice,
                    transactionComment: $"Invoice: {invoice}");

                _chainStorage.Save(chain);
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
    }
}