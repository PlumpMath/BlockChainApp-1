using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Autofac;
using BlockChainApp.DependencyInjector;
using Logic.Entitites;
using Logic.Fabric;
using Logic.Interfaces;
using Logic.Storages;

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
            AutofacConfig.ConfigureContainer();

            FillUsers();
            Application.Run(new MainForm(CreateExchange()));
        }

        private static void FillUsers()
        {
            IUserFactory factory = DI.Get<IUserFactory>();
            IUserStorage storage = DI.Get<IUserStorage>();
            for (var i = 0; i < 10; i++)
            {
                User user = factory.GenerateEntity(i);
                storage.Save(user);
            }
        }

        private static IExchange CreateExchange()
        {
            IBank bank = DI.Get<IBank>();
            IChainStorage chainStorage = DI.Get<IChainStorage>();

            var users = new List<IExchangeUser>();
            users.AddRange(DI.Get<IUserStorage>().GetAll());
            users.Add((IExchangeUser)bank);
            var exchange = new Exchange(bank, chainStorage, users);
            return exchange;
        }
    }

}
