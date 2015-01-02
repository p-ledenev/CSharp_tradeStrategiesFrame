using tradeStrategiesFrame.AveragingConstructors;

namespace tradeStrategiesFrame.Factories
{
    class AveragingConstructorFactory
    {
        public static AveragingConstructor createConstructor()
        {
            return new SEMAAveragingConstructor();
            //return new MAAveragingConstructor();
            //return new TwoDepthAveragingConstructor(new FAveragingConstructor());
        }
    }
}
