using System;
using Logic.Entitites;

namespace Logic.Interfaces
{
    public interface IObserver
    {
        void CommonMessage(string message);

        void Transaction(Transaction transaction);

        void Exception(Exception exception);
    }
}