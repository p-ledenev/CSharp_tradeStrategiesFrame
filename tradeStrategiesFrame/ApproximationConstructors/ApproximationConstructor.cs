using tradeStrategiesFrame.Model;

namespace tradeStrategiesFrame.ApproximationConstructors
{
    abstract class ApproximationConstructor
    {
        public abstract Approximation approximate(IValue[] values, int start, int depth);
    }
}
