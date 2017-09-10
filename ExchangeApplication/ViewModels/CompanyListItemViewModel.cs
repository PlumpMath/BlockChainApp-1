using System;
using Logic.Extensions;
using Logic.Participants;
using Utilities.Common;

namespace ExchangeApplication.ViewModels
{
    public class CompanyListItemViewModel
    {
        public string Name { get; set; }

        public string ShareBasePrice { get; set; }

        public string ShareCurrentPrice { get; set; }

        public int ShareCount { get; set; }

        public string CompanyCost { get; set; }

        public string PriceChangeTrand { get; set; }

        public string PriceChangeInPercents { get; set; }

        public CompanyListItemViewModel(Company company)
        {
            Name = company.Name;
            ShareCount = company.ShareCount;

            double currentPrice = company.ShareCurrentPrice;
            double basePrice = company.ShareBasePrice;

            ShareBasePrice = basePrice.FormatDouble();
            ShareCurrentPrice = currentPrice.FormatDouble();
            CompanyCost = company.CompanyCost.FormatDouble();
            PriceChangeTrand = company.SharePriceChangingTrand();

            PriceChangeInPercents = company.CalculatePriceChangePercent().FormatDouble();
        }

        
    }
}