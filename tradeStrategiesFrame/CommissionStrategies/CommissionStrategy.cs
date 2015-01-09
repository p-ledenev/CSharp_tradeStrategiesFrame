using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tradeStrategiesFrame.CommissionStrategies
{
    abstract class CommissionStrategy
    {
        protected CommissionStrategy()
        {
        }

        public abstract double computeOpenPositionCommission(CommissionRequest request);

        public abstract double computeClosePositionCommission(CommissionRequest request);
    }
}
