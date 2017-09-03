using System;
using Logic.Interfaces;

namespace Logic.Participants
{
    public class Company : IIdentificable, IHasExchangeUser
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public DateTime CreatedAt { get; }

        private CompanyExchangeUser _exchangeUser;

        public Company()
        {
            CreatedAt = DateTime.UtcNow;
        }

        public IExchangeUser GetExchangeUser()
        {
            if (_exchangeUser == null)
            {
                _exchangeUser = new CompanyExchangeUser
                {
                    Id = this.Id,
                    Name = this.Name
                };
            }

            return _exchangeUser;
        }

        public string UniqueExchangeId()
        {
            return nameof(Company).ToLowerInvariant() + this.Id;
        }

    }

    public class CompanyExchangeUser : ExchangeUserBase
    {
        public override string UniqueExchangeId()
        {
            return nameof(Company).ToLowerInvariant() + Id;
        }
    }
}