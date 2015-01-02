using System;
using System.Collections.Generic;
using tradeStrategiesFrame.Model;

namespace tradeStrategiesFrame.SiftValuesStrategies
{
    abstract class SiftValuesStrategy
    {
        protected double siftStep { get; set; }

        protected SiftValuesStrategy(Double siftStep)
        {
            this.siftStep = siftStep;
        }

        public abstract List<Candle> sift(List<Candle> values);

    }
}
