using Logic.Entitites;
using Utilities.Common;

namespace ExchangeApplication.ViewModels
{
    public class ChainViewModel : TransactionViewModel
    {
        public string PreviousHash { get; set; }

        public string CurrentHash { get; set; }

        public ChainViewModel(Chain chain) : base()
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