using System;

namespace Logic.Participants
{
    /// <summary>
    /// Объект, который может быть идентифицируем
    /// </summary>
    public interface IIdentificable
    {
        long Id { get; set;  }

        DateTime CreatedAt { get; }
    }
}