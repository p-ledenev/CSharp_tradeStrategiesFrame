using System.Collections.Generic;
using tradeStrategiesFrame.Model;

namespace tradeStrategiesFrame.SiftCanldesStrategies
{
    class NoSiftStrategy : SiftCandlesStrategy
    {
        public NoSiftStrategy()
            : base(0)
        {
        }

        public override List<Candle> sift(List<Candle> values)
        {
            return values;
        }
    }
}
