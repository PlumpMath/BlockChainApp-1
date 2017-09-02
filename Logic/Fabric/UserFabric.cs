using Logic.Entitites;
using Logic.Interfaces;
using Utilities.Common;

namespace Logic.Fabric
{
    public class UserFabric : IUserFactory
    {
        public User GenerateEntity()
        {
            return new User(MiscUtils.GetRandomName());
        }
    }

    public interface IUserFactory : IFabric<User> { }
}