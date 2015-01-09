using tradeStrategiesFrame.SiftCanldesStrategies;

namespace tradeStrategiesFrame.Factories
{
    class SiftCandlesStrategyFactory
    {
        public static SiftCandlesStrategy createSiftStrategie(double siftStep)
        {
            return createMinMaxSiftStrategy(siftStep);
        }

        protected static SiftCandlesStrategy createMinMaxSiftStrategy(double siftStep)
        {
            return new MinMaxSiftStrategy(siftStep);
        }
    }
}
