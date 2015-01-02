using System;
using System.Collections.Generic;
using System.Linq;
using tradeStrategiesFrame.Model;

namespace tradeStrategiesFrame.SiftValuesStrategies
{
    class SimpleSiftValuesStrategy : SiftValuesStrategy
    {
        public SimpleSiftValuesStrategy(double siftStep)
            : base(siftStep)
        {
        }

        public override List<Candle> sift(List<Candle> values)
        {
            List<Candle> sifted = new List<Candle>();
            Candle[] arrValues = values.ToArray();

            double lastValue = arrValues[0].value;

            double gap = 0;
            for (int i = 0; i < arrValues.Length - 1; i++)
            {
                double dGap = ArrayCount.countGap(arrValues, i);
                gap += dGap;

                Candle value = arrValues[i];
                if (dGap == 0 && (sifted.Count <= 0 || Math.Abs(lastValue - value.value) / lastValue * 100 >= siftStep))
                {
                    value.nonGapValue = value.value - gap;
                    value.nextValue = arrValues[i + 1].value;
                    value.timeOrder = sifted.Count() + 1;

                    sifted.Add(value);

                    lastValue = value.value;
                }
            }

            return sifted;
        }
    }
}
