using System;
using Logic.Interfaces;
using Utilities.Common;

namespace Logic.Entitites
{
    public class Chain : IIdentificable
    {
        public long Id { get; set; }

        public long SellerId { get; }

        public long BuyerId { get; }

        public string TransactionComment { get; }

        public double TransactionValue { get; }

        public string PreviousHash { get; set; }

        public DateTime CreatedAt { get; }

        public string CurrentHash { get; set; }

        public Chain(long sellerId, long buyerId, string transactionComment, double transactionValue)
        {
            Id = 0;
            SellerId = sellerId;
            BuyerId = buyerId;
            TransactionComment = transactionComment;
            TransactionValue = transactionValue;
            CreatedAt = DateTime.UtcNow;
            CurrentHash = null;
        }

        public void ComputeThisHash()
        {
            if (Id > 1 && PreviousHash == null)
            {
                throw new ArgumentNullException(nameof(PreviousHash));
            } 
            string text = $"{Id}{SellerId}{BuyerId}{TransactionComment}{TransactionValue}{PreviousHash}{CreatedAt}";
            CurrentHash = MiscUtils.HashEncode(text);
        }

        public override string ToString()
        {
            return $"Id {Id}. SellerId {SellerId}, BuyerId {BuyerId}. " +
                   $"TransactionValue {TransactionValue}. " +
                   $"Date {CreatedAt}\r\n" +
                   $"PrevHash {PreviousHash}\r\n" +
                   $"CurrHash {CurrentHash}\r\n";
        }
    }
}