using System;
using tradeStrategiesFrame.Factories;
using tradeStrategiesFrame.Model;
using tradeStrategiesFrame.TakeProfitStrategies;

namespace tradeStrategiesFrame.DecisionMakingStrategies
{
    abstract class DecisionStrategy
    {
        protected Portfolio pft { get; set; }
        protected TakeProfitStrategy takeProfitStrategie { get; set; }

        protected DecisionStrategy(Portfolio pft)
        {
            this.pft = pft;

            takeProfitStrategie = TakeProfitStrategieFactory.createTakeProfitStrategie(pft);
        }

        public TradeSignal tradeSignalFor(int start)
        {
            String lastCross = pft.getLastOpenPositionTrade().stock.mode;
            String cross = determOperationMode(start);

            if (!lastCross.Equals(cross))
                return TradeSignal.openPosition(cross);

            if (takeProfitStrategie.shouldTakeProfit(start))
                return TradeSignal.closePosition(cross);

            if (takeProfitStrategie.shouldReopenPosition(start))
                return TradeSignal.openPosition(cross);

            return TradeSignal.closePosition(Stock.none);
        }

        public abstract String determOperationMode(int start);

        public abstract void readParamsFrom(String xml);
    }
}
