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
using Utilities.Common;

namespace ExchangeApplication.Views
{
    /// <summary>
    /// Логика взаимодействия для ChainView.xaml
    /// </summary>
    public partial class ChainView : Window
    {
        public ChainView(ChainViewModel viewModel)
        {
            InitializeComponent();
            TextBlock_Id.Text = viewModel.Id.ToString();
            TextBlock_Seller.Text = viewModel.SellerName;
            TextBlock_Buyer.Text = viewModel.BuyerName;

            TextBlock_TransactionValue.Text = viewModel.TransactionValue;
            TextBlock_TransactionComission.Text = viewModel.TransactionComission;

            TextBlock_PrevHash.Text = viewModel.PreviousHash;
            TextBlock_CurrentHash.Text = viewModel.CurrentHash;

            TextBlock_CreatedAt.Text = viewModel.CreatedAt;
        }
    }
}
