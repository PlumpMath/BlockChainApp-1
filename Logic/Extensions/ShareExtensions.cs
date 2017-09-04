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
        public static IEnumerable<Share> GetSharesOfOneRandomCompany(this IEnumerable<Share> shares, out long outCompanyId)
        {
            IEnumerable<long> companyIds = shares
                .Select(share => share.CompanyId)
                .Distinct();
            int count = companyIds.Count();
            
            int randomIndex = MiscUtils.GetRandomNumber(count);
            long companyId = companyIds.ElementAt(randomIndex);
            outCompanyId = companyId;
            return shares.Where(share => share.CompanyId == companyId);
        }

        public static ShareInvoiceInfo GetRandomShareInvoiceInfo(this IEnumerable<Share> shares)
        {
            // Взятие акций одной компании
            shares = shares.GetSharesOfOneRandomCompany(out long companyId);
            int shareForSaleCount = shares.Count();

            // рандомное количество акций на продажу
            int count = MiscUtils.GetRandomNumber(shareForSaleCount, 1);
            shares = shares.Take(count);

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