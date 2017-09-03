using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Logic.DependencyInjector;
using Logic.Entitites;
using Logic.Exceptions;
using Logic.Extensions;
using Logic.Finance;
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
    public class Bank : IBank
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

        private IObserver _observer;

        private readonly ITransactionStorage _transactionStorage;

        private readonly IExchangeUser _bankExchangeUser;

        // Всеми деньгами считается собственный счет банка
        public double AllMoney
        {
            get => GetBankAccount(_bankExchangeUser.Id).AccountValue;
            set => GetBankAccount(_bankExchangeUser.Id).AccountValue = value;
        }

        private readonly List<BankAccount> _accounts;

        public Bank()
        {
            
            _bankExchangeUser = new BankExchangeUser
            {
                Name = "Bank of Bitcoins",
                Id = long.MaxValue
            };

            // При создании открывается свой же собственный счет
            _accounts = new List<BankAccount>
            {
                new BankAccount
                {
                    UserId = _bankExchangeUser.Id,
                    AccountValue = StartCapital
                }
            };

            
            _transactionStorage = Injector.Get<ITransactionStorage>();
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
            return GetBankAccount(userId).AccountValue;
        }

        public BankAccount GetBankAccount(long userId)
        {
            return _accounts.SingleOrDefault(ac => ac.UserId == userId)
                   ?? throw new BankAccountDoesNotExistsException($"Отсутствует счет для Id = {userId}");
        }

        /// <summary>
        /// Перечисление денег между продавцом и покупателем. 
        /// Возвращает флаг, произошла ли сделка
        /// </summary>
        public void TransferMoney(IExchangeUser agent, IExchangeUser contrAgent, double value)
        {
            BankAccount buyerAccount = contrAgent.GetBankAccount();


            if (buyerAccount.AccountValue < value)
            {
                // Если нет денег, то сделка не удалась
                return;
            }

            BankAccount sellerAccount = agent.GetBankAccount();
            double comission = CalculateComission(value);

            sellerAccount.AccountValue += value - comission;
            buyerAccount.AccountValue -= value;

            CreateTransaction(agent.Id, contrAgent.Id, value, comission);
        }

        public IExchangeUser GetExchangeUser() => _bankExchangeUser;

        public double CalculateComission(double invoice)
        {
            var result = invoice * BankComission;
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

                if (account.UserId == _bankExchangeUser.Id)
                {
                    // Чтобы банк самому себе не платил процент
                    continue;
                }

                allPercents += percent;
                AllMoney -= percent;
                account.AccountValue += percent;

                // Должно быть оформление в виде транзакции
                CreateTransaction(_bankExchangeUser.Id, account.UserId, percent, 0);
            }
            return allPercents;
        }

        public void SetChainChangeListener(IObserver listener)
        {
            _observer = listener;
        }

        private void CreateTransaction(long sellerId, long buyerId, double invoice, double comission)
        {
            var transaction = new Transaction(sellerId, buyerId, invoice, comission);
            _transactionStorage.Save(transaction);
            _observer?.Transaction(transaction);
        }
    }
}