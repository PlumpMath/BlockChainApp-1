using Logic.Helpers;
using Logic.Participants;

namespace Logic.ExchangeUsers
{
    public class IndividualUser : ExchangeUserBase
    {
        public IndividualUser() { }

        public IndividualUser(string name) :base()
        {
            Name = name;
        }

        public override string UniqueExchangeId()
        {
            return nameof(IndividualUser).ToLowerInvariant() + Id;
        }

        public override ExchangeUserType ExchangeUserType => ExchangeUserType.Individual;
    }
}