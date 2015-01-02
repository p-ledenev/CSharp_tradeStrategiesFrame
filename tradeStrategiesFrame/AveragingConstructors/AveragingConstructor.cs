using tradeStrategiesFrame.Model;

namespace tradeStrategiesFrame.AveragingConstructors
{
    abstract class AveragingConstructor
    {
        public abstract double average(IValue[] values, int start, int depth);
    }
}
