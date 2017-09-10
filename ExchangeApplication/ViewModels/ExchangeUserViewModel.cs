using System.Collections.Generic;
using Logic.Extensions;
using Logic.Finance;
using Logic.Interfaces;
using Utilities.Common;

namespace ExchangeApplication.ViewModels
{
    public class ExchangeUserViewModel
    {
        public string Name { get; set; }

        public string Wallet { get; set; }

        public int OwnedSharesCount { get; set; }

        public string OwnedSharesCost { get; set; }

        public ExchangeUserViewModel(IExchangeUser user)
        {
            Name = user.Name;
            Wallet = user.GetBankAccountValue().FormatDouble();
            OwnedSharesCount = user.OwnedShareCount;
            OwnedSharesCost = user.GetOwnedShares().GetSharesCost().FormatDouble();
        }
    }
}