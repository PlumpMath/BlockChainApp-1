﻿using System;

namespace Logic.Interfaces
{
    /// <summary>
    /// Объект, который может быть идентифицируем
    /// </summary>
    public interface IIdentificable
    {
        long Id { get; set;  }

        string UniqueExchangeId();

        /// <summary>
        /// Имя участника
        /// </summary>
        string Name { get; set; }

        DateTime CreatedAt { get; }
    }
}