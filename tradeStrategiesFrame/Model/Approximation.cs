using System;

namespace tradeStrategiesFrame.Model
{
    class Approximation
    {
        public double r2 { get; set; }
        public double[] k { get; set; }
        public int depth { get; set; }

        public Approximation() : base()
        {
            r2 = 0;
            k = new double[] { 0 };
            depth = 0;
        }

        public double countDeviation()
        {
            double dev = 0;

            if (k == null) return dev;

            for (int i = 0; i < k.Length - 1; i++)
            {
                dev += (k.Length - 1 - i) * k[i] * Math.Pow(depth - 1, k.Length - 2 - i);
            }

            return dev;
        }

        public double countFunctionFor()
        {
            return countFunctionFor(depth - 1);
        }

        public double countFunctionFor(int abscis)
        {
            double value = 0;

            if (k == null) return value;

            for (int i = 0; i < k.Length; i++)
            {
                value += k[i] * Math.Pow(abscis, k.Length - 1 - i);
            }

            return Math.Round(value);
        }

        public String printPowerFunc()
        {
            String str = "";

            if (k == null) return str;

            for (int i = 0; i < k.Length; i++)
                str += (k[i] > 0 ? "+" : "") + Math.Round(k[i], 4) + "x" + (k.Length - 1 - i);

            return str;
        }

        public double countResidual(Candle[] values, int start)
        {
            if (start - depth < 0)
                return 0;

            double residual = 0;

            for (int i = 0; i < depth; i++)
            {
                double Y = values[start - depth + 1 + i].nonGapValue;
                double Yapp = countFunctionFor(i);
                residual += Math.Abs(Y - Yapp) / Y;
            }

            return Math.Round(residual / depth * 100, 6);
        }
    }
}
