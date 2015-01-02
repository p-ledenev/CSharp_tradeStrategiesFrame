using System.Collections.Generic;
using tradeStrategiesFrame.Model;

namespace tradeStrategiesFrame.SiftValuesStrategies
{
    class NoSiftValuesStrategy : SiftValuesStrategy
    {
        public NoSiftValuesStrategy()
            : base(0)
        {
        }

        public override List<Candle> sift(List<Candle> values)
        {
            return values;
        }
    }
}
