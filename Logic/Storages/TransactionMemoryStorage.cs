using Logic.Finance;
using Logic.Interfaces;

namespace Logic.Storages
{
    public class TransactionMemoryStorage : EntityMemoryStorageBase<Transaction>, ITransactionStorage
    {
        
    }

    public interface ITransactionStorage : IEntityStorage<Transaction> { }
}