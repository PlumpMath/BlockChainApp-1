using System.Collections.Generic;
using Logic.Helpers;

namespace Logic.Finance
{
    public class ShareInvoiceInfo
    {
        public IEnumerable<Share> Shares { get; set; }

        public long CompanyId;

        public int Count { get; set; }

        public double Cost { get; set; }

        public SharePriceChangingType Trand { get; set; }
    }
}