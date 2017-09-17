using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Logic.Bank;
using Logic.DependencyInjector;
using Logic.Extensions;
using Logic.Interfaces;
using Logic.Observation;
using Logic.Participants;
using Logic.Storages;
using Utilities.Common;
using Utilities.Convert;

namespace Logic.Finance
{
    public class IPOExchangeInstitution
    {
        private static readonly double MaxTransactionPrice
            = ConfigurationManager.AppSettings["MaxTransactionPrice"].ParseAsDouble();

        /// <summary>
        /// Список участников
        /// </summary>
        private readonly List<IExchangeUser> _exchangeUsers;

        private readonly List<Company> _companies;

        /// <summary>
        /// Управляющий транзакциями банк
        /// </summary>
        private readonly IBank _bank;

        public delegate void IPOExchangeStepExecuted(ExchangeStepResult result);

        /// <summary>
        /// Событие, происходящее по исполнению торгов
        /// </summary>
        public event IPOExchangeStepExecuted ExchangeStepExecuted;

        public IPOExchangeInstitution(IBank bank, IEnumerable<IExchangeUser> users, IEnumerable<Company> companies)
        {
            _bank = bank;
            _exchangeUsers = new List<IExchangeUser>();
            _exchangeUsers.AddRange(users);

            _companies = new List<Company>();
            _companies.AddRange(companies);
        }

        public IEnumerable<IExchangeUser> GetExchangeUsers() => _exchangeUsers;

        public void ExecuteExchanging()
        {
            var result = new ExchangeStepResult();

            var sellOffersList = new List<SellDealOffer>();
            var buyOffersList = new List<BuyDealOffer>();

            // Формирование списка предложений о продаже
            foreach (IExchangeUser user in _exchangeUsers)
            {
                SellDealOffer offer = null;
                if (!user.WannaMakeDeals() || (offer = user.GetSellDealOffer()) == null)
                {
                    // Если участник-покупатель не захотел торговать сейчас
                    continue;
                }

                sellOffersList.Add(offer);
            }

            // Формирование списка предлжений о покупке
            foreach (IExchangeUser user in _exchangeUsers)
            {
                BuyDealOffer offer = null;
                if (!user.WannaMakeDeals() || (offer = user.GetBuyDealOffer()) == null)
                {
                    // Если участник-покупатель не захотел торговать сейчас
                    continue;
                }

                buyOffersList.Add(offer);
            }

            ICollection<ConfirmedDeal> confirmedDeals = TryMatchOffers(sellOffersList, buyOffersList, 
                out ICollection<SellDealOffer> firedDeals);

            foreach (ConfirmedDeal deal in confirmedDeals)
            {
                if (!MakeDeal(deal))
                {
                    // Если сделка не удалась по некоторой причине
                    continue;
                }
                result.StepDealCount++;
                result.StepDealSumm += deal.Deal.SharesCost;
            }

            // оповещаем продавцом, чьи сделки не состоялись, об этом
            foreach (SellDealOffer firedDeal in firedDeals)
            {
                IExchangeUser seller = GetUserByUniqueId(firedDeal.UniqueExhcangeUserId);
                seller.NotifyAboutFiredSellOffer(firedDeal);
            }

            if (result.StepDealCount != 0)
            {
                // Если сделки состоялись, то сообщаем слушателям
                ExchangeStepExecuted?.Invoke(result);
            }
        }

        private static double SellBuyPriceMaxDifference = 0.1;

        private ICollection<ConfirmedDeal> TryMatchOffers(
            ICollection<SellDealOffer> sellDealOffers, 
            ICollection<BuyDealOffer> buyDealOffers,
            out ICollection<SellDealOffer> firedDeals)
        {
            // Список итоговых сопоставленных сделок
            var deals = new List<ConfirmedDeal>();
            firedDeals = new List<SellDealOffer>();

            // Пробегаемся по списку предложений о продаже
            foreach (SellDealOffer sell in sellDealOffers)
            {
                BuyDealOffer buy = buyDealOffers
                    .FirstOrDefault(b => IsDealsMatched(sell, b));
                if (buy == null)
                {
                    // Если сделка не состоялась, нужно оповестить об этом владельца, пусть меняет тактику
                    firedDeals.Add(sell);
                    continue;
                }

                // если предложения о покупке и продаже совпадают, то добавляем в список подтвержденных сделок
                // при этом корректиуем показатели сделки о продаже, чтобы совпадали

                sell.Deal.ShareCount = buy.Deal.ShareCount <= sell.Deal.ShareCount
                    ? buy.Deal.ShareCount
                    : sell.Deal.ShareCount;
                sell.Deal.SharesCost = sell.Deal.SharePrice * sell.Deal.ShareCount;
                deals.Add(new ConfirmedDeal
                {
                    BuyerUniqueId = buy.UniqueExhcangeUserId,
                    SellerUniqueId = sell.UniqueExhcangeUserId,
                    Deal = sell.Deal
                });
            }
            return deals;
        }

        private bool IsDealsMatched(SellDealOffer sell, BuyDealOffer buy)
        {
            return buy.UniqueExhcangeUserId != sell.UniqueExhcangeUserId // Чтобы с самим собой сделка не случилась
                    //&& buy.ShareCount <= sell.ShareCount // У покупатель хочет купить 
                    && buy.Deal.ShareCompanyId == sell.Deal.ShareCompanyId // компании акций совпадают
                    && Math.Abs(buy.Deal.SharePrice - sell.Deal.SharePrice) <= SellBuyPriceMaxDifference; // Если цены покупки и продажи близки по значению
        }

        private bool MakeDeal(ConfirmedDeal confirmedDeal)
        {
            IExchangeUser seller = GetUserByUniqueId(confirmedDeal.SellerUniqueId);
            IExchangeUser buyer = GetUserByUniqueId(confirmedDeal.BuyerUniqueId);

            seller.DeattachShares(confirmedDeal.Deal);
            buyer.TakeShares(confirmedDeal.Deal);

            if (!_bank.TransferMoney(buyer, seller, confirmedDeal.Deal.SharesCost, out double comission))
            {
                // Если по какой-то причине не получилось трансферинга денег
                return false;
            }
            return true;
        }

        private IExchangeUser GetUserById(long id)
        {
            return _exchangeUsers.SingleOrDefault(user => user.Id == id);
        }

        private IExchangeUser GetUserByUniqueId(string uniqueId)
        {
            return _exchangeUsers.SingleOrDefault(user => user.UniqueExchangeId() == uniqueId);
        }
    }
}