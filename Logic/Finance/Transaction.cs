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

        public string SenderUniqueId { get; }

        public string ReceiverUniqueId { get; }

        public double TransactionComission { get; }

        public double TransactionValue { get; }

        public Transaction(string senderUniqueId, string receiverUniqueId, double transactionValue, double transactionComission)
        {
            Id = 0;
            Name = $"Транзакция {Id}";
            SenderUniqueId = senderUniqueId;
            ReceiverUniqueId = receiverUniqueId;
            TransactionComission = transactionComission;
            TransactionValue = transactionValue;

            CreatedAt = DateTime.UtcNow;
        }

        public virtual string UniqueExchangeId()
        {
            return nameof(Transaction).ToLowerInvariant() + this.Id;
        }

        public override string ToString() => Name;
    }
}