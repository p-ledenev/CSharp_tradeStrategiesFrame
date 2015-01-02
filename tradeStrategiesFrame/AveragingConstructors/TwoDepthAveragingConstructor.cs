using tradeStrategiesFrame.Model;

namespace tradeStrategiesFrame.AveragingConstructors
{
    internal class TwoDepthAveragingConstructor : AveragingConstructor
    {
        public AveragingConstructor constructor { get; set; }

        public TwoDepthAveragingConstructor(AveragingConstructor constructor)
        {
            this.constructor = constructor;
        }

        // Average(1/2k) - Average(k)
        public override double average(IValue[] values, int start, int depth)
        {
            double fast = constructor.average(values, start, depth / 2);
            double slow = constructor.average(values, start, depth);

            return 2 * fast - slow;
        }
    }
}
