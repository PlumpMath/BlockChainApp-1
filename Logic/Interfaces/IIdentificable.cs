using System;

namespace Logic.Interfaces
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