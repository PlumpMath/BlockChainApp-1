using System;
using Logic.Interfaces;

namespace Logic.Entitites
{
    public class Transaction : IIdentificable
    {
        public long Id { get; set; }

        public DateTime CreatedAt { get; }

        public long SellerId { get; }

        public long BuyerId { get; }

        public double TransactionComission { get; }

        public double TransactionValue { get; }

        public Transaction(long sellerId, long buyerId, double transactionValue, double transactionComission)
        {
            Id = 0;
            SellerId = sellerId;
            BuyerId = buyerId;
            TransactionComission = transactionComission;
            TransactionValue = transactionValue;

            CreatedAt = DateTime.UtcNow;
        }

        public override string ToString()
        {
            return $"Транзакция {Id}";
        }
    }
}