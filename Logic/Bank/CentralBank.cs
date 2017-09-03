﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Logic.DependencyInjector;
using Logic.Exceptions;
using Logic.ExchangeUsers;
using Logic.Extensions;
using Logic.Finance;
using Logic.Helpers;
using Logic.Interfaces;
using Logic.Observation;
using Logic.Participants;
using Logic.Storages;
using Utilities.Common;
using Utilities.Convert;

namespace Logic.Bank
{
    /// <summary>
    /// Банк может вести себя и как участник торгов
    /// </summary>
    public class CentralBank : IBank
    {
        private static readonly double StartCapital 
            = ConfigurationManager.AppSettings["StartCapital"].ParseAsDouble();

        private static readonly double BankComissionForIndividual 
            = ConfigurationManager.AppSettings["BankComissionForIndividual"].ParseAsDouble();

        private static readonly double BankComissionForCompanies
            = ConfigurationManager.AppSettings["BankComissionForCompanies"].ParseAsDouble();

        private static readonly double DepositPercent
            = ConfigurationManager.AppSettings["DepositPercent"].ParseAsDouble();

        private static readonly double MaximumByHandsIndividual
            = ConfigurationManager.AppSettings["MaximumByHandsIndividual"].ParseAsDouble();

        private static readonly double MinimumByHandsIndividual
            = ConfigurationManager.AppSettings["MinimumByHandsIndividual"].ParseAsDouble();

        private static readonly double MaximumByHandsCompany
            = ConfigurationManager.AppSettings["MaximumByHandsCompany"].ParseAsDouble();

        private static readonly double MinimumByHandsCompany
            = ConfigurationManager.AppSettings["MinimumByHandsCompany"].ParseAsDouble();

        private IObserver _observer;

        private readonly IExchangeUser _bankExchangeUser;

        // Всеми деньгами считается собственный счет банка
        public double AllMoney
        {
            get => GetBankAccount(_bankExchangeUser.UniqueExchangeId()).AccountValue;
            set => GetBankAccount(_bankExchangeUser.UniqueExchangeId()).AccountValue = value;
        }

        private readonly List<BankAccount> _accounts;

        public CentralBank()
        {
            _bankExchangeUser = new BankExchangeUser
            {
                Id = long.MaxValue,
                Name = "Bank of Bitcoins"
            };

            // При создании открывается свой же собственный счет
            _accounts = new List<BankAccount>
            {
                new BankAccount
                {
                    UniqueUserId = _bankExchangeUser.UniqueExchangeId(),
                    AccountValue = StartCapital
                }
            };
        }

        /// <summary>
        /// Возвращает рандомное кол-во денег для участника в разрешенном диапазоне 
        /// </summary>
        public double GetRandomMoney(IExchangeUser user)
        {
            double max = user is Company
                ? MaximumByHandsCompany
                : MaximumByHandsIndividual;
            double min = user is Company
                ? MinimumByHandsCompany
                : MinimumByHandsIndividual;

            double amount = MiscUtils.GetRandomNumber(max, min);
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
        public double GetAccountValue(string uniqueUserId)
        {
            return GetBankAccount(uniqueUserId).AccountValue;
        }

        public BankAccount GetBankAccount(string uniqueUserId)
        {
            return _accounts.SingleOrDefault(ac => ac.UniqueUserId == uniqueUserId)
                   ?? throw new BankAccountDoesNotExistsException($"Отсутствует счет для UniqueUserId = {uniqueUserId}");
        }

        /// <summary>
        /// Перечисление денег между продавцом и покупателем. 
        /// Возвращает флаг, произошла ли сделка
        /// </summary>
        public void TransferMoney(IExchangeUser sender, IExchangeUser receiver, double value)
        {
            BankAccount senderAccount = sender.GetBankAccount();


            if (senderAccount.AccountValue < value)
            {
                // Если нет денег, то сделка не удалась
                return;
            }

            BankAccount receiverAccount = receiver.GetBankAccount();
            
            double comission = CalculateComission(value, sender);

            receiverAccount.AccountValue += value;
            senderAccount.AccountValue -= value + comission;

            CreateTransaction(receiver.UniqueExchangeId(), sender.UniqueExchangeId(), value, comission);
        }

        public IExchangeUser GetExchangeUser() => _bankExchangeUser;

        private double CalculateComission(double invoice, IExchangeUser sender)
        {
            double comissionRate;
            switch (sender.ExchangeUserType)
            {
                case ExchangeUserType.Individual:
                    comissionRate = BankComissionForIndividual;
                    break;
                case ExchangeUserType.Company:
                    comissionRate = BankComissionForCompanies;
                    break;
                case ExchangeUserType.CentralBank:
                    comissionRate = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            var result = invoice * comissionRate;
            AllMoney += result;
            return result;
        }

        public void CreateAccount(IExchangeUser user)
        {
            if (user is BankExchangeUser)
            {
                throw new ArgumentException("Нельзя создать счет у банка повторно");
            }
            _accounts.Add(new BankAccount
            {
                UniqueUserId = user.UniqueExchangeId(),
                AccountValue = GetRandomMoney(user),
                ExchangeUser = user
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

                if (account.UniqueUserId == _bankExchangeUser.UniqueExchangeId())
                {
                    // Чтобы банк самому себе не платил процент
                    continue;
                }

                allPercents += percent;
                AllMoney -= percent;
                account.AccountValue += percent;

                // Должно быть оформление в виде транзакции
                CreateTransaction(account.UniqueUserId, _bankExchangeUser.UniqueExchangeId(), percent, 0);
            }
            return allPercents;
        }

        public void SetChainChangeListener(IObserver listener)
        {
            _observer = listener;
        }

        private void CreateTransaction(string senderUniqueId, string receiverUniqueId, double invoice, double comission)
        {
            var transaction = new Transaction(senderUniqueId, receiverUniqueId, invoice, comission);
            Injector.Get<ITransactionStorage>().Save(transaction);
            _observer?.Transaction(transaction);
        }
    }
}