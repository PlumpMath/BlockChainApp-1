using Logic.Finance;
using Utilities.Common;

namespace ExchangeApplication.ViewModels
{
    public class ExchangeStepResultViewModel
    {
        public long ExchangeStep { get; set; }

        public string StepDealMoneySumm { get; set; }

        public int StepDealCount { get; set; }

        public ExchangeStepResultViewModel(ExchangeStepResult result, long step)
        {
            ExchangeStep = step;
            StepDealMoneySumm = result.StepDealSumm.FormatDouble();
            StepDealCount = result.StepDealCount;
        }
    }
}