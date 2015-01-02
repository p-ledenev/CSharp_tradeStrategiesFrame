using System;
using tradeStrategiesFrame.Model;

namespace tradeStrategiesFrame.TakeProfitStrategies
{
    class NoTakeProfitStrategy : TakeProfitStrategy
    {
        public NoTakeProfitStrategy(Portfolio pft)
            : base(pft)
        {
        }

        public override bool shouldTakeProfit(int start)
        {
            return false;
        }

        public override bool shouldReopenPosition(int start)
        {
            return true;
        }

        public override void readParamsFrom(String xml)
        {
        }
    }
}
