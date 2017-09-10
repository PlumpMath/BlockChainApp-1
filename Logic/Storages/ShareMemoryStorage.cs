using System;
using System.Collections.Generic;
using System.Linq;
using Logic.DependencyInjector;
using Logic.Finance;
using Logic.Helpers;
using Logic.Interfaces;

namespace Logic.Storages
{
    public interface IShareStorage : IEntityStorage<Share>
    {
        ICollection<Share> GetByCompanyId(long companyId);

        ICollection<Share> GetByOwnerId(string ownerUniqueId);

        void IncreaseShareCurrentPrice(long companyId, double additionRate);

        void DecreaseShareCurrentPrice(long companyId, double submissionRate);

        void ChangeShareCurrentPrice(long companyId, double changeRate, SharePriceChangingType changingType);
    }

    public class ShareMemoryStorage : EntityMemoryStorageBase<Share>, IShareStorage
    {
        public ICollection<Share> GetByCompanyId(long companyId)
        {
            return GetAll().Where(share => share.CompanyId == companyId).ToArray();
        }

        public ICollection<Share> GetByOwnerId(string ownerUniqueId)
        {
            return GetAll().Where(share => share.OwnerUniqueId == ownerUniqueId).ToArray();
        }

        public void IncreaseShareCurrentPrice(long companyId, double additionRate)
        {
            ChangeShareCurrentPrice(companyId, additionRate, SharePriceChangingType.Increasing);
        }

        public void DecreaseShareCurrentPrice(long companyId, double submissionRate)
        {
            ChangeShareCurrentPrice(companyId, submissionRate, SharePriceChangingType.Decreasing);
        }

        public void ChangeShareCurrentPrice(long companyId, double changeRate, SharePriceChangingType changingType)
        {
            var shares = GetByCompanyId(companyId);
            foreach (Share share in shares)
            {
                share.PriceChangingTrand = changingType;
                double diff = share.CurrentPrice * changeRate;
                switch (changingType)
                {
                    case SharePriceChangingType.Fixed:
                        continue;

                    case SharePriceChangingType.Increasing:
                        share.CurrentPrice += diff;
                        break;

                    case SharePriceChangingType.Decreasing:
                        share.CurrentPrice -= diff;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(changingType));
                }
            }
            Save(shares);
        }
    }
}