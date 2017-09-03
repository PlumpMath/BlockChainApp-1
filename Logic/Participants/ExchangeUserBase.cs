using System;
using Logic.Interfaces;
using Utilities.Common;

namespace Logic.Participants
{
    public abstract class ExchangeUserBase : IExchangeUser
    {
        public long Id { get; set; }

        public DateTime CreatedAt { get; }

        public string Name { get; set; }

        /// <summary>
        /// Участник может не захотеть вести торги на этот раз
        /// </summary>
        public virtual bool WannaMissTurn()
        {
            return MiscUtils.ContinueByRandom();
        }

        protected ExchangeUserBase()
        {
            Id = 0;
            CreatedAt = DateTime.UtcNow;
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}