namespace tradeStrategiesFrame.Model
{
    class Derivative : IValue
    {
        public double averageValue { get; set; }
        public double derivative { get; set; }
        public double averageDerivative { get; set; }

        public Derivative()
        {
            averageValue = 0;
            derivative = 0;
            averageDerivative = 0;
        }

        public double getValue()
        {
            return derivative;
        }
    }
}
