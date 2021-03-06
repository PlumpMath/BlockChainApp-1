﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Logic.Bank;
using Logic.DependencyInjector;
using Logic.ExchangeUsers;
using Logic.Fabrics;
using Logic.Finance;
using Logic.Interfaces;
using Logic.Participants;
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
            IBank bank = Injector.Get<IBank>();
            IEnumerable<IndividualUser> users = Injector.Get<IIndividualUserFabric>().GetEntities(30);
            IEnumerable<Company> companies = CreateCompanies();
            Injector.Get<IExchangeUserStorage>()
                .Save(users.Cast<ExchangeUserBase>()
                .Concat(companies
                    .Select(company => (ExchangeUserBase)company.GetExchangeUser())));

            IPOExchangeInstitution ipoExchangeInstitution = CreateExchange(users, companies);

            MainWindow mainWindow = new MainWindow(ipoExchangeInstitution);
            mainWindow.Show();
        }

        private IEnumerable<Company> CreateCompanies()
        {
            IEnumerable<Company> companies = Injector.Get<ICompanyFabric>().GetEntities(20);
            Injector.Get<ICompanyStorage>().Save(companies);

            foreach (Company company in companies)
            {
                int count = MiscUtils.GetRandomNumber(300, 10);
                double price = MiscUtils.GetRandomNumber(1000.0, 10.0);
                IEnumerable<Share> shares = Injector.Get<IShareFabric>().GetEntitiesOfCompany(company.Id, company.UniqueExchangeId(), count, price);
                Injector.Get<IShareStorage>().Save(shares);
            }
            Injector.Get<IExchangeUserStorage>()
                .Save(companies.Select(company => (ExchangeUserBase)company.GetExchangeUser()));
            return companies;
        }

        private IPOExchangeInstitution CreateExchange(IEnumerable<IndividualUser> users, IEnumerable<Company> companies)
        {
            IBank bank = Injector.Get<IBank>();

            var exchangeUsers = new List<IExchangeUser>();
            exchangeUsers.AddRange(users);
            exchangeUsers.AddRange(companies.Select(company => company.GetExchangeUser()));

            foreach (IExchangeUser exchangeUser in exchangeUsers)
            {
                bank.CreateAccount(exchangeUser);
            }
            // Банк как участник биржи
            exchangeUsers.Add(bank.GetExchangeUser());
            Injector.Get<IExchangeUserStorage>().Save((ExchangeUserBase)bank.GetExchangeUser());

            var exchange = new IPOExchangeInstitution(bank, exchangeUsers, companies);
            return exchange;
        }
    }
}
