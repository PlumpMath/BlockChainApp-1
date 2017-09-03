using Autofac;
using Logic.Bank;
using Logic.Fabrics;
using Logic.Interfaces;
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
            RegisterStorages(builder);
            RegisterFabrics(builder);

            builder.RegisterType<CentralBank>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
            // создаем новый контейнер с теми зависимостями, которые определены выше
            Injector.SetContainer(builder.Build());
        }

        private static void RegisterStorages(ContainerBuilder builder)
        {
            builder.RegisterType<ChainMemoryStorage>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<ExchangeUserMemoryStorage>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<TransactionMemoryStorage>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<CompanyMemoryStorage>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<ShareMemoryStorage>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }

        private static void RegisterFabrics(ContainerBuilder builder)
        {
            builder.RegisterType<IndividualUserFabric>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<CompanyFabric>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<ShareFabric>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}