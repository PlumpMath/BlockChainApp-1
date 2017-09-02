using Logic.Entitites;
using Utilities.Common;

namespace ExchangeApplication.ViewModels
{
    public class TransactionViewModel
    {
        public long Id { get; set; }

        public long SellerId { get; set; }

        public string SellerName { get; set; }

        public long BuyerId { get; set; }

        public string BuyerName { get; set; }

        public string TransactionComission { get; set; }

        public string TransactionValue { get; set; }

        public string CreatedAt { get; set; }

        // Для потомков класса
        public TransactionViewModel() { }

        public TransactionViewModel(Transaction transaction)
        {
            Id = transaction.Id;
            SellerId = transaction.SellerId;
            BuyerId = transaction.BuyerId;
            TransactionComission = MiscUtils.FormatDouble(transaction.TransactionComission);
            TransactionValue = MiscUtils.FormatDouble(transaction.TransactionValue);
            CreatedAt = transaction.CreatedAt.ToLongDateString();
        }
    }
}