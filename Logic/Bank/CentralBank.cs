using System;
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

        private static readonly double DepositPercentIndividual
            = ConfigurationManager.AppSettings["DepositPercentIndividual"].ParseAsDouble();

        private static readonly double DepositPercentCompanies
            = ConfigurationManager.AppSettings["DepositPercentCompanies"].ParseAsDouble();

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
                    AccountValue = StartCapital,
                    DepositPercent = 0,
                    TransactionComissionRate = 0
                }
            };
        }

        /// <summary>
        /// Возвращает рандомное кол-во денег для участника в разрешенном диапазоне 
        /// </summary>
        public double GetRandomMoney(IExchangeUser user)
        {
            bool isCompany = user.ExchangeUserType == ExchangeUserType.Company;
            double max = isCompany
                ? MaximumByHandsCompany
                : MaximumByHandsIndividual;
            double min = isCompany
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

        public IExchangeUser GetOwnerByBankAccount(string accountUniqueId)
        {
            var account = GetBankAccount(accountUniqueId);
            return account.ExchangeUser;
        }

        /// <summary>
        /// Перечисление денег между продавцом и покупателем. 
        /// Возвращает флаг, произошла ли сделка
        /// </summary>
        public void TransferMoney(IExchangeUser sender, IExchangeUser receiver, double invoice)
        {
            BankAccount senderAccount = sender.GetBankAccount();


            if (senderAccount.AccountValue < invoice)
            {
                // Если нет денег, то сделка не удалась
                return;
            }

            BankAccount receiverAccount = receiver.GetBankAccount();
            
            double comissionRate = senderAccount.TransactionComissionRate;
            double comission = invoice * comissionRate;

            // Перечисление комиссии в счет банка
            AllMoney += comission;

            receiverAccount.AccountValue += invoice;
            senderAccount.AccountValue -= invoice + comission;

            CreateTransaction(sender, receiver, invoice, comission);
        }

        public IExchangeUser GetExchangeUser() => _bankExchangeUser;

        public void CreateAccount(IExchangeUser user)
        {
            if (user.ExchangeUserType == ExchangeUserType.CentralBank)
            {
                throw new ArgumentException("Нельзя создать счет у банка повторно");
            }
            _accounts.Add(new BankAccount
            {
                UniqueUserId = user.UniqueExchangeId(),
                AccountValue = GetRandomMoney(user),
                ExchangeUser = user,
                DepositPercent = DefineDepositPercentRateBasedOnUser(user),
                TransactionComissionRate = DefineTransactionComissionRateBasedOnUser(user)
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
                double percent = account.AccountValue * account.DepositPercent;
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
                CreateTransaction(_bankExchangeUser, account.GetOwnerByBankAccount(), percent, 0);
            }
            return allPercents;
        }

        public void SetChainChangeListener(IObserver listener)
        {
            _observer = listener;
        }

        private void CreateTransaction(IExchangeUser sender, IExchangeUser receiver, double invoice, double comission)
        {
            string senderUniqueId = sender.UniqueExchangeId();
            string receiverUniqueId = receiver.UniqueExchangeId();

            var transaction = new Transaction(senderUniqueId, receiverUniqueId, invoice, comission);
            Injector.Get<ITransactionStorage>().Save(transaction);
            _observer?.Transaction(transaction);
        }

        private double DefineTransactionComissionRateBasedOnUser(IExchangeUser user)
        {
            double rate;
            switch (user.ExchangeUserType)
            {
                case ExchangeUserType.Individual:
                    rate = BankComissionForIndividual;
                    break;
                case ExchangeUserType.Company:
                    rate = BankComissionForCompanies;
                    break;
                case ExchangeUserType.CentralBank:
                    rate = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return rate;
        }

        private double DefineDepositPercentRateBasedOnUser(IExchangeUser user)
        {
            double rate;
            switch (user.ExchangeUserType)
            {
                case ExchangeUserType.Individual:
                    rate = DepositPercentIndividual;
                    break;
                case ExchangeUserType.Company:
                    rate = DepositPercentCompanies;
                    break;
                case ExchangeUserType.CentralBank:
                    rate = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return rate;
        }
    }
}