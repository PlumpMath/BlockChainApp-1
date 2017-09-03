using Logic.Interfaces;
using Logic.Participants;

namespace Logic.Storages
{
    public interface ICompanyStorage : IEntityStorage<Company> { }

    public class CompanyMemoryStorage : EntityMemoryStorageBase<Company>, ICompanyStorage
    {
        
    }
}