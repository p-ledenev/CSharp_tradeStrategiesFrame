using tradeStrategiesFrame.CommissionStrategies;

namespace tradeStrategiesFrame.Factories
{
    class CommissionStrategyFactory
    {
        public static CommissionStrategy createConstantCommissionStrategy(double commission)
        {
            return createScaplingCommissionStrategy(commission);
        }

        protected static CommissionStrategy createScaplingCommissionStrategy(double commission)
        {
            return new ScalpingCommissionStrategy(commission);
        }
    }
}
