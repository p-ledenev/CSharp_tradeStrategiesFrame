using System;
using tradeStrategiesFrame.Model;

namespace tradeStrategiesFrame.TakeProfitStrategies
{
    class TakeProfitFromMaxStrategy : TakeProfitStrategy
    {
        public double k {get; set;}

        public TakeProfitFromMaxStrategy(Portfolio pft, double k)
            : base(pft)
        {
            this.k = k;
        }

        public override bool shouldTakeProfit(int start)
        {
            double value = pft.getRearValue(start);
            double extremumValue = pft.getLastTrade().stock.buyValue;

            int sgn = (pft.getLastOpenPositionTrade().stock.mode.Equals(Stock.buy)) ? 1 : -1;

            for (int i = pft.getLastTrade().stock.index; i <= start; i++)
                if (sgn * pft.getRearValue(i) > sgn * extremumValue)
                    extremumValue = pft.getRearValue(i);

            return sgn * (extremumValue - value) / extremumValue > k;            
        }

        public override bool shouldReopenPosition(int start)
        {
            /*
            double value = pft.getRearValue(start);
            double lastValue = pft.getLastTrade().stock.buyValue;

            int sgn = (pft.getLastOpenPositionTrade().stock.mode.Equals(Stock.buy)) ? 1 : -1;

            return sgn * (value - lastValue) / lastValue > maxProfitPercent;
            */

            return false;
        }

        public override void readParamsFrom(String xml)
        {
        }
    }
}
