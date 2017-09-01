using System;
using System.Collections.Generic;
using System.Linq;
using Logic.Interfaces;
using Utilities.Common;

namespace Logic.Entitites
{
    public class Bank : ExchangeUserBase, IBank
    {
        private const double StartCapital = 21000000;

        private const double BankComission = 0.01;

        private const double DepositPercent = 0.05;

        // Всеми деньгами считается собственный счет банка
        public double AllMoney
        {
            get => GetAccountByUserId(this.Id).AccountValue;
            set => GetAccountByUserId(this.Id).AccountValue = value;
        }

        private const double MaximumByHands = 100000;

        private readonly List<BankAccount> _accounts;

        public Bank()
        {
            // Как будто самый старший игрок на рынке
            Id = 1000;

            // При создании открывается свой же собственный счет
            _accounts = new List<BankAccount>
            {
                new BankAccount
                {
                    UserId = this.Id,
                    AccountValue = StartCapital
                }
            };

            Name = "Bank of Bitcoins";
        }

        public double GetRandomMoney(int seed = 0)
        {
            double amount = MiscUtils.GetRandomNumber(MaximumByHands, seed: seed);
            AllMoney -= amount;
            return amount;
        }

        public double GetMoneyAmount()
        {
            return _accounts.Sum(account => account.AccountValue);
        }

        public double GetAccountValue(long userId)
        {
            BankAccount account = GetAccountByUserId(userId);
            return account.AccountValue;
        }

        public void PutMoneyToTheAccount(long userId, double value)
        {
            BankAccount account = GetAccountByUserId(userId);
            account.AccountValue += value;
        }

        public void WithdrawMoney(long userId, double value)
        {
            BankAccount account = GetAccountByUserId(userId);
            account.AccountValue -= value;
        }

        public bool TransferMoney(IExchangeUser seller, IExchangeUser buyer, double invoice, out double comission)
        {
            BankAccount buyerAccount = GetAccountByUserId(buyer.Id);
            // Если нет денег
            if (buyerAccount.AccountValue < invoice)
            {
                comission = 0;
                return false;
            }

            BankAccount sellerAccount = GetAccountByUserId(seller.Id);
            comission = CalculateComission(invoice);

            sellerAccount.AccountValue += invoice - comission;
            buyerAccount.AccountValue -= invoice;

            return true;
        }

        public double CalculateComission(double invoice)
        {
            var result = invoice * BankComission;
            AllMoney += result;
            return result;
        }

        public void CreateAccount(IExchangeUser user, int seed = 0)
        {
            double money = user is Bank 
                ? AllMoney 
                : GetRandomMoney(seed);
            _accounts.Add(new BankAccount
            {
                UserId = user.Id,
                AccountValue = money
            });
        }

        public double PayoutDepositPercent()
        {
            double allPercents = 0;
            foreach (BankAccount account in _accounts)
            {
                double percent = account.AccountValue * DepositPercent;
                if (account.UserId == this.Id 
                    || AllMoney < percent) continue;

                allPercents += percent;
                AllMoney -= percent;
                account.AccountValue += percent;
            }
            return allPercents;
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