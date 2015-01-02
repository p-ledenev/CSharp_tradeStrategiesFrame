using System;
using tradeStrategiesFrame.AveragingConstructors;
using tradeStrategiesFrame.Factories;
using tradeStrategiesFrame.Model;

namespace tradeStrategiesFrame.DecisionMakingStrategies
{
    abstract class DeviationDecisionStrategy : DecisionStrategy
    {
        public Deviation[] devValues { get; set; }

        protected DeviationDecisionStrategy(Portfolio pft)
            : base(pft)
        {
            clearDeviation();
            Candle.keys = new String[] { "dev" };
        }

        public override String determOperationMode(int start)
        {
            if (start < 1)
                return Stock.none;

            Deviation deviation = findDeviation(start);

            pft.parent.siftedValues[start].addRequisitieByKeyIndex(0, Math.Round(deviation.averageDeviation * 1000, 4).ToString());

            return crossOperationFor(deviation);
        }

        protected String crossOperationFor(Deviation deviation)
        {
            if (deviation == null)
                return Stock.none;

            if (deviation.averageDeviation > 0)
                return Stock.buy;

            if (deviation.averageDeviation < 0)
                return Stock.sell;

            return Stock.none;
        }

        protected virtual void addDeviation(int start, int depth)
        {
            AveragingConstructor constructor = AveragingConstructorFactory.createConstructor();

            devValues[start].averageValue = constructor.average(pft.parent.siftedValues, start, depth);

            devValues[start].deviation = (devValues[start].averageValue - devValues[start - 1].averageValue) / devValues[start].averageValue;

            devValues[start].averageDeviation = constructor.average(devValues, start, depth);
        }

        protected abstract Deviation findDeviation(int start);

        public override void readParamsFrom(string xml)
        {
        }

        public void clearDeviation()
        {
            devValues = new Deviation[pft.parent.siftedValues.Length];

            for (int i = 0; i < devValues.Length; i++)
                devValues[i] = new Deviation();
        }
    }
}
