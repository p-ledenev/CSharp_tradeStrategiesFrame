using tradeStrategiesFrame.SiftValuesStrategies;

namespace tradeStrategiesFrame.Factories
{
    class SiftValuesStrategieFactory
    {
        public static SiftValuesStrategy createSiftStrategie(double siftStep)
        {
            return new MinMaxSiftValuesStrategy(siftStep);
            //return new NoSiftValuesStrategie();
        }
    }
}
