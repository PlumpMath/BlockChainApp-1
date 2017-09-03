namespace Logic.Interfaces
{
    /// <summary>
    /// Участник биржи и торгов
    /// </summary>
    public interface IExchangeUser : IIdentificable
    {
        /// <summary>
        /// Имя участника
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Участник может не захотеть вести торги на этот раз
        /// </summary>
        bool WannaMissTurn();
    }
}