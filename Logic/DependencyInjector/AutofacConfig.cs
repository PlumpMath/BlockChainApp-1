using System;
using Autofac;
using Logic.Entitites;
using Logic.Fabrics;
using Logic.Storages;

namespace Logic.DependencyInjector
{
    public class AutofacConfig
    {
        public static void ConfigureContainer()
        {
            // получаем экземпляр контейнера
            var builder = new ContainerBuilder();

            // регистрируем споставление типов
            builder.RegisterType<ChainMemoryStorage>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
            builder.RegisterType<ExchangeUserMemoryStorage>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
            builder.RegisterType<TransactionMemoryStorage>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<UserFabric>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<Bank.Bank>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            // создаем новый контейнер с теми зависимостями, которые определены выше
            DI.SetContainer(builder.Build());
        }
    }

    #region DependencyInjection

    public static class DI
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

    #endregion
}