using System.Collections.Generic;
using Logic.Helpers;

namespace Logic.Finance
{
    public class ShareInvoiceInfo
    {
        public ICollection<Share> Shares { get; set; }

        public long CompanyId;

        public int Count { get; set; }

        public double Cost { get; set; }

        public double Comission { get; set; }

        public SharePriceChangingType Trand { get; set; }
    }
}