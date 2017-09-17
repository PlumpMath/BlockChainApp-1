using System.Collections.Generic;

namespace Logic.Finance
{
    public class Deal
    {
        /// <summary>
        /// Список акций
        /// </summary>
        public ICollection<Share> Shares { get; set; }

        /// <summary>
        /// Цена одной акции, по которой будет происходить сделка
        /// </summary>
        public double SharePrice { get; set; }

        /// <summary>
        /// Кол-во акций
        /// </summary>
        public int ShareCount { get; set; }

        /// <summary>
        /// Общая стоимость акций
        /// </summary>
        public double SharesCost { get; set; }

        /// <summary>
        /// Ауди компании-владельца акций
        /// </summary>
        public long ShareCompanyId { get; set; }
    }

    public class ConfirmedDeal
    {
        public string SellerUniqueId { get; set; }

        public string BuyerUniqueId { get; set; }

        public Deal Deal { get; set; }
    }
}