using System;
using System.Collections.Generic;
using System.Linq;
using Logic.Finance;
using Logic.Helpers;
using Utilities.Common;

namespace Logic.Extensions
{
    public static class ShareExtensions
    {
        public static ICollection<Share> GetSharesOfOneRandomCompany(this ICollection<Share> shares, out long outCompanyId)
        {
            IEnumerable<long> companyIds = shares
                .Select(share => share.CompanyId)
                .Distinct();
            int count = companyIds.Count();
            
            int randomIndex = MiscUtils.GetRandomNumber(count);
            long companyId = companyIds.ElementAt(randomIndex);
            outCompanyId = companyId;
            return shares.Where(share => share.CompanyId == companyId).ToArray();
        }

        public static ShareInvoiceInfo GetRandomShareInvoiceInfo(this ICollection<Share> shares, double invoiceOffer)
        {
            // Взятие акций одной компании
            shares = shares.GetSharesOfOneRandomCompany(out long companyId);
            int shareForSaleCount = shares.Count();
            double currentPrice = shares.First().CurrentPrice;

            double allSharesCost = currentPrice * shareForSaleCount;

            if (allSharesCost > invoiceOffer)
            {
                int couldBuyCount = (int)Math.Floor(invoiceOffer / currentPrice);

                if (couldBuyCount == 0)
                {
                    return null;
                }
                shares = shares.Take(couldBuyCount).ToArray();
            }        

            // вычисление общей стоимости акций на основе текущей цены
            double invoice = shares.GetSharesCost();

            var shareInvoiceInfo = new ShareInvoiceInfo
            {
                Shares = shares,
                CompanyId = companyId,
                Count = shareForSaleCount,
                Cost = invoice,
                Trand = shares.First().PriceChangingTrand
            };

            return shareInvoiceInfo;
        }

        public static double GetSharesCost(this IEnumerable<Share> shares)
        {
            return shares.Count() * shares.First().CurrentPrice;
        }
    }

    public static class ShareHelpers
    {
        public static SharePriceChangingType GetPriceChangingTypeByRandom()
        {
            IEnumerable<SharePriceChangingType> values = Enum
                .GetValues(typeof(SharePriceChangingType))
                .OfType<SharePriceChangingType>();

            int index = MiscUtils.GetRandomNumber(values.Count() - 1);
            return values.ElementAt(index);
        }
    }
}