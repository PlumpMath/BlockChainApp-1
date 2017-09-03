using System;
using Logic.Entitites;
using Utilities.Common;

namespace Logic.Finance
{
    public class Chain :  Transaction
    {
        public string PreviousHash { get; set; }

        public string CurrentHash { get; set; }

        public Chain(long sellerId, long buyerId, double transactionValue, double transactionComission) 
            : base(sellerId, buyerId, transactionComission, transactionValue)
        {
            CurrentHash = null;
        }

        public void ComputeThisHash()
        {
            if (Id > 1 && PreviousHash == null)
            {
                throw new ArgumentNullException(nameof(PreviousHash));
            } 
            string text = $"{Id}{SellerId}{BuyerId}{TransactionComission}{TransactionValue}{PreviousHash}{CreatedAt}";
            CurrentHash = MiscUtils.HashEncode(text);
        }

        public override string ToString()
        {
            return $"Чейн {Id}";
        }

        public static Chain CreateFromTransaction(Transaction transaction)
        {
            if (transaction is Chain)
            {
                // чтобы не возникло создания из самого себя
                throw new InvalidCastException($"Под ссылкой {nameof(Transaction)} передан объект {nameof(Chain)}");
            }
            return new Chain(transaction.SellerId, transaction.BuyerId, transaction.TransactionValue, transaction.TransactionComission);
        }
    }
}