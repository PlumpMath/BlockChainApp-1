using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ExchangeApplication.ViewModels;
using Logic.DependencyInjector;
using Logic.Interfaces;
using Utilities.Common;

namespace ExchangeApplication
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IExchange _exchange;

        private readonly IBank _bank;

        private readonly DispatcherTimer _exchangeTimer;

        public MainWindow(IExchange exchange)
        {
            _exchange = exchange;
            _bank = DI.Get<IBank>();
            InitializeComponent();
            FillData();

            _exchangeTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _exchangeTimer.Tick += _exchangeTimer_Tick;
        }

        private void _exchangeTimer_Tick(object sender, EventArgs e)
        {
            _exchange.ExecuteExchanging();
            FillData();
        }

        private void FillData()
        {
            ListView_Users.Items.Clear();
            IEnumerable<IExchangeUser> users = _exchange.GetExchangeUsers();
            for (int i = users.Count() - 1; i >= 0; i--)
            {
                IExchangeUser user = users.ElementAt(i);
                var model = new ExchangeUserViewModel
                {
                    Name = user.Name,
                    Wallet = MiscUtils.FormatDouble(_bank.GetAccountValue(user.Id))
                };
                ListView_Users.Items.Add(model);
            }
            //---------------------
            TextBlock_BankMoney.Text = MiscUtils.FormatDouble(DI.Get<IBank>().GetMoneyAmount());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string title = _exchangeTimer.IsEnabled
                ? "Начать торги"
                : "Закрыть торги";
            Button_StartStopExchange.Content = title;
            _exchangeTimer.IsEnabled = !_exchangeTimer.IsEnabled;
        }
    }
}
