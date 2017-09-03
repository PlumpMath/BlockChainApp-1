using System;
using System.Collections.Generic;
using System.Linq;
using Logic.DependencyInjector;
using Logic.ExchangeUsers;
using Logic.Finance;
using Logic.Interfaces;
using Logic.Storages;

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

        public IEnumerable<Share> GetCompanyShares()
        {
            return Injector.Get<IShareStorage>().GetByCompanyId(this.Id);
        }

        public int GetCompanyShareCount()
        {
            return GetCompanyShares().Count();
        }

        public double GetCompanyShareBasePrice()
        {
            return GetCompanyShares().First().BasePrice;
        }

        public double GetCompanyShareCurrentPrice()
        {
            return GetCompanyShares().First().BasePrice;
        }

        public double GetCompanyCost()
        {
            int count = GetCompanyShareCount();
            double price = GetCompanyShareCurrentPrice();
            return price * count;
        }

    }
}