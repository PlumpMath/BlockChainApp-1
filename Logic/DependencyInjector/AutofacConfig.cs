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
            Injector.SetContainer(builder.Build());
        }
    }
}