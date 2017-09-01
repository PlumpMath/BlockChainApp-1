using Dal.Dal.Parent;
using Dal.Entitites;
using Dal.Interfaces;

namespace Dal.Dal
{
    public class ChainRepository : BaseRepository<Chain>
    {
        public ChainRepository(IDbContext context) : base(context)
        {
        }
    }
}