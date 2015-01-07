using System;
using tradeStrategiesFrame.Factories;
using tradeStrategiesFrame.Model;
using tradeStrategiesFrame.TakeProfitStrategies;

namespace tradeStrategiesFrame.DecisionMakingStrategies
{
    abstract class DecisionStrategy
    {
        protected Machine pft { get; set; }
        protected TakeProfitStrategy takeProfitStrategie { get; set; }

        protected DecisionStrategy(Machine pft)
        {
            this.pft = pft;

            takeProfitStrategie = TakeProfitStrategieFactory.createTakeProfitStrategie(pft);
        }

        public TradeSignal tradeSignalFor(int start)
        {
            Position.Direction lastDirection = pft.getLastOpenPositionTrade().getDirection();
            Position.Direction direction = determineTradeDirection(start);

            if (!direction.Equals(lastDirection))
                return TradeSignal.forCloseAndOpenPosition(direction);

            if (takeProfitStrategie.shouldTakeProfit(start))
                return TradeSignal.forClosePosition(direction);

            if (takeProfitStrategie.shouldReopenPosition(start))
                return TradeSignal.forCloseAndOpenPosition(direction);

            return TradeSignal.forClosePosition(Position.Direction.None);
        }

        public abstract Position.Direction determineTradeDirection(int start);

        public abstract void readParamsFrom(String xml);
    }
}
