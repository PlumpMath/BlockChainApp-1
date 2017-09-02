using Logic.DependencyInjector;
using Logic.Entitites;
using Logic.Interfaces;

namespace Logic.Extensions
{
    public static class BankExtensions
    {
        public static double GetBankAccountValue(this IExchangeUser user)
        {
            IBank bank = DI.Get<IBank>();
            return bank.GetAccountValue(user.Id);
        }

        public static BankAccount GetBankAccount(this IExchangeUser user)
        {
            IBank bank = DI.Get<IBank>();
            return bank.GetBankAccount(user.Id);
        }
    }
}