﻿using System;
using Logic.Interfaces;

namespace Logic.Finance
{
    public class Share : IIdentificable
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public double CurrentPrice { get; set; }

        public double BasePrice { get; set; }

        public long CompanyId { get; set; }

        public long OwnerId { get; set; }
        
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