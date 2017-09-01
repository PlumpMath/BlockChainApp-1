using Logic.Entitites;

namespace Logic.Interfaces
{
    public interface IExchangeEventListener
    {
        void CommonMessage(string message);

        void Transaction(Chain chain);
    }
}