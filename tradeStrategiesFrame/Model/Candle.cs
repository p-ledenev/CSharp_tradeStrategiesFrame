using System;

namespace tradeStrategiesFrame.Model
{
    class Candle : Description, IValue
    {
        public DateTime date { get; set; }
        public int dateIndex { get; set; }
        public double value { get; set; }
        public double tradeValue { get; set; }
        public int volume { get; set; }

        public Candle()
        {
            this.value = 0;
            this.volume = 0;
            this.date = DateTime.MinValue;
            this.dateIndex = -1;
            this.tradeValue = 0;
        }

        public Candle(double value, int volume, DateTime date, int index)
        {
            this.value = value;
            this.tradeValue = 0;
            this.volume = volume;
            this.date = date;
            this.dateIndex = index;
        }

        public Candle(String[] candle, int index)
        {
            this.date = DateTime.Parse(candle[0]);
            this.value = Double.Parse(candle[4].Replace(".", ","));
            this.tradeValue = 0;
            this.volume = Int32.Parse(candle[5].Replace(".", ","));
            this.dateIndex = index;
        }

        public Candle clone()
        {
            Candle candle = new Candle
            {
                date = this.date,
                value = this.value,
                tradeValue = this.tradeValue,
                volume = this.volume,
                dateIndex = this.dateIndex
            };

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

        public double getValue()
        {
            return value;
        }
    }
}
