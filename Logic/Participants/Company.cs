using System;
using System.Collections.Generic;
using System.Linq;
using Logic.DependencyInjector;
using Logic.ExchangeUsers;
using Logic.Finance;
using Logic.Interfaces;
using Logic.Storages;
using Utilities.Common;

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

        public ICollection<Share> GetCompanyShares()
        {
            return Injector.Get<IShareStorage>().GetByCompanyId(this.Id);
        }

        public int ShareCount => GetCompanyShares().Count();

        public double ShareBasePrice => GetCompanyShares().First().BasePrice;

        public double ShareCurrentPrice => GetCompanyShares().First().CurrentPrice;

        public string SharePriceChangingTrand()
        {
            return GetCompanyShares().First().PriceChangingTrand.GetEnumValueDescription();
        }

        public double CompanyCost => ShareCurrentPrice * ShareCount;

    }
}