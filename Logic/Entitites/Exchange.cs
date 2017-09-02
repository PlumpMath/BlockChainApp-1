﻿using System;
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
            foreach (IExchangeUser user in _exchangeUsers)
            {
                if (MiscUtils.ContinueByRandom())
                {
                    // Как бдуто участник просто не захотел торговаться на этот ход
                    continue;
                } 
                IExchangeUser seller = user;
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

        /// <summary>
        /// Выплата процентов по депозитам
        /// </summary>
        public void PayoutDepositPercents()
        {
            double allPercents = _bank.PayoutDepositPercent();
            var text = $"Выплачены проценты по депозитам {MiscUtils.FormatDouble(allPercents)}";
            _exchangeEventListener?.CommonMessage(text);
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
        /// <param name="listener"></param>
        public void SetChainChangeListener(IExchangeEventListener listener)
        {
            _exchangeEventListener = listener;
        }
    }
}