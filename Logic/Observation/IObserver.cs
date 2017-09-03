using System;
using Logic.Entitites;
using Logic.Finance;

namespace Logic.Observation
{
    public interface IObserver
    {
        void CommonMessage(string message);

        void Transaction(Transaction transaction);

        void Exception(Exception exception);
    }
}