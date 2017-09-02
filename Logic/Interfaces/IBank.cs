namespace Logic.Interfaces
{
    public interface IBank
    {
        double CalculateComission(double invoice);

        void CreateAccount(IExchangeUser user);

        double PayoutDepositPercent();

        double GetMoneyAmount();

        double GetAccountValue(long userId);

        bool TransferMoney(IExchangeUser seller, IExchangeUser buyer, double invoice, out double comission);
    }
}