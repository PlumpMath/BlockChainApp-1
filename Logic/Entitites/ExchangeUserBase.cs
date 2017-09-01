using System;
using Logic.Interfaces;

namespace Logic.Entitites
{
    public class ExchangeUserBase : IExchangeUser
    {
        public long Id { get; set; }

        public DateTime CreatedAt { get; }

        public double Wallet { get; set; }

        public string Name { get; set; }

        public ExchangeUserBase()
        {
            Id = 0;
            CreatedAt = DateTime.UtcNow;
        }

        public bool WithdrawMoney(double invoice)
        {
            if (Wallet < invoice)
            {
                return false;
            }
            Wallet -= invoice;
            return true;
        }

        public void TakeMoney(double invoice)
        {
            Wallet += invoice;
        }

        public override string ToString()
        {
            return $"{Name}. Account {Wallet}";
        }
    }
}