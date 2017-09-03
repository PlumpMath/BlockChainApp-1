using Logic.DependencyInjector;
using Logic.Entitites;
using Logic.Storages;
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

            IExchangeUserStorage storage = DI.Get<IExchangeUserStorage>();
            var sellerName = storage.GetEntity(SellerId).Name;
            var buyerName = storage.GetEntity(BuyerId).Name;
            SellerName = sellerName;
            BuyerName = buyerName;
            
            TransactionComission = MiscUtils.FormatDouble(chain.TransactionComission);
            TransactionValue = MiscUtils.FormatDouble(chain.TransactionValue);
            PreviousHash = chain.PreviousHash;
            CurrentHash = chain.CurrentHash;
            CreatedAt = chain.CreatedAt.ToLongDateString();
        }
    }

    
}