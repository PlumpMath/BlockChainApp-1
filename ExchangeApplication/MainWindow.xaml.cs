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
using Logic.Bank;
using Logic.DependencyInjector;
using Logic.Extensions;
using Logic.Finance;
using Logic.Interfaces;
using Logic.Observation;
using Logic.Participants;
using Logic.Storages;
using Utilities.Common;

namespace ExchangeApplication
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IObserver
    {
        private readonly IExchange _exchange;

        private readonly IBank _bank;

        private readonly IChainStorage _chainStorage;

        private readonly ITransactionStorage _transactionStorage;

        private readonly DispatcherTimer _exchangeTimer;

        private const int DepositPayoutTick = 29;

        private int _depositPayoutTickCount = 0;

        private long _exchangeStepCount = 0;

        public MainWindow(IExchange exchange)
        {
            _exchange = exchange;
            _exchange.SetChainChangeListener(this);

            _bank = Injector.Get<IBank>();
            _bank.SetChainChangeListener(this);

            _chainStorage = Injector.Get<IChainStorage>();
            _transactionStorage = Injector.Get<ITransactionStorage>();

            InitializeComponent();
            FillData();
            FillBaseData();
            FillCompanies();

            _exchangeTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(300)
            };
            _exchangeTimer.Tick += _exchangeTimer_Tick;
        }

        private void _exchangeTimer_Tick(object sender, EventArgs e)
        {
            ExchangeStepResult stepResult = _exchange.ExecuteExchanging();
            _exchangeStepCount++;

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
            FillBaseData();
            FillCompanies();
            if (stepResult != null) 
            {
                DisplayExchangeStepResult(new ExchangeStepResultViewModel(stepResult, _exchangeStepCount));
            }
            
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
                    Wallet = MiscUtils.FormatDouble(user.GetBankAccountValue()),
                    OwnedSharesCount = user.OwnedShareCount
                };
                ListView_Users.Items.Add(model);
            }
            //---------------------
        }

        private void FillBaseData()
        {
            TextBlock_BankMoney.Text = MiscUtils.FormatDouble(Injector.Get<IBank>().GetMoneyAmount());
            TextBlock_ShareCount.Text = Injector.Get<IShareStorage>().GetAll().Count().ToString();
        }

        private void FillCompanies()
        {
            ListView_Companies.Items.Clear();
            IEnumerable<Company> companies = Injector.Get<ICompanyStorage>().GetAll();

            for (int i = 0; i < companies.Count(); i++)
            {
                Company company = companies.ElementAt(i);
                var model = new CompanyListItemViewModel
                {
                    Name = company.Name,
                    ShareCount = company.GetCompanyShareCount(),
                    ShareBasePrice = MiscUtils.FormatDouble(company.GetCompanyShareBasePrice()),
                    ShareCurrentPrice = MiscUtils.FormatDouble(company.GetCompanyShareCurrentPrice()),
                    CompanyCost = MiscUtils.FormatDouble(company.GetCompanyCost()),
                    PriceChangeTrand = company.GetCompanySharePriceChangingTrand()
                };
                ListView_Companies.Items.Add(model);
            }
        }

        private void DisplayExchangeStepResult(ExchangeStepResultViewModel stepResultViewModel)
        {
            ListView_TransactionsSummary.Items.Add(stepResultViewModel);
            ListView_TransactionsSummary.SelectedIndex = ListView_TransactionsSummary.Items.Count - 1;
            ListView_TransactionsSummary.ScrollIntoView(ListView_TransactionsSummary.SelectedItem);
        }

        private void StartStopExchangeProcess(object sender, RoutedEventArgs e)
        {
            string title = _exchangeTimer.IsEnabled
                ? "Начать торги"
                : "Закрыть торги";
            MenuItem_StartStopExchange.Header = title;
            _exchangeTimer.IsEnabled = !_exchangeTimer.IsEnabled;
        }

        public void CommonMessage(string message)
        {
            WriteToLog(message);
        }

        public void Transaction(Transaction transaction)
        {
            CreateNewChain(transaction);
        }

        public void Exception(Exception exception)
        {
            WriteToLog("ОШИБКА! " + exception.Message);
        }

        private void WriteToLog(string text)
        {
            var time = DateTime.Now.ToShortTimeString();
            TextBox_Log.Text += time + ") " + text + Environment.NewLine;
            TextBox_Log.ScrollToEnd();
            TextBox_Log.CaretIndex = TextBox_Log.Text.Length - 1;
        }

        private void CreateNewChain(Transaction transaction)
        {
            var chain = Chain.CreateFromTransaction(transaction);
            _chainStorage.Save(chain);
        }

        private void MenuItem_Transactions_OnClick(object sender, RoutedEventArgs e)
        {
            IEnumerable<TransactionViewModel> transactions = Injector.Get<ITransactionStorage>()
                .GetAll()
                .Select(transaction => new TransactionViewModel(transaction));
            var transactionWindow = new TransactionsWindow(transactions);
            transactionWindow.Show();
        }
    }
}
