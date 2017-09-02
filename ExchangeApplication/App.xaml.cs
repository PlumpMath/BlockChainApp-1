using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Logic.DependencyInjector;
using Logic.Entitites;
using Logic.Fabric;
using Logic.Interfaces;
using Logic.Storages;

namespace ExchangeApplication
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        // https://stackoverflow.com/a/10276293
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AutofacConfig.ConfigureContainer();
            FillUsers();
            IExchange exchange = CreateExchange();

            MainWindow mainWindow = new MainWindow(exchange);
            mainWindow.Show();
        }

        private void FillUsers()
        {
            IUserFactory factory = DI.Get<IUserFactory>();
            IUserStorage storage = DI.Get<IUserStorage>();
            for (var i = 0; i < 10; i++)
            {
                User user = factory.GenerateEntity();
                storage.Save(user);
            }
        }

        private IExchange CreateExchange()
        {
            IBank bank = DI.Get<IBank>();
            IChainStorage chainStorage = DI.Get<IChainStorage>();

            var users = new List<IExchangeUser>();
            users.AddRange(DI.Get<IUserStorage>().GetAll());

            foreach (IExchangeUser user in users)
            {
                bank.CreateAccount(user);
            }
            // Банк как участник биржи
            users.Add((IExchangeUser)bank);

            var exchange = new Exchange(bank, chainStorage, users);
            return exchange;
        }
    }
}
