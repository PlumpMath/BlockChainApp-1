using System;
using Logic.Interfaces;
using Logic.Participants;

namespace Logic.Bank
{
    public class BankExchangeUser : ExchangeUserBase
    {
        public override string UniqueExchangeId()
        {
            return nameof(BankExchangeUser).ToLowerInvariant() + this.Id;
        }
    }
}