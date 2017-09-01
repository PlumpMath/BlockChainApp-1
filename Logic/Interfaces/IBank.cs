using Logic.Entitites;

namespace Logic.Interfaces
{
    public interface IBank
    {
        double CalculateComission(double invoice);

        void CreateAccount(IExchangeUser user);

        void IncreaseAccountMoneyValue();
    }
}