using System;
using System.Collections.Generic;
using System.Linq;
using tradeStrategiesFrame.Model;

namespace tradeStrategiesFrame.SiftValuesStrategies
{
    class MinMaxSiftValuesStrategy : SiftValuesStrategy
    {
        public MinMaxSiftValuesStrategy(double siftStep)
            : base(siftStep)
        {
        }

        public override List<Candle> sift(List<Candle> values)
        {
            List<Candle> sifted = new List<Candle>();
            Candle[] arrValues = values.ToArray();

            double maxValue = arrValues[0].value;
            double minValue = arrValues[0].value;

            double gap = 0;
            for (int i = 0; i < arrValues.Length - 1; i++)
            {
                double dGap = 0; // ArrayCount.countGap(arrValues, i);
                gap += dGap;

                Candle value = arrValues[i];

                if (value.value > maxValue) maxValue = value.value;
                if (value.value < minValue) minValue = value.value;

                if (dGap == 0 && (sifted.Count <= 0 ||
                    Math.Abs(maxValue - value.value) / maxValue * 100 >= siftStep ||
                    Math.Abs(minValue - value.value) / minValue * 100 >= siftStep))
                {
                    value.nonGapValue = value.value - gap;
                    value.nextValue = arrValues[i + 1].value;
                    value.timeOrder = sifted.Count() + 1;

                    sifted.Add(value);

                    maxValue = value.value;
                    minValue = value.value;
                }
            }

            return sifted;
        }
    }
}
