using tradeStrategiesFrame.AverageConstructors;

namespace tradeStrategiesFrame.Factories
{
    class AverageConstructorFactory
    {
        public static AverageConstructor createConstructor()
        {
            return createMovingAverageConstructor();
            //return createTwoDepthAverageConstructor();
        }

        protected static AverageConstructor createMovingAverageConstructor()
        {
            return new MovingAverageConstructor();
        }

        protected static AverageConstructor createTwoDepthAverageConstructor()
        {
            return new TwoDepthAverageConstructor(createMovingAverageConstructor());
        }
    }
}
