using System;
using System.Reflection;
using System.Windows.Forms;
using Autofac;
using BlockChainApp.DependencyInjector;
using Dal.Interfaces;

namespace BlockChainApp
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            IContainer container = AutofacConfig.ConfigureContainer();
            DI.SetContainer(container);
            Application.Run(new Form1(DI.Get<IDbContext>()));
        }
    }

    public static class DI
    {
        private static IContainer _container;

        public static void SetContainer(IContainer container)
        {
            _container = container;
        }

        public static TInterface Get<TInterface>()
        {
            return _container.Resolve<TInterface>();
        }
    }
}
