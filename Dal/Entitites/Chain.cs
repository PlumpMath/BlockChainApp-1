using System;
using System.ComponentModel.DataAnnotations;
using Dal.Entitites.Parent;
using Utilities.Common;
using System.Data.SQLite;

namespace Dal.Entitites
{
    public class Chain : DbEntity
    {
        [Required]
        public long SellerId { get; }

        [Required]
        public long BuyerId { get; }

        public string TransactionComment { get; }

        [Required]
        public double TransactionValue { get; }

        [Required]
        public string PreviousHash { get; }

        public DateTime CreatedAt { get; }

        public string CurrentHash { get; }

        public Chain(long id, long sellerId, long buyerId, string transactionComment, double transactionValue, string previousHash)
        {
            Id = id;
            SellerId = sellerId;
            BuyerId = buyerId;
            TransactionComment = transactionComment;
            TransactionValue = transactionValue;
            PreviousHash = previousHash;
            CreatedAt = DateTime.UtcNow;
            CurrentHash = ComputeThisHash();
        }

        private string ComputeThisHash()
        {
            string text = $"{Id}{SellerId}{BuyerId}{TransactionComment}{TransactionValue}{PreviousHash}{CreatedAt}";
            return MiscUtils.HashEncode(text);
        }
    }
}