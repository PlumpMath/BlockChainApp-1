using System;
using Autofac;

namespace Logic.DependencyInjector
{
    public static class Injector
    {
        private static IContainer _container;

        public static void SetContainer(IContainer container)
        {
            if (_container != null)
            {
                throw new ArgumentException(nameof(container));
            }
            _container = container;
        }

        public static TInterface Get<TInterface>()
        {
            if (_container == null)
            {
                throw new NullReferenceException(nameof(_container));
            }
            return _container.Resolve<TInterface>();
        }
    }
}