using tradeStrategiesFrame.Model;
using tradeStrategiesFrame.TakeProfitStrategies;

namespace tradeStrategiesFrame.Factories
{
    class TakeProfitStrategyFactory
    {
        public static TakeProfitStrategy createTakeProfitStrategy(Machine machine)
        {
            return createNoTakeProfitStrategy(machine);
        }

        protected static TakeProfitStrategy createNoTakeProfitStrategy(Machine machine)
        {
            return new NoTakeProfitStrategy(machine);
        }
    }
}
