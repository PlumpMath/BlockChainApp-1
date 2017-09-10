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
        bool WannaMissTurn();

        /// <summary>
        /// Хочет ли участник потратить деньги на акции
        /// </summary>
        bool WannaBuyShares(ShareInvoiceInfo invoice);

        /// <summary>
        /// Хочет ли участник продавать свои акции
        /// </summary>
        bool WannaSellShares(double offer, out ShareInvoiceInfo invoice); 

        /// <summary>
        /// Отчуждение акций, которые были куплены другим участником в результате сделки
        /// </summary>
        void DeattachShares(ShareInvoiceInfo invoice);

        /// <summary>
        /// Прием акций, купленных в результате сделки
        /// </summary>
        void TakeShares(ShareInvoiceInfo invoice);

        /// <summary>
        /// Пользователь может понизить стоимость акций определенной компании, которые у него есть
        /// </summary>
        void DecreaseSharePriceIfWantTo(long companyId);

        /// <summary>
        /// Пользователь может повысить стоимость акций определенной компании, которые у него есть
        /// </summary>
        void IncreaseSharePriceIfWantTo(long companyId);

        double MakeInvoiceOffer();

        ICollection<Share> GetOwnedShares();

        ExchangeUserType GetExchangeUserType();
    }
}