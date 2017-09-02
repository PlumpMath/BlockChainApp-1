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
using ExchangeApplication.Views;
using Logic.DependencyInjector;
using Logic.Entitites;
using Logic.Extensions;
using Logic.Interfaces;
using Logic.Storages;
using Utilities.Common;

namespace ExchangeApplication
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IExchangeEventListener
    {
        private readonly IExchange _exchange;

        private readonly IBank _bank;

        private readonly IChainStorage _chainStorage;

        private readonly DispatcherTimer _exchangeTimer;

        private const int DepositPayoutTick = 9;

        private int _depositPayoutTickCount = 0;

        public MainWindow(IExchange exchange)
        {
            _exchange = exchange;
            _exchange.SetChainChangeListener(this);

            _bank = DI.Get<IBank>();
            _chainStorage = DI.Get<IChainStorage>();

            InitializeComponent();
            FillData();

            _exchangeTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(200)
            };
            _exchangeTimer.Tick += _exchangeTimer_Tick;
        }

        private void _exchangeTimer_Tick(object sender, EventArgs e)
        {
            _exchange.ExecuteExchanging();
            if (_depositPayoutTickCount == DepositPayoutTick)
            {
                _exchange.PayoutDepositPercents();
                _depositPayoutTickCount = 1;
            }
            else
            {
                _depositPayoutTickCount++;
            }

            FillData();
        }

        private void FillData()
        {
            ListView_Users.Items.Clear();
            IEnumerable<IExchangeUser> users = _exchange
                .GetExchangeUsers()
                .OrderByDescending(u => u.GetBankAccountValue());

            for (int i = 0; i < users.Count(); i++)
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

        public void CommonMessage(string message)
        {
            TextBox_Log.Text += DateTime.Now.ToShortTimeString() + ") " 
                + message + Environment.NewLine;
            TextBox_Log.ScrollToEnd();
            TextBox_Log.CaretIndex = TextBox_Log.Text.Length - 1;
        }

        public void Transaction(Chain chain)
        {
            ListView_BlockChain.Items.Add(new ChainViewModel(chain));
            ListView_BlockChain.SelectedIndex = ListView_BlockChain.Items.Count - 1;
            ListView_BlockChain.ScrollIntoView(ListView_BlockChain.SelectedItem);
        }

        private void ListView_BlockChain_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var chainViewModel = ListView_BlockChain.SelectedItem as ChainViewModel;
            if (chainViewModel == null)
            {
                throw new InvalidCastException();
            }

            IUserStorage storage = DI.Get<IUserStorage>();
            chainViewModel.SellerName = storage.GetEntity(chainViewModel.SellerId).Name;
            chainViewModel.BuyerName = storage.GetEntity(chainViewModel.BuyerId).Name;

            var chainView = new ChainView(chainViewModel);
            chainView.Show();
        }
    }
}
