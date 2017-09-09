using System;
using Logic.Helpers;
using Logic.Interfaces;

namespace Logic.Finance
{
    public class Share : IIdentificable
    {
        public long Id { get; set; }

        public string Name { get; set; }

        private double _currentprice = 1;
        public double CurrentPrice
        {
            get => _currentprice;
            set
            {
                if (value <= 1)
                {
                    // Минимальная цена акции не может быть ниже 1
                    value = 1;
                }
                _currentprice = value;
            }
        }

        public double BasePrice { get; set; }

        public SharePriceChangingType PriceChangingTrand { get; set; }

        public long CompanyId { get; set; }

        public string OwnerUniqueId { get; set; }
        
        public DateTime CreatedAt { get; }

        public Share()
        {
            CreatedAt = DateTime.UtcNow;
        }

        public Share(double basePrice)
        {
            BasePrice = basePrice;
            CurrentPrice = basePrice;
            CreatedAt = DateTime.UtcNow;
        }

        public string UniqueExchangeId()
        {
            return nameof(Share).ToLowerInvariant() + this.Id;
        }


    }
}