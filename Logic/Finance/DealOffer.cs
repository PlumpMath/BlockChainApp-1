using System;
using System.Collections.Generic;
using Logic.Interfaces;

namespace Logic.Finance
{
    public class DealOffer
    {
        /// <summary>
        /// Тип сделки
        /// </summary>
        public virtual DealOfferType OfferType => DealOfferType.Unknown;

        /// <summary>
        /// Уникальное имя владельца
        /// </summary>
        public string UniqueExhcangeUserId { get; set; }

        public Deal Deal { get; set; }
    }

    public class BuyDealOffer : DealOffer
    {
        public override DealOfferType OfferType => DealOfferType.Buy;

        public BuyDealOffer() { }

        public BuyDealOffer(DealOffer offer) : base()
        {
            UniqueExhcangeUserId = offer.UniqueExhcangeUserId;
            Deal = offer.Deal;
        }
    }

    public class SellDealOffer : DealOffer
    {
        public override DealOfferType OfferType => DealOfferType.Sell;

        public SellDealOffer() { }

        public SellDealOffer(DealOffer offer) : base()
        {
            UniqueExhcangeUserId = offer.UniqueExhcangeUserId;
            Deal = offer.Deal;
        }
    }
}