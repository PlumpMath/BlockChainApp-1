using System;
using System.Collections.Generic;
using Logic.DependencyInjector;
using Logic.Finance;
using Logic.Helpers;
using Logic.Interfaces;
using Logic.Storages;
using Utilities.Common;

namespace Logic.Participants
{
    public abstract class ExchangeUserBase : IExchangeUser
    {
        public long Id { get; set; }

        public DateTime CreatedAt { get; }

        public string Name { get; set; }

        public abstract string UniqueExchangeId();

        /// <summary>
        /// Участник может не захотеть вести торги на этот раз
        /// </summary>
        public virtual bool WannaMissTurn()
        {
            return MiscUtils.ContinueByRandom();
        }

        public bool WannaSellShares(Share share, int count)
        {
            throw new NotImplementedException();
        }

        public bool WannaBuyShares(Share share, int count)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Share> GetOwnedShares()
        {
            return Injector.Get<IShareStorage>().GetByOwnerId(UniqueExchangeId());
        }

        public abstract ExchangeUserType GetExchangeUserType();

        protected ExchangeUserBase()
        {
            Id = 0;
            CreatedAt = DateTime.UtcNow;
        }

        public override string ToString()
        {
            return $"{Name}, UniqueId {UniqueExchangeId()}";
        }
    }
}