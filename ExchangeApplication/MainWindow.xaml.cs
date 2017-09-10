using System;
using System.Collections;
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
        private readonly IPOExchangeInstitution _ipoExchangeInstitution;

        private readonly IBank _bank;

        private readonly IChainStorage _chainStorage;

        private readonly ITransactionStorage _transactionStorage;

        private readonly DispatcherTimer _exchangeTimer;

        private const int DepositPayoutTick = 29;

        private int _depositPayoutTickCount = 0;

        private long _exchangeStepCount = 0;


        public MainWindow(IPOExchangeInstitution ipoExchangeInstitution)
        {
            _ipoExchangeInstitution = ipoExchangeInstitution;

            _bank = Injector.Get<IBank>();
            _bank.SetChainChangeListener(this);

            _chainStorage = Injector.Get<IChainStorage>();
            _transactionStorage = Injector.Get<ITransactionStorage>();

            InitializeComponent();
            FillExchangeUsersList();
            FillBaseData();
            FillCompanies();

            _exchangeTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(300)
            };
            _exchangeTimer.Tick += _exchangeTimer_Tick;

            _ipoExchangeInstitution.ExchangeStepExecuted += IpoExchangeInstitutionOnExchangeStepExecuted;
        }

        private void IpoExchangeInstitutionOnExchangeStepExecuted(ExchangeStepResult result)
        {
            _exchangeStepCount++;
            FillExchangeUsersList();
            FillBaseData();
            FillCompanies();
            if (result != null)
            {
                DisplayExchangeStepResult(new ExchangeStepResultViewModel(result, _exchangeStepCount));
            }

            // Выплаты по депоситам, если наступил рубеж
            if (_depositPayoutTickCount == DepositPayoutTick)
            {
                _bank.PayoutDepositPercent();
                _depositPayoutTickCount = 1;
            }
            else
            {
                _depositPayoutTickCount++;
            }
        }

        private void _exchangeTimer_Tick(object sender, EventArgs e)
        {
            _ipoExchangeInstitution.ExecuteExchanging();
        }

        private void FillExchangeUsersList()
        {
            ListView_Users.ItemsSource = _ipoExchangeInstitution
                .GetExchangeUsers()
                .OrderByDescending(u => u.GetBankAccountValue())
                .Select(user => new ExchangeUserViewModel(user));
        }

        private void FillBaseData()
        {
            TextBlock_BankMoney.Text = Injector.Get<IBank>().GetMoneyAmount().FormatDouble();
            ICollection<Share> shares = Injector.Get<IShareStorage>().GetAll();
            TextBlock_ShareCount.Text = shares.Count().ToString();
            TextBlock_ShareCosts.Text = shares.GetSharesCost().FormatDouble();
        }

        private void FillCompanies()
        {
            ListView_Companies.ItemsSource = Injector.Get<ICompanyStorage>()
                .GetAll()
                .Select(company => new CompanyListItemViewModel(company));
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
