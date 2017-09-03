using System;
using System.Collections.Generic;
using System.Linq;
using Logic.Interfaces;
using Utilities.Common;

namespace Logic.Fabrics
{
    public abstract class FabricBase<TEntity> : IFabricBase<TEntity> where TEntity : IIdentificable, new()
    {
        protected virtual bool UniqueNames { get; set; } = true;

        protected Func<TEntity, TEntity> CreateNewInstance;

        public virtual TEntity GetEntity()
        {
            return new TEntity()
            {
                Id = 0,
                Name = GetRandomName()
            };
        }

        public virtual IEnumerable<TEntity> GetEntities(int count)
        {
            var list = new List<TEntity>();
            for (var i = 0; i < count; i++)
            {
                list.Add(GetEntity());
            }
            return list;
        }

        protected string GetRandomName()
        {
            var names = UniqueNames
                ? EntityNames.Except(ExceptEntityNames)
                : EntityNames;
            var name = names.GetRandomEntity();
            ExceptEntityNames.Add(name);
            return name;
        }

        protected virtual IEnumerable<string> EntityNames { get; set; } 
            = Enumerable.Empty<string>();

        private IList<string> ExceptEntityNames { get; }
            = new List<string>();
    }
}