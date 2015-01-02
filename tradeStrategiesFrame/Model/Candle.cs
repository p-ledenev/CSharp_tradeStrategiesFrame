using System;

namespace tradeStrategiesFrame.Model
{
    class Candle : Description, IValue
    {
        public DateTime dt { get; set; }
        public int index { get; set; }
        public int timeOrder { get; set; }
        public double value { get; set; }
        public double nextValue { get; set; }
        public double nonGapValue { get; set; }
        public int volume { get; set; }

        public Candle()
        {
            this.value = 0;
            this.volume = 0;
            this.dt = DateTime.MinValue;

            this.index = -1;
            this.timeOrder = -1;

            this.nextValue = 0;
            this.nonGapValue = 0;
        }

        public Candle(double value, int volume, DateTime dt, int index)
        {
            this.value = value;
            this.nextValue = 0;
            this.volume = volume;

            this.dt = dt;
            this.index = index;
            this.timeOrder = index;
        }

        public Candle(String[] candle, int index)
        {
            this.dt = DateTime.Parse(candle[0]);
            this.value = Double.Parse(candle[4].Replace(".", ","));
            this.nextValue = 0;
            this.volume = Int32.Parse(candle[5].Replace(".", ","));
            this.index = index;
            this.timeOrder = index;
        }

        public Candle clone()
        {
            Candle candle = new Candle();

            candle.dt = this.dt;
            candle.value = this.value;
            candle.nextValue = this.nextValue;
            candle.volume = this.volume;
            candle.index = this.index;
            candle.timeOrder = this.timeOrder;

            return candle;
        }

        public double absoluteAbsSpread(Candle candle)
        {
            return Math.Abs(this.value - candle.value);
        }

        public double absoluteSpread(Candle candle)
        {
            return (this.value - candle.value);
        }

        public double relativeAbsSpread(Candle candle)
        {
            if (this.value != 0)
                return Math.Abs(this.value - candle.value) / this.value;

            return 0;
        }

        public double relativeSpread(Candle candle)
        {
            if (this.value != 0)
                return (this.value - candle.value) / this.value;

            return 0;
        }

        public String toString()
        {
            return index + "|" + dt.ToString("dd.MM.yyyy HH:mm:ss") + "|" + value + "|" + volume;
        }

        public double getValue()
        {
            return nonGapValue;
        }
    }
}
