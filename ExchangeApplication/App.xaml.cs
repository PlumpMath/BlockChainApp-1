using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Logic.DependencyInjector;
using Logic.Entitites;
using Logic.Fabrics;
using Logic.Interfaces;
using Logic.Storages;
using Utilities.Common;

namespace ExchangeApplication
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        // https://stackoverflow.com/a/10276293
        // Выполнение некоторых операций по старту
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AutofacConfig.ConfigureContainer();

            var users = CreateUsers();
            IExchange exchange = CreateExchange(users);

            MainWindow mainWindow = new MainWindow(exchange);
            mainWindow.Show();
        }

        private IEnumerable<User> CreateUsers()
        {
            IEnumerable<User> users = DI.Get<IUserFabric>().GetEntities(10);
            DI.Get<IExchangeUserStorage>().Save(users);
            return users;
        }

        private IExchange CreateExchange(IEnumerable<User> users)
        {
            IBank bank = DI.Get<IBank>();

            var exchangeUsers = new List<IExchangeUser>();
            exchangeUsers.AddRange(users);

            foreach (IExchangeUser exchangeUser in exchangeUsers)
            {
                bank.CreateAccount(exchangeUser);
            }
            // Банк как участник биржи
            exchangeUsers.Add((IExchangeUser)bank);

            var exchange = new Exchange(bank, exchangeUsers);
            return exchange;
        }
    }
}
