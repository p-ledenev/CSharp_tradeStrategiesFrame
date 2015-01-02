using tradeStrategiesFrame.Model;

namespace tradeStrategiesFrame.AveragingConstructors
{
    internal class SEMAAveragingConstructor : AveragingConstructor
    {
        protected double L;

        public SEMAAveragingConstructor()
        {
            L = 2.0;
        }

        public SEMAAveragingConstructor(double L)
        {
            this.L = L;
        }

        // fast L*F - (L-1)*MA average
        public override double average(IValue[] values, int start, int depth)
        {
            double series = 0;
            double summ = 0;

            for (int i = depth; i > 0; i--)
            {
                series += 1.0 / i;
                if (start - i + 1 >= 0)
                    summ += values[start - i + 1].getValue() * (series - (L - 1.0) / L);
            }

            if (summ != 0)
                return L / depth * summ;

            return 0;
        }
    }
}
