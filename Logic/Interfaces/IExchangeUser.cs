namespace Logic.Interfaces
{
    public interface IExchangeUser : IIdentificable
    {
        double Wallet { get; }

        string Name { get; }
    }
}