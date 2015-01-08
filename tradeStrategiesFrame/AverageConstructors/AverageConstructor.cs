using tradeStrategiesFrame.Model;

namespace tradeStrategiesFrame.AverageConstructors
{
    abstract class AverageConstructor
    {
        public abstract double average(IValue[] values, int start, int depth);
    }
}
