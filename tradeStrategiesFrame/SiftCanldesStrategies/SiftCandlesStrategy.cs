using System;
using System.Collections.Generic;
using tradeStrategiesFrame.Model;

namespace tradeStrategiesFrame.SiftCanldesStrategies
{
    abstract class SiftCandlesStrategy
    {
        protected double siftStep { get; set; }

        protected SiftCandlesStrategy(Double siftStep)
        {
            this.siftStep = siftStep;
        }

        public abstract List<Candle> sift(List<Candle> values);

    }
}
