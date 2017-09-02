using System;

namespace Logic.Exceptions
{
    /// <summary>
    /// Банк обанкротился, средств для выплат нет
    /// </summary>
    public class BankMoneyDefaultException : Exception { }

    /// <summary>
    /// Аккаунта в банке не существует
    /// </summary>
    public class BankAccountDoesNotExistsException : Exception
    {
        public BankAccountDoesNotExistsException() : base() { }

        public BankAccountDoesNotExistsException(string message) : base(message) { }
    }
}