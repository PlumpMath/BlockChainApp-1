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

        ExchangeUserType ExchangeUserType { get; }
    }
}