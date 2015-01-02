using tradeStrategiesFrame.Model;
using tradeStrategiesFrame.TakeProfitStrategies;

namespace tradeStrategiesFrame.Factories
{
    class TakeProfitStrategieFactory
    {
        public static TakeProfitStrategy createTakeProfitStrategie(Portfolio pft)
        {
            return new NoTakeProfitStrategy(pft);
        }
    }
}
