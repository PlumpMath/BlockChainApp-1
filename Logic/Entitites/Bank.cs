using System;
using System.Collections.Generic;
using System.Linq;
using Logic.Interfaces;
using Utilities.Common;

namespace Logic.Entitites
{
    public class Bank : ExchangeUserBase, IBank
    {
        private const double BankComission = 0.05;

        private double _allMoney;

        private const double MaximumByHands = 10000;

        private readonly List<BankAccount> _accounts;

        public Bank()
        {
            // Как будто самый старший игрок на рынке
            Id = 1000;

            _allMoney = 21000000;
            _accounts = new List<BankAccount>();
            Name = "Bank of Bitcoins";
        }

        public double GetRandomMoney()
        {
            return MiscUtils.GetRandomNumber(MaximumByHands);
        }

        public double CalculateComission(double invoice)
        {
            _allMoney += invoice * BankComission;
            return invoice * BankComission;
        }

        public void CreateAccount(IExchangeUser user)
        {
            if (_accounts.All(account => account.UserId != user.Id))
            {
                throw new ArgumentException();
            }
            _accounts.Add(new BankAccount(user.Id)
            {
                AccountValue = GetRandomMoney()
            });
        }

        public void IncreaseAccountMoneyValue()
        {

            foreach (BankAccount account in _accounts)
            {
                double randomMoney = GetRandomMoney();
                if (_allMoney < randomMoney) continue;

                account.AccountValue += randomMoney;
            }
        }
    }

    internal class BankAccount
    {
        public long UserId { get; }

        public double AccountValue { get; set; }

        public BankAccount(long userId)
        {
            UserId = userId;
        }
    }
}