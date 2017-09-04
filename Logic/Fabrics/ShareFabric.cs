using System.Collections.Generic;
using Logic.Finance;

namespace Logic.Fabrics
{
    public interface IShareFabric : IFabricBase<Share>
    {
        IEnumerable<Share> GetEntitiesOfCompany(long companyId, string companyUniqueId, int count, double price);
    }

    public class ShareFabric : FabricBase<Share>, IShareFabric
    {
        protected override bool UniqueNames { get; set; } = false;

        public IEnumerable<Share> GetEntitiesOfCompany(long companyId, string companyUniqueId, int count, double price)
        {
            IEnumerable<Share> shares = base.GetEntities(count);
            foreach (Share share in shares)
            {
                share.CompanyId = companyId;
                share.OwnerUniqueId = companyUniqueId;
                share.BasePrice = price;
                share.CurrentPrice = price;
            }
            return shares;
        }

        protected override IEnumerable<string> EntityNames { get; set; } = new string[]{"Акция"};
    }
}