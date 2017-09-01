using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Logic.Entitites;
using Logic.Interfaces;
using Logic.Storages;

namespace BlockChainApp
{
    public partial class MainForm : Form
    {
        private readonly IExchange _exchange;

        public MainForm(IExchange exchange)
        {
            _exchange = exchange;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FillUsers();
        }

        private void FillUsers()
        {
            ListBox_Users.Items.Clear();
            IEnumerable<IExchangeUser> users = _exchange.GetExchangeUsers();
            for (int i = users.Count() - 1; i >= 0; i--)
            {
                IExchangeUser user = users.ElementAt(i);
                var listViewitem = new ListViewItem
                {
                    Tag = user,
                    Text = $@"{user.Id}. {user.Name}. Money: {user.Wallet}"
                };

                ListBox_Users.Items.Add(user);
            }
            
        }
    }
}
