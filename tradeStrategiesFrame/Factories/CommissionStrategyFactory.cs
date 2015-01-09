using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tradeStrategiesFrame.CommissionStrategies;
using tradeStrategiesFrame.SiftCanldesStrategies;

namespace tradeStrategiesFrame.Factories
{
    class CommissionStrategyFactory
    {
        public static CommissionStrategy createConstantCommissionStrategie(double commission)
        {
            return createScaplingCommissionStrategy(commission);
        }

        protected static CommissionStrategy createScaplingCommissionStrategy(double commission)
        {
            return new ScalpingCommissionStrategy(commission);
        }
    }
}
