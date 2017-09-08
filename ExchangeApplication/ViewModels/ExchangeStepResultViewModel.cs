using Logic.Finance;

namespace ExchangeApplication.ViewModels
{
    public class ExchangeStepResultViewModel
    {
        public long ExchangeStep { get; set; }

        public double StepDealMoneySumm { get; set; }

        public double StepDealBankComission { get; set; }

        public int StepDealCount { get; set; }

        public ExchangeStepResultViewModel(ExchangeStepResult result, long step)
        {
            ExchangeStep = step;
            StepDealMoneySumm = result.StepDealSumm;
            StepDealBankComission = result.StepDealBankComission;
            StepDealCount = result.StepDealCount;
        }
    }
}