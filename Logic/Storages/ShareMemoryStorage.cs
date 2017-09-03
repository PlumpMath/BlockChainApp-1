using System.Collections.Generic;
using System.Linq;
using Logic.Finance;
using Logic.Interfaces;

namespace Logic.Storages
{
    public interface IShareStorage : IEntityStorage<Share>
    {
        IEnumerable<Share> GetByCompanyId(long companyId);

        IEnumerable<Share> GetByOwnerId(long ownerId);
    }

    public class ShareMemoryStorage : EntityMemoryStorageBase<Share>, IShareStorage
    {
        public IEnumerable<Share> GetByCompanyId(long companyId)
        {
            return GetAll().Where(share => share.CompanyId == companyId);
        }

        public IEnumerable<Share> GetByOwnerId(long ownerId)
        {
            return GetAll().Where(share => share.OwnerId == ownerId);
        }
    }
}