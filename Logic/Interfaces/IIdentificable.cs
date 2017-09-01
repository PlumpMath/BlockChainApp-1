using System;

namespace Logic.Interfaces
{
    public interface IIdentificable
    {
        long Id { get; set;  }

        DateTime CreatedAt { get; }
    }
}