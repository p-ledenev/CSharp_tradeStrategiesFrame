using System;
using tradeStrategiesFrame.Model;

namespace tradeStrategiesFrame.TakeProfitStrategies
{
    abstract class TakeProfitStrategy
    {
        public Portfolio pft { get; set; }

        public TakeProfitStrategy(Portfolio pft)
        {
            this.pft = pft;
        }

        public abstract bool shouldTakeProfit(int start);

        public abstract bool shouldReopenPosition(int start);

        public abstract void readParamsFrom(String xml);
    }
}
