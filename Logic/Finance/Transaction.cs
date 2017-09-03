using System;
using Logic.Interfaces;
using Logic.Participants;

namespace Logic.Finance
{
    public class Transaction : IIdentificable
    {
        public long Id { get; set; }

        public virtual string Name { get; set; }

        public DateTime CreatedAt { get; }

        public long SellerId { get; }

        public long BuyerId { get; }

        public double TransactionComission { get; }

        public double TransactionValue { get; }

        public Transaction(long sellerId, long buyerId, double transactionValue, double transactionComission)
        {
            Id = 0;
            Name = $"Транзакция {Id}";
            SellerId = sellerId;
            BuyerId = buyerId;
            TransactionComission = transactionComission;
            TransactionValue = transactionValue;

            CreatedAt = DateTime.UtcNow;
        }

        public override string ToString() => Name;
    }
}