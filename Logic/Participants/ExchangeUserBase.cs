using System;
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
        public static double MaxIncreaseDecreaseRate = 0.1;
        public static double MinIncreaseDecreaseRate = 0.01;

        public long Id { get; set; }

        public DateTime CreatedAt { get; }

        public string Name { get; set; }

        public abstract string UniqueExchangeId();

        /// <summary>
        /// Получение списка акций, принадлежащих пользователю
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Share> GetMineShares(long? companyId = null)
        {
            var shares = Injector.Get<IShareStorage>().GetByOwnerId(this.UniqueExchangeId());
            return companyId.HasValue 
                ? shares.Where(share => share.CompanyId == companyId) 
                : shares;
        }

        /// <summary>
        /// Участник может не захотеть вести торги на этот раз
        /// </summary>
        public virtual bool WannaMissTurn()
        {
            return MakeRandomDecision();
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
            int riskness = MiscUtils.GetRandomNumber(90, 20);
            if (invoice.Trand == SharePriceChangingType.Increasing)
            {
                riskness += 10;
            }
            else if (invoice.Trand == SharePriceChangingType.Decreasing)
            {
                riskness -= 10;
            }
            return MakeRandomDecision(riskness);
        }

        public void DeattachShares(ShareInvoiceInfo invoice)
        {
            if (invoice.Shares.Any(share => share.OwnerUniqueId != UniqueExchangeId()))
            {
                throw new NotExchangeUserOwnedSharesException(
                    $"Переданные акции не принадлежат пользователю {UniqueExchangeId()}");
            }

            // Непосредственно отчуждение прав на владение акциями
            invoice.Shares = invoice.Shares.Select(share =>
            {
                share.OwnerUniqueId = null;
                return share;
            });
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
            invoice.Shares = invoice.Shares.Select(share =>
            {
                share.OwnerUniqueId = UniqueExchangeId();
                return share;
            });
            Injector.Get<IShareStorage>().Save(invoice.Shares);
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

        public void ChangeSharePriceIfWantTo(long companyId)
        {
            ChangeSharePriceIfWantTo(companyId, ShareHelpers.GetPriceChangingTypeByRandom());
        }

        private void ChangeSharePriceIfWantTo(long companyId, SharePriceChangingType sharePriceChangingType)
        {
            IEnumerable<Share> mineShares = GetMineShares(companyId);
            if (!WannaChangeAPrice(mineShares, out double? changeRate))
            {
                // Если нет хочет/может повысить цену на акции
                //sharePriceChangingType = SharePriceChangingType.Fixed;
                return;
            }
            Injector.Get<IShareStorage>().ChangeShareCurrentPrice(companyId, changeRate ?? 0, sharePriceChangingType);
        }

        private bool WannaChangeAPrice(IEnumerable<Share> mineShares, out double? changeRate)
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

        public IEnumerable<Share> GetOwnedShares()
        {
            return Injector.Get<IShareStorage>().GetByOwnerId(UniqueExchangeId());
        }

        public abstract ExchangeUserType GetExchangeUserType();

        protected ExchangeUserBase()
        {
            Id = 0;
            CreatedAt = DateTime.UtcNow;
        }

        public override string ToString()
        {
            return $"{Name}, UniqueId {UniqueExchangeId()}";
        }

        private bool MakeRandomDecision(int riskness = 50)
        {
            const int maxLevel = 100;
            int lowLevel = maxLevel - riskness;
            var rand = MiscUtils.GetRandomNumber(maxLevel);
            return rand >= lowLevel;
        }
    }
}