using Logic.DependencyInjector;
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
    }
}