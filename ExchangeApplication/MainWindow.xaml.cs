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
using Logic.DependencyInjector;
using Logic.Interfaces;

namespace ExchangeApplication
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IExchange _exchange;

        public MainWindow(IExchange exchange)
        {
            _exchange = exchange;
            InitializeComponent();
            FillData();
        }

        private void FillData()
        {
            //ListBox_Users.Items.Clear();
            IEnumerable<IExchangeUser> users = _exchange.GetExchangeUsers();
            for (int i = users.Count() - 1; i >= 0; i--)
            {
                IExchangeUser user = users.ElementAt(i);
                //ListBox_Users.Items.Add(user);
            }
            //---------------------
            //TextBox_BankMoneyAmount.Text = DI.Get<IBank>().GetMoneyAmount().ToString("0.##");
        }
    }
}
