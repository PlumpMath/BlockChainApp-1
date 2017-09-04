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
        /// <summary>
        /// Участник может не захотеть вести торги на этот раз
        /// </summary>
        bool WannaMissTurn();

        /// <summary>
        /// Хочет ли участник потратить деньги на акции
        /// </summary>
        bool WannaBuyShares(ShareInvoiceInfo invoice);

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

        /// <summary>
        /// В завимости от рандома пользователь может или повысить, или понизить стоимость
        /// </summary>
        void ChangeSharePriceIfWantTo(long companyId);

        IEnumerable<Share> GetOwnedShares();

        ExchangeUserType GetExchangeUserType();
    }
}