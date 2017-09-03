using Logic.Interfaces;

namespace Logic.Bank
{
    public class BankAccount
    {
        public string UniqueUserId { get; set; }

        public double AccountValue { get; set; }

        public IExchangeUser ExchangeUser { get; set; }
    }
}