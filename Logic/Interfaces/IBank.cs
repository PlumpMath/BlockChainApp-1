namespace Logic.Interfaces
{
    public interface IBank
    {
        double CalculateComission(double invoice);

        void CreateAccount(IExchangeUser user, int seed = 0);

        void IncreaseAccountMoneyValue();

        double GetMoneyAmount();

        double GetAccountValue(IExchangeUser user);

        void PutMoneyToTheAccount(IExchangeUser user, double value);

        void WithdrawMoney(IExchangeUser user, double value);

        bool TransferMoney(IExchangeUser seller, IExchangeUser buyer, double invoice);
    }
}