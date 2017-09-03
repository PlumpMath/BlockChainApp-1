using Logic.Helpers;
using Logic.Participants;

namespace Logic.ExchangeUsers
{
    public class CompanyExchangeUser : ExchangeUserBase
    {
        public override string UniqueExchangeId()
        {
            return nameof(Company).ToLowerInvariant() + Id;
        }

        public override ExchangeUserType ExchangeUserType => ExchangeUserType.Company;
    }
}