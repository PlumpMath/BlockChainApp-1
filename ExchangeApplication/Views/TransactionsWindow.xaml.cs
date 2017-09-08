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
using System.Windows.Shapes;
using ExchangeApplication.ViewModels;
using Logic.DependencyInjector;
using Logic.Finance;
using Logic.Storages;

namespace ExchangeApplication.Views
{
    /// <summary>
    /// Логика взаимодействия для TransactionsWindow.xaml
    /// </summary>
    public partial class TransactionsWindow : Window
    {
        public TransactionsWindow(IEnumerable<TransactionViewModel> models)
        {
            InitializeComponent();
            ListView_Transactions.ItemsSource = models;
            ListView_Transactions.SelectedIndex = ListView_Transactions.Items.Count - 1;
            ListView_Transactions.ScrollIntoView(ListView_Transactions.SelectedItem);
        }

        private void ListView_Transactions_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var transactionViewModel = ListView_Transactions.SelectedItem as TransactionViewModel;
            if (transactionViewModel == null)
            {
                throw new InvalidCastException();
            }
            // У чейна тот же айди, что и у транзакции
            Chain chain = Injector.Get<IChainStorage>().GetEntity(transactionViewModel.Id);

            var chainView = new ChainView(new ChainViewModel(chain));
            chainView.Show();
        }
    }
}
