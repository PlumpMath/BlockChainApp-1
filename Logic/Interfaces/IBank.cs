﻿namespace Logic.Interfaces
{
    public interface IBank
    {
        double CalculateComission(double invoice);

        void CreateAccount(IExchangeUser user, int seed = 0);

        void IncreaseAccountMoneyValue();

        double GetMoneyAmount();

        double GetAccountValue(long userId);

        void PutMoneyToTheAccount(long userId, double value);

        void WithdrawMoney(long userId, double value);

        bool TransferMoney(IExchangeUser seller, IExchangeUser buyer, double invoice);
    }
}