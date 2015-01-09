using tradeStrategiesFrame.DecisionMakingStrategies;
using tradeStrategiesFrame.Model;

namespace tradeStrategiesFrame.Factories
{
    class DecisionStrategyFactory
    {
        public static DecisionStrategy createDecisionStrategie(Machine machine)
        {
            return createDerivativeDecisionStrategy(machine);
            //return createApproximationDecisionStrategy(machine);
        }

        protected static DecisionStrategy createDerivativeDecisionStrategy(Machine machine)
        {
            return new DerivativeDecisionStrategy(machine);
        }

        protected static DecisionStrategy createApproximationDecisionStrategy(Machine machine)
        {
            return new DerivativeDecisionStrategy(machine);
        }
    }
}
