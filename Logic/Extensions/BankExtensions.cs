using Logic.Bank;
using Logic.DependencyInjector;
using Logic.Interfaces;
using Logic.Participants;

namespace Logic.Extensions
{
    public static class BankExtensions
    {
        public static double GetBankAccountValue(this IExchangeUser user)
        {
            return Injector.Get<IBank>().GetAccountValue(user.UniqueExchangeId());
        }

        public static BankAccount GetBankAccount(this IExchangeUser user)
        {
            return Injector.Get<IBank>().GetBankAccount(user.UniqueExchangeId());
        }
    }
}