using System;
using tradeStrategiesFrame.ApproximationConstructors;
using tradeStrategiesFrame.Factories;
using tradeStrategiesFrame.Model;

namespace tradeStrategiesFrame.DecisionMakingStrategies
{
    internal abstract class ApproximationDecisionStrategy : DecisionStrategy
    {
        protected bool reverseDirection { get; set; }
        public Approximation[] appValues { get; set; }

        protected ApproximationDecisionStrategy(Machine pft)
            : base(pft)
        {
            reverseDirection = false;
            clearApproximation();
        }

        public override Position.Direction determineTradeDirection(int start)
        {
            Approximation ap = approximationSearch(start);
            appValues[start] = ap;

            return crossOperationFor(ap);
        }

        protected Approximation approximationSearch(int start)
        {
            ApproximationConstructor constructor = ApproximationConstructorFactory.createConstructor();

            Approximation ap = constructor.approximate(pft.portfolio.candles, start, pft.minDepth);
            setCandleRequisities(ap, start);

            int depth = start - pft.getLastTrade().dateIndex + getLastTradeDepth();

            for (int i = pft.minDepth; i < depth;)
            {
                int j = computeSearchIndex(i, pft.minDepth, depth);
                
                Approximation current = constructor.approximate(pft.portfolio.candles, start, j);
                setCandleRequisities(current, start);

                if (!shouldContinueApproximationSearch(current, start)) 
                    return combine(ap, current);

                ap = current;

                i += countIncrement(i - pft.minDepth);
            }

            return ap;
        }

        protected int countIncrement(int i)
        {
            if (i < 10) return 1;
            if (i < 20) return 2;
            if (i < 30) return 3;
            if (i < 50) return 5;

            return 10;
        }

        protected int computeSearchIndex(int i, int begin, int end)
        {
            if (!reverseDirection)
                return i;

            return end - i - 1 + begin;
        }

        protected Position.Direction crossOperationFor(Approximation ap)
        {
            if (ap == null || ap.k == null) return Position.Direction.None;

            if (ap.k[0] > 0) return Position.Direction.Buy;
            if (ap.k[0] < 0) return Position.Direction.Sell;

            return Position.Direction.None;
        }

        public int getLastTradeDepth()
        {
            return appValues[pft.getLastTrade().dateIndex].depth;
        }

        protected abstract bool shouldContinueApproximationSearch(Approximation ap, int start);

        protected abstract Approximation combine(Approximation toReturn, Approximation current);

        protected abstract void setCandleRequisities(Approximation ap, int start);

        public void clearApproximation()
        {
            appValues = new Approximation[pft.portfolio.candles.Length];

            for (int i = 0; i < appValues.Length; i++)
                appValues[i] = new Approximation();
        }
    }
}
