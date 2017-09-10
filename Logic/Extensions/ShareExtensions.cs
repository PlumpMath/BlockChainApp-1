using System;
using System.Collections.Generic;
using System.Linq;
using Logic.Bank;
using Logic.DependencyInjector;
using Logic.Finance;
using Logic.Helpers;
using Logic.Interfaces;
using Logic.Participants;
using Utilities.Common;

namespace Logic.Extensions
{
    public static class ShareExtensions
    {
        public static double GetBankAccountValue(this IExchangeUser user)
        {
            return GetBankAccount(user).AccountValue;
        }

        public static bool GotEnoughMoney(this IExchangeUser user, double invoice)
        {
            return GetBankAccount(user).AccountValue >= invoice;
        }

        public static BankAccount GetBankAccount(this IExchangeUser user)
        {
            return Injector.Get<IBank>().GetBankAccount(user.UniqueExchangeId());
        }

        public static IExchangeUser GetOwnerByBankAccount(this BankAccount account)
        {
            return Injector.Get<IBank>().GetOwnerByBankAccount(account.UniqueUserId);
        }

        public static double GetSharesCost(this IEnumerable<Share> shares)
        {
            return !shares.Any() 
                ? 0
                : shares.Count() * shares.First().CurrentPrice;
        }

        public static double CalculatePriceChangePercent(this Company company)
        {
            double currentPrice = company.ShareCurrentPrice,
                basePrice = company.ShareBasePrice;

            double percent = Math.Abs(currentPrice - basePrice) < 0.01
                ? 0
                : ((currentPrice - basePrice) / basePrice) * 100;

            return percent;
        }

        public static Company GetCompanyWithMaxGrow(this ICollection<Company> companies)
        {
            double maxGrowPercent = companies.Max(c => c.CalculatePriceChangePercent());
            return companies.First(c => Math.Abs(c.CalculatePriceChangePercent() - maxGrowPercent) < 0.001);
        }

        public static Company GetCompanyWithMinimalGrow(this ICollection<Company> companies)
        {
            double maxGrowPercent = companies.Min(c => c.CalculatePriceChangePercent());
            return companies.First(c => Math.Abs(c.CalculatePriceChangePercent() - maxGrowPercent) < 0.001);
        }
    }
}