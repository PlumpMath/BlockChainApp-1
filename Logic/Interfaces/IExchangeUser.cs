namespace Logic.Interfaces
{
    public interface IExchangeUser : IIdentificable
    {
        double Wallet { get; set; }

        string Name { get; }

        bool WithdrawMoney(double invoice);

        void TakeMoney(double invoice);
    }
}