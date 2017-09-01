using Logic.Entitites;
using Utilities.Common;

namespace ExchangeApplication.ViewModels
{
    public class ChainViewModel
    {
        public long SellerId { get; set; }

        public long BuyerId { get; set; }

        public string TransactionComission { get; set; }

        public string TransactionValue { get; set; }

        public ChainViewModel(Chain chain)
        {
            SellerId = chain.SellerId;
            BuyerId = chain.BuyerId;
            TransactionComission = MiscUtils.FormatDouble(chain.TransactionComission);
            TransactionValue = MiscUtils.FormatDouble(chain.TransactionValue);
        }
    }

    
}