using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Logic.Exceptions;
using Logic.Interfaces;
using Utilities.Common;
using Utilities.Convert;

namespace Logic.Entitites
{
    public class Bank : ExchangeUserBase, IBank
    {
        private static readonly double StartCapital 
            = ConfigurationManager.AppSettings["StartCapital"].ParseAsDouble();

        private static readonly double BankComission 
            = ConfigurationManager.AppSettings["BankComission"].ParseAsDouble();

        private static readonly double DepositPercent
            = ConfigurationManager.AppSettings["DepositPercent"].ParseAsDouble();

        private static readonly double MaximumByHands
            = ConfigurationManager.AppSettings["MaximumByHands"].ParseAsDouble();

        private static readonly double MinimumByHands
            = ConfigurationManager.AppSettings["MinimumByHands"].ParseAsDouble();

        // Всеми деньгами считается собственный счет банка
        public double AllMoney
        {
            get => GetAccountByUserId(this.Id).AccountValue;
            set => GetAccountByUserId(this.Id).AccountValue = value;
        }

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

        /// <summary>
        /// Возвращает рандомное кол-во денег для участника в разрешенном диапазоне 
        /// </summary>
        public double GetRandomMoney()
        {
            double amount = MiscUtils.GetRandomNumber(MaximumByHands, MinimumByHands);
            AllMoney -= amount;
            return amount;
        }

        /// <summary>
        /// Возвращает общее количество денег в системе (у всех участников)
        /// </summary>
        /// <returns></returns>
        public double GetMoneyAmount()
        {
            return _accounts.Sum(account => account.AccountValue);
        }

        /// <summary>
        /// Возвращает кол-во денег у клиента. 
        /// Внутри происходит исключение, если у клиента нет аккаунта в банке
        /// </summary>
        public double GetAccountValue(long userId)
        {
            BankAccount account = GetAccountByUserId(userId);
            return account.AccountValue;
        }

        /// <summary>
        /// Перечисление денег между продавцом и покупателем
        /// </summary>
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

        public void CreateAccount(IExchangeUser user)
        {
            if (user is Bank)
            {
                throw new ArgumentException("НГельзя создать счет у банка повторно");
            }
            _accounts.Add(new BankAccount
            {
                UserId = user.Id,
                AccountValue = GetRandomMoney()
            });
        }

        /// <summary>
        /// Выплата процентов по депозитам
        /// </summary>
        public double PayoutDepositPercent()
        {
            double allPercents = 0;
            foreach (BankAccount account in _accounts)
            {
                double percent = account.AccountValue * DepositPercent;
                if (AllMoney < percent)
                {
                    // Случился дефолт системы
                    throw new BankMoneyDefaultException();
                }

                if (account.UserId == this.Id)
                {
                    // Чтобы банк самому себе не платил процент
                    continue;
                }

                allPercents += percent;
                AllMoney -= percent;
                account.AccountValue += percent;
            }
            return allPercents;
        }

        private BankAccount GetAccountByUserId(long userId)
        {
            var result = _accounts.SingleOrDefault(ac => ac.UserId == userId);
            return result 
                ?? throw new BankAccountDoesNotExistsException($"Отсутствует счет для Id = {userId}"); ;
        }
    }

    internal class BankAccount
    {
        public long UserId { get; set; }

        public double AccountValue { get; set; }
    }
}