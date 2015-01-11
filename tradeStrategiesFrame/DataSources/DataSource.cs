using System;
using System.Collections.Generic;
using tradeStrategiesFrame.Model;

namespace tradeStrategiesFrame.DataSources
{
    abstract class DataSource
    {
        public abstract List<Candle> readCandlesFrom(String fileName);

    }
}
