using System.Collections.Generic;

namespace Logic.Helpers
{
    public class OwnedShareRepoController
    {
        private readonly Dictionary<long, int> _companyIdSHareSellDenyCountDictironary;

        public OwnedShareRepoController()
        {
            _companyIdSHareSellDenyCountDictironary = new Dictionary<long, int>();
        }

        private class OwnedShareItem
        {
            public long Id { get; set; }
        }
    }

    
}