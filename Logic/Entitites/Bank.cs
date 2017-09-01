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

        public double GetAccountValue(long userId)
        {
            BankAccount account = GetAccountByUserId(userId);
            return account.AccountValue;
        }

        public void PutMoneyToTheAccount(long userId, double value)
        {
            BankAccount account = GetAccountByUserId(userId);
            account.AccountValue += value;
            SyncBankAccountMoney();
        }

        public void WithdrawMoney(long userId, double value)
        {
            BankAccount account = GetAccountByUserId(userId);
            account.AccountValue -= value;
            SyncBankAccountMoney();
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

            SyncBankAccountMoney();
            return true;
        }

        public double CalculateComission(double invoice)
        {
            _allMoney += invoice * BankComission;
            SyncBankAccountMoney();
            return invoice * BankComission;
        }

        public void CreateAccount(IExchangeUser user, int seed = 0)
        {
            double money = user is Bank 
                ? _allMoney 
                : GetRandomMoney(seed);
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
                if (account.UserId == this.Id 
                    || _allMoney < percent) continue;

                _allMoney -= percent;
                account.AccountValue += percent;
            }
            SyncBankAccountMoney();
        }

        private BankAccount GetAccountByUserId(long userId)
        {
            var result = _accounts.SingleOrDefault(ac => ac.UserId == userId);
            if (result == null) throw new NullReferenceException(nameof(result));
            return result;
        }

        private void SyncBankAccountMoney()
        {
            var mine = GetAccountByUserId(this.Id);
            mine.AccountValue = _allMoney;
        }
    }

    internal class BankAccount
    {
        public long UserId { get; set; }

        public double AccountValue { get; set; }
    }
}