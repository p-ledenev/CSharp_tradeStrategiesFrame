using System;
using tradeStrategiesFrame.Model;

namespace tradeStrategiesFrame.TakeProfitStrategies
{
    abstract class TakeProfitStrategy
    {
        public Machine machine { get; set; }

        public TakeProfitStrategy(Machine machine)
        {
            this.machine = machine;
        }

        public abstract bool shouldTakeProfit(int start);

        public abstract bool shouldReopenPosition(int start);

        public abstract void readParamsFrom(String xml);
    }
}
