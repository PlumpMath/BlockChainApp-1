using System;

namespace Logic.Interfaces
{
    /// <summary>
    /// Объект, который может быть идентифицируем
    /// </summary>
    public interface IIdentificable : IId
    {
        string UniqueExchangeId();

        /// <summary>
        /// Имя участника
        /// </summary>
        string Name { get; set; }

        DateTime CreatedAt { get; }
    }

    public interface IId
    {
        long Id { get; set; }
    }
}