﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tradeStrategiesFrame.CommissionStrategies
{
    class ScalpingCommissionStrategy : ConstantCommissionStrategy
    {
        public ScalpingCommissionStrategy(double commission)
            : base(commission)
        {
        }

        public override double computeOpenPositionCommission(CommissionRequest request)
        {
            return commission * request.volume;
        }

        public override double computeClosePositionCommission(CommissionRequest request)
        {
            if (request.intraday)
                return 0;

            return commission * request.volume;
        }
    }
}
