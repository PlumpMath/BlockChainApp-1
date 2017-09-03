using Logic.DependencyInjector;
using Logic.Finance;
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
            SenderUniqueId = chain.SenderUniqueId;
            ReceiverUniqueId = chain.ReceiverUniqueId;

            IExchangeUserStorage storage = Injector.Get<IExchangeUserStorage>();
            var sellerName = storage.GetEntity(SenderUniqueId).Name;
            var buyerName = storage.GetEntity(ReceiverUniqueId).Name;
            SenderName = sellerName;
            ReceiverName = buyerName;
            
            TransactionComission = MiscUtils.FormatDouble(chain.TransactionComission);
            TransactionValue = MiscUtils.FormatDouble(chain.TransactionValue);
            PreviousHash = chain.PreviousHash;
            CurrentHash = chain.CurrentHash;
            CreatedAt = chain.CreatedAt.ToLongDateString();
        }
    }

    
}