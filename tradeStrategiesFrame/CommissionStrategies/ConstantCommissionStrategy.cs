using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tradeStrategiesFrame.CommissionStrategies
{
    abstract class ConstantCommissionStrategy : CommissionStrategy
    {
        protected double commission;

        protected ConstantCommissionStrategy(double commission) : base()
        {
            this.commission = commission;
        }
    }
}
