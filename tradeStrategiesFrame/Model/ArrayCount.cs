namespace tradeStrategiesFrame.Model
{
    class ArrayCount
    {
        public static Candle doubleAver(Machine portfolio, int start)
        {
            Candle[] values = portfolio.portfolio.candles;
            Candle candle = values[start].clone();

            if (start < portfolio.aver)
            {
                candle.value = 0;
                candle.volume = 0;
            }
            else
            {
                candle.value = (values[start - 2].value + 2 * values[start - 1].value + values[start].value) / 4;
                candle.volume = (values[start - 2].volume + 2 * values[start - 1].volume + values[start].volume) / 4;
            }

            return candle;
        }

        public static Candle directionMovement(Machine portfolio, int start)
        {
            Candle[] values = portfolio.portfolio.candles;
            Candle candle = values[start].clone();

            double path = 0;
            double dist = 0;

            for (int i = start; i >= 0; i--)
            {
                if (candle.relativeAbsSpread(values[i]) * 100 > 0)
                {
                    Candle current = values[i];
                    dist = candle.absoluteSpread(current);

                    for (int j = i; j <= start; j++)
                    {
                        if (current.relativeAbsSpread(values[j]) * 100 >= portfolio.portfolio.historyParam)
                        {
                            path += current.absoluteAbsSpread(values[j]);
                            current = values[j];
                        }
                    }
                    path += current.absoluteAbsSpread(values[start]);

                    i = -1;
                }
            }

            candle.value = (path != 0) ? dist / path : 0;

            return candle;
        }

        public static double countGap(Candle[] values, int start)
        {
            if (start == 0) return 0;

            if (isDayChanged(values, start))
                return values[start].value - values[start - 1].value;

            return 0;
        }

        public static bool isDayChanged(Candle[] values, int start)
        {
            if (start == 0)
                return false;

            return (values[start].date.Day != values[start - 1].date.Day);
        }
    }
}
