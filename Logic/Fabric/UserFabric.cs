using Logic.Entitites;
using Logic.Interfaces;
using Utilities.Common;

namespace Logic.Fabric
{
    public class UserFabric : IUserFactory
    {
        public User GenerateEntity(int seed = 0)
        {
            return new User(MiscUtils.GetRandomName(seed));
        }
    }

    public interface IUserFactory : IFabric<User> { }
}