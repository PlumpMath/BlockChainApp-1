using Logic.DependencyInjector;
using Logic.Finance;
using Logic.Storages;
using Utilities.Common;

namespace ExchangeApplication.ViewModels
{
    public class TransactionViewModel
    {
        public long Id { get; set; }

        public string SenderUniqueId { get; set; }

        public string SenderName { get; set; }

        public string ReceiverUniqueId { get; set; }

        public string ReceiverName { get; set; }

        public string TransactionComission { get; set; }

        public string TransactionValue { get; set; }

        public string CreatedAt { get; set; }

        // Для потомков класса
        public TransactionViewModel() { }

        public TransactionViewModel(Transaction transaction)
        {
            Id = transaction.Id;
            SenderUniqueId = transaction.SenderUniqueId;
            ReceiverUniqueId = transaction.ReceiverUniqueId;

            IExchangeUserStorage storage = Injector.Get<IExchangeUserStorage>();
            var senderName = storage.GetExchangeUserByUniqueId(SenderUniqueId).Name;
            var receiverName = storage.GetExchangeUserByUniqueId(ReceiverUniqueId).Name;

            SenderName = senderName;
            ReceiverName = receiverName;
            TransactionComission = MiscUtils.FormatDouble(transaction.TransactionComission);
            TransactionValue = MiscUtils.FormatDouble(transaction.TransactionValue);
            CreatedAt = transaction.CreatedAt.ToLongDateString();
        }
    }
}