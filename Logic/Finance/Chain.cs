using System;
using Utilities.Common;

namespace Logic.Finance
{
    public sealed class Chain :  Transaction
    {
        public string PreviousHash { get; set; }

        public string CurrentHash { get; set; }

        public Chain(string senderUniqueId, string receiverUniqueId, double transactionValue, double transactionComission) 
            : base(senderUniqueId, receiverUniqueId, transactionValue, transactionComission)
        {
            Name = $"Чейн {Id}";
            CurrentHash = null;
        }

        public void ComputeThisHash()
        {
            if (Id > 1 && PreviousHash == null)
            {
                throw new ArgumentNullException(nameof(PreviousHash));
            } 
            string text = $"{Id}{SenderUniqueId}{ReceiverUniqueId}{TransactionComission}{TransactionValue}{PreviousHash}{CreatedAt}";
            CurrentHash = MiscUtils.HashEncode(text);
        }

        public static Chain CreateFromTransaction(Transaction transaction)
        {
            if (transaction is Chain)
            {
                // чтобы не возникло создания из самого себя
                throw new InvalidCastException($"Под ссылкой {nameof(Transaction)} передан объект {nameof(Chain)}");
            }
            return new Chain(transaction.SenderUniqueId, transaction.ReceiverUniqueId, transaction.TransactionValue, transaction.TransactionComission);
        }

        public override string UniqueExchangeId()
        {
            return nameof(Chain).ToLowerInvariant() + this.Id;
        }
    }
}