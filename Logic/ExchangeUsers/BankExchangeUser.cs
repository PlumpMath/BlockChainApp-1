using Logic.Helpers;
using Logic.Participants;

namespace Logic.ExchangeUsers
{
    public class BankExchangeUser : ExchangeUserBase
    {
        public override string UniqueExchangeId()
        {
            return nameof(BankExchangeUser).ToLowerInvariant() + this.Id;
        }

        public override ExchangeUserType ExchangeUserType => ExchangeUserType.CentralBank;
    }
}