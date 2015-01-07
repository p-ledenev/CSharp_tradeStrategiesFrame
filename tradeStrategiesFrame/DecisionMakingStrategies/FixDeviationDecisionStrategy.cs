using tradeStrategiesFrame.Model;

namespace tradeStrategiesFrame.DecisionMakingStrategies
{
    class FixDeviationDecisionStrategy : DeviationDecisionStrategy
    {
        public FixDeviationDecisionStrategy(Machine pft)
            : base(pft)
        {
        }

        protected override Deviation findDeviation(int start)
        {
            addDeviation(start, pft.minDepth);

            return devValues[start];
        }
    }
}
