using System;

namespace Logic.Exceptions
{
    public class NotExchangeUserOwnedSharesException : Exception
    {
        public NotExchangeUserOwnedSharesException() : base()
        {
        }

        public NotExchangeUserOwnedSharesException(string message) : base(message)
        {
        }
    }
}