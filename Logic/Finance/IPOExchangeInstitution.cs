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
                if (!user.WannaMakeSellDeals() || (offer = user.GetSellDealOffer()) == null)
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
                if (!user.WannaMakeBuyDeals() || (offer = user.GetBuyDealOffer()) == null)
                {
                    // Если участник-покупатель не захотел торговать сейчас
                    continue;
                }

                buyOffersList.Add(offer);
            }

            // предложения сделок, которые на этапе сопоставления остаются несвершившимися
            List<DealOffer> remainOffers = sellOffersList
                .Cast<DealOffer>()
                .Concat(buyOffersList)
                .ToList();

            ICollection<ConfirmedDeal> confirmedDeals 
                = TryMatchOffers(sellOffersList, buyOffersList, ref remainOffers);

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

            //оповещаем участников, чьи сделки не состоялись, об этом
            foreach (DealOffer offer in remainOffers)
            {
                GetUserByUniqueId(offer.UniqueExhcangeUserId)
                    .NotifyAboutFiredOffer(offer);
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
            ref List<DealOffer> remainOffers)
        {
            // Список итоговых сопоставленных сделок
            var deals = new List<ConfirmedDeal>();

            // Пробегаемся по списку предложений о продаже
            foreach (SellDealOffer sell in sellDealOffers)
            {
                BuyDealOffer buy = buyDealOffers.FirstOrDefault(b => IsDealsMatched(sell, b));
                if (buy == null)
                {
                    // Если сделка не состоялась
                    continue;
                }

                // если предложения о покупке и продаже совпадают, то добавляем в список подтвержденных сделок
                // при этом корректиуем показатели сделки о продаже, чтобы совпадали

                sell.Deal.ShareCount = buy.Deal.ShareCount <= sell.Deal.ShareCount
                    ? buy.Deal.ShareCount
                    : sell.Deal.ShareCount;
                sell.Deal.SharesCost = sell.Deal.SharePrice * sell.Deal.ShareCount;

                var confirmedDeal = new ConfirmedDeal
                {
                    BuyerUniqueId = buy.UniqueExhcangeUserId,
                    SellerUniqueId = sell.UniqueExhcangeUserId,
                    Deal = sell.Deal
                };
                deals.Add(confirmedDeal);

                // Убираем из общего списка предложений те, которые свершились
                remainOffers.Remove(buy);
                remainOffers.Remove(sell);
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

        private IExchangeUser GetUserByUniqueId(string uniqueId)
        {
            return _exchangeUsers.SingleOrDefault(user => user.UniqueExchangeId() == uniqueId);
        }
    }
}