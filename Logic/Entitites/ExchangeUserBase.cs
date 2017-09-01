using System;
using Logic.DependencyInjector;
using Logic.Interfaces;

namespace Logic.Entitites
{
    public class ExchangeUserBase : IExchangeUser
    {
        public long Id { get; set; }

        public DateTime CreatedAt { get; }

        public double Wallet => DI.Get<IBank>().GetAccountValue(this);

        public string Name { get; set; }

        public ExchangeUserBase()
        {
            Id = 0;
            CreatedAt = DateTime.UtcNow;
        }

        public override string ToString()
        {
            return $"{Name}. Account {Wallet}";
        }
    }
}