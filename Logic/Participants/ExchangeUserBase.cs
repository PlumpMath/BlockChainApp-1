using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Logic.DependencyInjector;
using Logic.Exceptions;
using Logic.Extensions;
using Logic.Finance;
using Logic.Helpers;
using Logic.Interfaces;
using Logic.Storages;
using Utilities.Common;

namespace Logic.Participants
{
    public abstract class ExchangeUserBase : IExchangeUser
    {
        public static double MaxIncreaseDecreaseRate = 0.01;
        public static double MinIncreaseDecreaseRate = 0.005;

        public long Id { get; set; }

        public DateTime CreatedAt { get; }

        public string Name { get; set; }

        public abstract string UniqueExchangeId();

        private readonly OwnedShareRepoController _ownedShareRepoController;

        protected ExchangeUserBase()
        {
            Id = 0;
            CreatedAt = DateTime.UtcNow;
            _ownedShareRepoController = new OwnedShareRepoController();
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

        public int OwnedShareCount => GetMineShares().Count();

        /// <summary>
        /// Участник может не захотеть вести торги на этот раз
        /// </summary>
        public virtual bool WannaMissTurn()
        {
            int riskness = GetCurrentRiskness();
            if (OwnedShareCount == 0)
            {
                riskness += 20;
            } else if (OwnedShareCount > 100)
            {
                riskness -= 40;
            }
            return MakeRandomDecision(riskness);
        }

        public bool WannaBuyShares(ShareInvoiceInfo invoice)
        {
            if (!this.GotEnoughMoney(invoice.Cost))
            {
                // Если денег недостаточно, то пользователь не покупает
                return false;
            }

            double myWallet = this.GetBankAccountValue();
            if ((myWallet - invoice.Cost) / myWallet <= 0.2)
            {
                // Если на счету останется меньше 20% после сделки, то тоже отказываемся
                return false;
            }
            int riskness = GetCurrentRiskness();
            if (invoice.Trand == SharePriceChangingType.Increasing)
            {
                riskness += 10;
            }
            else if (invoice.Trand == SharePriceChangingType.Decreasing)
            {
                riskness -= 10;
            }

            Share share = invoice.Shares.First();

            if (share.CurrentPrice / share.BasePrice > 1.5)
            {
                // расскомментирование приводит к образованию пузырей
                // riskness += 20;
            }
            return MakeRandomDecision(riskness);
        }

        public bool WannaSellShares(double offer, out ShareInvoiceInfo invoice)
        {
            ICollection<Share> mineShares = GetMineShares();
            if (!mineShares.Any())
            {
                // если у продавца нет акций
                invoice = null;
                return false;
            }

            invoice = GetRandomShareInvoiceInfo(mineShares, offer);
            return invoice != null;
        }

        public ShareInvoiceInfo GetRandomShareInvoiceInfo(ICollection<Share> shares, double invoiceOffer)
        {
            // Взятие акций одной компании
            shares = ChooseCompanyAndReturnShares(shares, out long companyId);
            int shareForSaleCount = shares.Count();
            double currentPrice = shares.First().CurrentPrice;

            double allSharesCost = currentPrice * shareForSaleCount;

            if (allSharesCost > invoiceOffer)
            {
                int couldBuyCount = (int)Math.Floor(invoiceOffer / currentPrice);

                if (couldBuyCount == 0)
                {
                    return null;
                }
                shares = shares.Take(couldBuyCount).ToArray();
            }

            // вычисление общей стоимости акций на основе текущей цены
            double invoice = shares.GetSharesCost();

            var shareInvoiceInfo = new ShareInvoiceInfo
            {
                Shares = shares,
                CompanyId = companyId,
                Count = shareForSaleCount,
                Cost = invoice,
                Trand = shares.First().PriceChangingTrand
            };

            return shareInvoiceInfo;
        }

        public void DeattachShares(ShareInvoiceInfo invoice)
        {
            if (invoice.Shares.Any(share => share.OwnerUniqueId != UniqueExchangeId()))
            {
                throw new NotExchangeUserOwnedSharesException(
                    $"Переданные акции не принадлежат пользователю {UniqueExchangeId()}");
            }

            // Непосредственно отчуждение прав на владение акциями
            var list = new List<Share>();
            foreach (Share invoiceShare in invoice.Shares)
            {
                string id = UniqueExchangeId();
                invoiceShare.OwnerUniqueId = id;
                list.Add(invoiceShare);
            }
            invoice.Shares = list;
            // Раздумье, стоит ли повысить цену на акции, если хочет или остались в наличии
            IncreaseSharePriceIfWantTo(invoice.CompanyId);
        }

        public void TakeShares(ShareInvoiceInfo invoice)
        {
            if (invoice.Shares.Any(share => share.OwnerUniqueId == UniqueExchangeId()))
            {
                throw new NotExchangeUserOwnedSharesException(
                    $"Переданные акции уже принадлежат пользователю {UniqueExchangeId()}");
            }
            // Перебивка владельца акций
            var list = new List<Share>();
            foreach (Share invoiceShare in invoice.Shares)
            {
                string id = UniqueExchangeId();
                invoiceShare.OwnerUniqueId = id;
                list.Add(invoiceShare);
            }
            invoice.Shares = list;
            Injector.Get<IShareStorage>().Save(list);
            // На вновь полученные акции пользователь может захотеть повысить цену сразу
            IncreaseSharePriceIfWantTo(invoice.CompanyId);
        }

        public void DecreaseSharePriceIfWantTo(long companyId)
        {
            ChangeSharePriceIfWantTo(companyId, SharePriceChangingType.Decreasing);
        }

        public void IncreaseSharePriceIfWantTo(long companyId)
        {
            ChangeSharePriceIfWantTo(companyId, SharePriceChangingType.Increasing);
        }

        public double MakeInvoiceOffer()
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

        public override string ToString() => $"{Name}, UniqueId {UniqueExchangeId()}";

        private bool MakeRandomDecision(int riskness = 50)
        {
            const int maxLevel = 100;
            riskness = riskness.CorrectRiskness();

            // нижний уровень желания, после которого юзер не соглашается
            int lowLevel = maxLevel - riskness;
            var rand = MiscUtils.GetRandomNumber(maxLevel);
            return rand >= lowLevel;
        }

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