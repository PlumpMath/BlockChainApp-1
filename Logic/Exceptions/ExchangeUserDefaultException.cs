using System;
using Logic.Interfaces;

namespace Logic.Exceptions
{
    /// <summary>
    /// Сигнализирует, что пользователь обанкротился. 
    /// </summary>
    public class ExchangeUserDefaultException : Exception
    {
        public IExchangeUser User { get; }

        public ExchangeUserDefaultException(IExchangeUser user)
        {
            User = user;
        }
    }
}