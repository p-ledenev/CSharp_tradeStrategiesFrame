using tradeStrategiesFrame.ApproximationConstructors;

namespace tradeStrategiesFrame.Factories
{
    class ApproximationConstructorFactory
    {
        public static ApproximationConstructor createConstructor()
        {
            return new LinearApproximationConstructor();
            //return new WeightedGeometricalLinearApproximationConstructor(0.98);
        }
    }
}
