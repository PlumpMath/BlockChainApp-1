using Logic.Bank;
using Logic.DependencyInjector;
using Logic.Interfaces;

namespace Logic.Extensions
{
    public static class BankExtensions
    {
        public static double GetBankAccountValue(this IExchangeUser user)
        {
            return GetBankAccount(user).AccountValue;
        }

        public static BankAccount GetBankAccount(this IExchangeUser user)
        {
            return Injector.Get<IBank>().GetBankAccount(user.UniqueExchangeId());
        }

        public static IExchangeUser GetOwnerByBankAccount(this BankAccount account)
        {
            return Injector.Get<IBank>().GetOwnerByBankAccount(account.UniqueUserId);
        }
    }
}