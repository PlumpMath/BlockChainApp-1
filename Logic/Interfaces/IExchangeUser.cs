using System.Collections.Generic;
using Logic.Finance;
using Logic.Helpers;
using Logic.Participants;

namespace Logic.Interfaces
{
    /// <summary>
    /// Участник биржи и торгов
    /// </summary>
    public interface IExchangeUser : IIdentificable
    {
        int OwnedShareCount { get; }
        /// <summary>
        /// Участник может не захотеть вести торги на этот раз
        /// </summary>
        bool WannaMakeDeals();

        /// <summary>
        /// Отчуждение акций, которые были куплены другим участником в результате сделки
        /// </summary>
        void DeattachShares(Deal deal);

        /// <summary>
        /// Прием акций, купленных в результате сделки
        /// </summary>
        void TakeShares(Deal deal);

        /// <summary>
        /// оповещаем участника, что его сделка не была осуществлена
        /// </summary>
        void NotifyAboutFiredSellOffer(SellDealOffer offer);

        /// <summary>
        /// Пользователь может понизить стоимость акций определенной компании, которые у него есть
        /// </summary>
        void DecreaseSharePriceIfWantTo(long companyId);

        /// <summary>
        /// Пользователь может повысить стоимость акций определенной компании, которые у него есть
        /// </summary>
        void IncreaseSharePriceIfWantTo(long companyId);

        double HowManyCouldSpendMoney();

        ICollection<Share> GetOwnedShares();

        ExchangeUserType GetExchangeUserType();

        BuyDealOffer GetBuyDealOffer();

        SellDealOffer GetSellDealOffer();


    }
}