using Logic.DependencyInjector;
using Logic.Exceptions;
using Logic.Extensions;
using Logic.Finance;
using Logic.Helpers;
using Logic.Interfaces;
using Logic.Storages;
using System;
using System.Collections.Generic;
using System.Linq;
using Utilities.Common;

namespace Logic.Participants
{
    public abstract class ExchangeUserBase : IExchangeUser
    {
        public static double MaxIncreaseDecreaseRate = 0.01;
        public static double MinIncreaseDecreaseRate = 0.005;

        public static int RisknessDecreaseOwnedShareCount = 20;
        public static int RisknessIncreaseOwnedShareCount = 100;


        public long Id { get; set; }

        public DateTime CreatedAt { get; }

        public string Name { get; set; }

        public abstract string UniqueExchangeId();

        protected ExchangeUserBase()
        {
            Id = 0;
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Получение списка акций, принадлежащих пользователю
        /// </summary>
        /// <returns></returns>
        private ICollection<Share> GetMineShares(long? companyId = null)
        {
            var shares = Injector.Get<IShareStorage>().GetByOwnerId(this.UniqueExchangeId()).ToArray();
            return companyId.HasValue 
                ? shares.Where(share => share.CompanyId == companyId).ToArray()
                : shares;
        }

        public int OwnedShareCount => GetMineShares().Count;

        /// <summary>
        /// Участник может не захотеть вести торги на этот раз
        /// </summary>
        public virtual bool WannaMakeDeals()
        {
            int riskness = GetCurrentRiskness();
            if (OwnedShareCount < RisknessDecreaseOwnedShareCount)
            {
                // Если акций мало, то желание торговать повышается
                riskness += 20;
            }
            else if (OwnedShareCount > RisknessIncreaseOwnedShareCount)
            {
                // Если акций много, то желание торговаться уменьшается
                riskness -= 40;
            }
            return MakeRandomDecision(riskness);
        }

        public void DeattachShares(Deal deal)
        {
            if (deal.Shares.Any(share => share.OwnerUniqueId != UniqueExchangeId()))
            {
                throw new NotExchangeUserOwnedSharesException(
                    $"Переданные акции не принадлежат пользователю {UniqueExchangeId()}");
            }

            // Непосредственно отчуждение прав на владение акциями
            var list = new List<Share>();
            foreach (Share invoiceShare in deal.Shares)
            {
                invoiceShare.OwnerUniqueId = null;
                list.Add(invoiceShare);
            }
            deal.Shares = list;
            // Раздумье, стоит ли повысить цену на акции, если хочет или остались в наличии
            IncreaseSharePriceIfWantTo(deal.ShareCompanyId);
        }

        public void TakeShares(Deal deal)
        {
            if (deal.Shares.Any(share => share.OwnerUniqueId == UniqueExchangeId()))
            {
                throw new NotExchangeUserOwnedSharesException(
                    $"Переданные акции уже принадлежат пользователю {UniqueExchangeId()}");
            }
            // Перебивка владельца акций
            var list = new List<Share>();
            foreach (Share invoiceShare in deal.Shares)
            {
                invoiceShare.OwnerUniqueId = UniqueExchangeId();
                list.Add(invoiceShare);
            }
            deal.Shares = list;
            Injector.Get<IShareStorage>().Save(list);
            // На вновь полученные акции пользователь может захотеть повысить цену сразу
            IncreaseSharePriceIfWantTo(deal.ShareCompanyId);
        }

        /// <summary>
        /// оповещаем пользователя о том, что его предложение о сделке не было осуществлено
        /// </summary>
        public void NotifyAboutFiredSellOffer(SellDealOffer offer)
        {
            // TODO Сделать более умное решение насчет сбрасывания цены
            if (MakeRandomDecision())
            {
                // Если не купили акции, значит цена может быть завышенной
                DecreaseSharePriceIfWantTo(offer.Deal.ShareCompanyId);
            }
        }

        public void DecreaseSharePriceIfWantTo(long companyId)
        {
            ChangeSharePriceIfWantTo(companyId, SharePriceChangingType.Decreasing);
        }

        public void IncreaseSharePriceIfWantTo(long companyId)
        {
            ChangeSharePriceIfWantTo(companyId, SharePriceChangingType.Increasing);
        }

        public double HowManyCouldSpendMoney()
        {
            const double maxCouldSpendRate = 0.8;
            double mineMoney = this.GetBankAccountValue();
            return mineMoney * maxCouldSpendRate;
        }

        private void ChangeSharePriceIfWantTo(long companyId, SharePriceChangingType sharePriceChangingType)
        {
            ICollection<Share> mineShares = GetMineShares(companyId);
            if (!WannaChangeAPrice(mineShares, out double? changeRate))
            {
                // Если нет хочет/может повысить цену на акции
                sharePriceChangingType = SharePriceChangingType.Fixed;
                // return;
            }
            Injector.Get<IShareStorage>().ChangeShareCurrentPrice(companyId, changeRate ?? 0, sharePriceChangingType);
        }

        private bool WannaChangeAPrice(ICollection<Share> mineShares, out double? changeRate)
        {
            // Раздумье, стоит ли повысить цену на акции, если остались в наличии
            if (!MakeRandomDecision() && mineShares.Any())
            {
                // Если не хочет менять ничего, то завершение операции
                changeRate = null;
                return false;
            }
            changeRate = MiscUtils.GetRandomNumber(MaxIncreaseDecreaseRate, MinIncreaseDecreaseRate);
            return true;
        }

        public ICollection<Share> GetOwnedShares() => GetMineShares();

        public abstract ExchangeUserType GetExchangeUserType();


        public BuyDealOffer GetBuyDealOffer()
        {
            // Выбираем компанию, у которой будем покупать акции
            ICollection<Share> shares = ChooseCompanyShares();

            double maxCosts = HowManyCouldSpendMoney();

            DealOffer offer = CreateDealOffer(shares, maxCosts);
            return offer != null 
                ? new BuyDealOffer(offer)
                : null;
        }

        public SellDealOffer GetSellDealOffer()
        {
            ICollection<Share> shares = GetMineShares();
            if (!shares.Any())
            {
                // если у меня нет акций
                return null;
            }
            DealOffer offer = CreateDealOffer(shares);
            return offer != null
                ? new SellDealOffer(offer)
                : null;
        }

        private ICollection<Share> ChooseCompanyShares()
        {
            ICollection<Share> shares = Injector.Get<IShareStorage>().GetAll()
                .Where(s => s.OwnerUniqueId != UniqueExchangeId())
                .ToArray();

            IEnumerable<long> companyIds = shares
                .Select(s => s.CompanyId)
                .Distinct();

            // Выбираю компанию на основании тренда ее акций
            long? choosedCompanyId = null;
            foreach (long companyId in companyIds)
            {
                ICollection<Share> shareByCompany = shares
                    .Where(s => s.CompanyId == companyId)
                    .ToArray();

                Share share = shareByCompany.First();
                int riskness = GetCurrentRiskness();
                if (share.PriceChangingTrand == SharePriceChangingType.Increasing)
                {
                    riskness += 10;
                }
                else if (share.PriceChangingTrand == SharePriceChangingType.Decreasing)
                {
                    riskness -= 10;
                }

                if (share.CurrentPrice / share.BasePrice > 1.5)
                {
                    // расскомментирование приводит к образованию пузырей
                    // riskness += 20;
                }

                if (MakeRandomDecision(riskness))
                {
                    choosedCompanyId = companyId;
                    break;
                }
            }

            if (choosedCompanyId == null)
            {
                return null;
            }
            return shares.Where(s => s.CompanyId == choosedCompanyId.Value).ToArray();
        }

        /// <summary>
        /// Создает предложение о сделке, будь то о покупке или о продаже, на основе переданных акций
        /// </summary>
        /// <param name="shares"></param>
        /// <param name="maxCost"></param>
        /// <returns></returns>
        private DealOffer CreateDealOffer(ICollection<Share> shares, double? maxCost = null)
        {
            // Взятие акций одной компании
            shares = ChooseCompanyAndReturnShares(shares, out long companyId);
            int shareForSaleCount = shares.Count;
            double currentPrice = shares.First().CurrentPrice;

            double allSharesCost = currentPrice * shareForSaleCount;

            if (maxCost.HasValue && allSharesCost > maxCost.Value)
            {
                int couldBuyCount = (int)Math.Floor(maxCost.Value / currentPrice);

                if (couldBuyCount == 0)
                {
                    return null;
                }
                shares = shares.Take(couldBuyCount).ToArray();
            }
            return new DealOffer
            {
                UniqueExhcangeUserId = UniqueExchangeId(),
                Deal = new Deal
                {
                    Shares = shares,
                    SharePrice = currentPrice,
                    ShareCount = shares.Count,
                    SharesCost = shares.GetSharesCost(),
                    ShareCompanyId = companyId
                }
            };
        }

        public override string ToString() => $"{Name}, UniqueId {UniqueExchangeId()}";

        private bool MakeRandomDecision(int riskness = 50)
        {
            const int maxLevel = 100;
            riskness = riskness.CorrectRiskness(maxLevel);

            // нижний уровень желания, после которого юзер не соглашается
            int lowLevel = maxLevel - riskness;
            var rand = MiscUtils.GetRandomNumber(maxLevel);
            return rand >= lowLevel;
        }

        /// <summary>
        /// Возвращает рандомное число, соответствующее рискованности конкретнорго игрока
        /// </summary>
        /// <returns></returns>
        private int GetCurrentRiskness()
        {
            return MiscUtils.GetRandomNumber(90, 10);
        }

        /// <summary>
        /// Выбирает компанию, акции которой хочет продать, на основе имеющихся акций
        /// </summary>
        private ICollection<Share> ChooseCompanyAndReturnShares(ICollection<Share> shares, out long outCompanyId)
        {
            // Получение всех айдишников компаний, акции которых юзер имеет
            ICollection<long> companyIds = shares
                .Select(share => share.CompanyId)
                .Distinct()
                .ToArray();
            ICollection<Company> companies = companyIds
                .Select(id => Injector.Get<ICompanyStorage>().GetEntity(id))
                .ToArray();

            // Нахождение компании с отрицательным ростом, чтобы продать
            Company companyWithMinGrow = companies.GetCompanyWithMinimalGrow();
            if (companyWithMinGrow.CalculatePriceChangePercent() <= -10)
            {
                // Продаем акции, у которых прибыльность упала на 10%
                outCompanyId = companyWithMinGrow.Id;
                return shares
                    .Where(share => share.CompanyId == companyWithMinGrow.Id)
                    .ToArray();
            }

            // Нахождение максимального числа капитализации
            Company maxGrowCompany = companies
                .GetCompanyWithMaxGrow();

            if (maxGrowCompany.CalculatePriceChangePercent() >= 5)
            {
                // Продаем акции, у которых прибыльность выросла на 5%
                outCompanyId = maxGrowCompany.Id;
                return shares
                    .Where(share => share.CompanyId == maxGrowCompany.Id)
                    .ToArray();
            }

            int count = companyIds.Count;

            int randomIndex = MiscUtils.GetRandomNumber(count);
            long companyId = companyIds.ElementAt(randomIndex);
            outCompanyId = companyId;
            return shares.Where(share => share.CompanyId == companyId).ToArray();
        }
    }
}