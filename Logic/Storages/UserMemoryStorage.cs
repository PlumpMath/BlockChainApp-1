using Logic.Entitites;
using Logic.Interfaces;

namespace Logic.Storages
{
    public class UserMemoryStorage : EntityMemoryStorageBase<User>, IUserStorage
    {
        
    }

    public interface IUserStorage : IEntityStorage<User> { }
}