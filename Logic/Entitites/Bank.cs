﻿using System;
using System.Collections.Generic;
using System.Linq;
using Logic.Interfaces;
using Utilities.Common;

namespace Logic.Entitites
{
    public class Bank : ExchangeUserBase, IBank
    {
        private const double BankComission = 0.05;

        private const double DepositPercent = 0.5;

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

        public double GetRandomMoney(int seed = 0)
        {
            double amount = MiscUtils.GetRandomNumber(MaximumByHands, seed: seed);
            _allMoney -= amount;
            return amount;
        }

        public double GetMoneyAmount() => _allMoney;

        public double GetAccountValue(IExchangeUser user)
        {
            BankAccount account = _accounts.SingleOrDefault(ac => ac.UserId == user.Id);
            if (account == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return account.AccountValue;
        }

        public void PutMoneyToTheAccount(IExchangeUser user, double value)
        {
            BankAccount account = GetAccountByUserId(user.Id);
            account.AccountValue += value;
        }

        public void WithdrawMoney(IExchangeUser user, double value)
        {
            BankAccount account = GetAccountByUserId(user.Id);
            account.AccountValue -= value;
        }

        public bool TransferMoney(IExchangeUser seller, IExchangeUser buyer, double invoice)
        {
            BankAccount buyerAccount = GetAccountByUserId(buyer.Id);
            // Если нет денег
            if (buyerAccount.AccountValue < invoice) return false;

            BankAccount sellerAccount = GetAccountByUserId(seller.Id);
            double comission = CalculateComission(invoice);

            sellerAccount.AccountValue += invoice - comission;
            buyerAccount.AccountValue -= invoice;

            return true;
        }

        public double CalculateComission(double invoice)
        {
            _allMoney += invoice * BankComission;
            return invoice * BankComission;
        }

        public void CreateAccount(IExchangeUser user, int seed = 0)
        {
            double money = GetRandomMoney(seed);
            _accounts.Add(new BankAccount
            {
                UserId = user.Id,
                AccountValue = money
            });
        }

        public void IncreaseAccountMoneyValue()
        {
            foreach (BankAccount account in _accounts)
            {
                double percent = account.AccountValue * DepositPercent;
                if (_allMoney < percent) continue;

                account.AccountValue += percent;
            }
        }

        private BankAccount GetAccountByUserId(long userId)
        {
            var result = _accounts.SingleOrDefault(ac => ac.UserId == userId);
            if (result == null) throw new NullReferenceException(nameof(result));
            return result;
        }
    }

    internal class BankAccount
    {
        public long UserId { get; set; }

        public double AccountValue { get; set; }
    }
}