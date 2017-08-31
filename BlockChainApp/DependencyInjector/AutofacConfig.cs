using System.Reflection;
using System.Windows.Forms;
using Autofac;
using Dal.Dal;
using Dal.Interfaces;

namespace BlockChainApp.DependencyInjector
{
    public class AutofacConfig
    {
        public static IContainer ConfigureContainer()
        {
            // получаем экземпляр контейнера
            var builder = new ContainerBuilder();

            var assembly = Assembly.GetExecutingAssembly();
            builder.RegisterAssemblyTypes(assembly)
                .Where(type => type.IsSubclassOf(typeof(Form)));

            // регистрируем споставление типов
            builder.RegisterType<SqLiteContext>().AsImplementedInterfaces();

            // создаем новый контейнер с теми зависимостями, которые определены выше
            IContainer container = builder.Build();
            return container;
        }
    }
}