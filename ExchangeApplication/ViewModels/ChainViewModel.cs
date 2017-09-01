using Logic.Entitites;
using Utilities.Common;

namespace ExchangeApplication.ViewModels
{
    public class ChainViewModel
    {
        public long Id { get; set; }

        public long SellerId { get; set; }

        public string SellerName { get; set; }

        public long BuyerId { get; set; }

        public string BuyerName { get; set; }

        public string TransactionComission { get; set; }

        public string TransactionValue { get; set; }

        public string PreviousHash { get; set; }

        public string CurrentHash { get; set; }

        public string CreatedAt { get; set; }

        public ChainViewModel(Chain chain)
        {
            Id = chain.Id;
            SellerId = chain.SellerId;
            BuyerId = chain.BuyerId;
            TransactionComission = MiscUtils.FormatDouble(chain.TransactionComission);
            TransactionValue = MiscUtils.FormatDouble(chain.TransactionValue);
            PreviousHash = chain.PreviousHash;
            CurrentHash = chain.CurrentHash;
            CreatedAt = chain.CreatedAt.ToLongDateString();
        }
    }

    
}