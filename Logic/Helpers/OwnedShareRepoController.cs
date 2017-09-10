using System.Collections.Generic;
using Logic.Interfaces;
using Logic.Storages;

namespace Logic.Helpers
{
    public class OwnedShareRepoController
    {
        private readonly OwnedShareitemEntityStorage _ownedShareitemEntityStorage;

        public OwnedShareRepoController()
        {
            _ownedShareitemEntityStorage = new OwnedShareitemEntityStorage();

        }

        // public 

        private class OwnedShareItem : IId
        {
            public long Id { get; set; }

            public long CompanyId { get; set; }

            public int DenyCount { get; set; }
        }

        private class OwnedShareitemEntityStorage : EntityMemoryStorageBase<OwnedShareItem>
        {
            
        }
    }

    
}