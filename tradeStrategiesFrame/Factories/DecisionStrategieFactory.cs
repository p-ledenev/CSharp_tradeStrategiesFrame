using tradeStrategiesFrame.DecisionMakingStrategies;
using tradeStrategiesFrame.Model;

namespace tradeStrategiesFrame.Factories
{
    class DecisionStrategieFactory
    {
        public static DecisionStrategy createDecisionStrategie(Machine pft)
        {
            return new FixDeviationDecisionStrategy(pft);
            //return new FixApproximationDecisionStrategie(pft);
        }
    }
}
