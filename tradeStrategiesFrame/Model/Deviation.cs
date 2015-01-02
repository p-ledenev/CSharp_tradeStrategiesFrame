namespace tradeStrategiesFrame.Model
{
    class Deviation : IValue
    {
        public double averageValue { get; set; }
        public double deviation { get; set; }
        public double averageDeviation { get; set; }

        public Deviation()
        {
            averageValue = 0;
            deviation = 0;
            averageDeviation = 0;
        }

        public double getValue()
        {
            return deviation;
        }
    }
}
