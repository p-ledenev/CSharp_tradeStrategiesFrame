using System;
using tradeStrategiesFrame.AveragingConstructors;
using tradeStrategiesFrame.Factories;
using tradeStrategiesFrame.Model;

namespace tradeStrategiesFrame.DecisionMakingStrategies
{
    abstract class DeviationDecisionStrategy : DecisionStrategy
    {
        public Deviation[] devValues { get; set; }

        protected DeviationDecisionStrategy(Machine pft)
            : base(pft)
        {
            clearDeviation();
            Candle.keys = new String[] { "dev" };
        }

        public override Position.Direction determineTradeDirection(int start)
        {
            if (start < 1)
                return Position.Direction.None;

            Deviation deviation = findDeviation(start);

            pft.portfolio.candles[start].addRequisitieByKeyIndex(0, Math.Round(deviation.averageDeviation * 1000, 4).ToString());

            return crossOperationFor(deviation);
        }

        protected Position.Direction crossOperationFor(Deviation deviation)
        {
            if (deviation == null)
                return Position.Direction.None;

            if (deviation.averageDeviation > 0)
                return Position.Direction.Buy;

            if (deviation.averageDeviation < 0)
                return Position.Direction.Sell;

            return Position.Direction.None;
        }

        protected virtual void addDeviation(int start, int depth)
        {
            AveragingConstructor constructor = AveragingConstructorFactory.createConstructor();

            devValues[start].averageValue = constructor.average(pft.portfolio.candles, start, depth);

            devValues[start].deviation = (devValues[start].averageValue - devValues[start - 1].averageValue) / devValues[start].averageValue;

            devValues[start].averageDeviation = constructor.average(devValues, start, depth);
        }

        protected abstract Deviation findDeviation(int start);

        public override void readParamsFrom(string xml)
        {
        }

        public void clearDeviation()
        {
            devValues = new Deviation[pft.portfolio.candles.Length];

            for (int i = 0; i < devValues.Length; i++)
                devValues[i] = new Deviation();
        }
    }
}
