using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using tradeStrategiesFrame.Factories;
using tradeStrategiesFrame.SiftValuesStrategies;

namespace tradeStrategiesFrame.Model
{
    class Portfolio
    {
        public String ticket { get; set; }
        public double comission { get; set; }
        public double historyParam { get; set; }
        public Candle[] candles { get; set; }
        public Machine[] machines { get; set; }
        public List<Slice> averageMoney { get; set; }

        public String suffix { get; set; }

        public Portfolio(String suffix)
        {
            averageMoney = new List<Slice>();

            this.suffix = suffix;
        }

        public void initPortfolios(List<Candle> candles, String ticket, double comission, double historyParam, int[] arrAver, double[] arrTopR2)
        {
            this.ticket = ticket;
            this.comission = comission;
            this.historyParam = historyParam;

            sift(candles);

            int machineAmount = arrAver.Length * arrTopR2.Length;
            machines = new Machine[machineAmount];

            for (int i = 0; i < arrTopR2.Length; i++)
            {
                for (int j = 0; j < arrAver.Length; j++)
                {
                    Machine pft = new Machine(10000000, 500, arrAver[j], arrTopR2[i], this);
                    machines[i * arrAver.Length + j] = pft;
                }
            }
        }

        public void trade(String year)
        {
            for (int i = 0; i < candles.Length - 2; i++)
            {
                Candle candle = candles[i];

                if (ArrayCount.countGap(candles, i) != 0)
                {
                    addAverageMoneyValue(candle.date, candle.dateIndex);
                    flushResults(year);
                }

                bool onlyCalculate = (candle.date.Year.ToString() != year);

                foreach (Machine pft in machines)
                    pft.trade(i, onlyCalculate);
            }

            flushResults(year);
        }

        protected void flushResults(String year)
        {
            if (machines.Length > 1)
            {
                writeProtfoliosMoneys(year);
                writeAverageMoneyValue(year);
            }
            else
            {
                writePortfolioValues(year);
            }

            writePortfoliosResume(year);
        }

        public void addAverageMoneyValue(DateTime dt, int index)
        {
            double averageValue = machines.Sum(machine => machine.countStock());
            double value = Math.Round(averageValue / machines.Length / 100000, 2);

            Slice slice = new Slice(dt, index, value);

            if (averageMoney.Count <= 0 || !slice.hasEqualDate(averageMoney.Last()))
                averageMoney.Add(slice);
        }

        public void writeAverageMoneyValue(String year)
        {
            List<String> collection = averageMoney.Select(slice => slice.print()).ToList();

            File.WriteAllLines("averageMoneys_" + ticket + "_" + year + "_" + suffix + ".txt", collection);
        }

        public void writeProtfoliosMoneys(String year)
        {
            List<String> collection = new List<String>();

            String str = "";
            foreach (Machine pft in machines)
            {
                str += "depth_" + pft.minDepth + " topR2_" + pft.topR2 + "|date|money| |";
            }
            collection.Add(str);

            int count = getPortfoliosTradesCount();
            for (int j = 0; j < count; j++)
            {
                str = "";
                foreach (Machine machine in machines)
                {
                    if (j < machine.trades.Count)
                        str += machine.trades.ElementAt(j).resumeString() + "| |";
                    else
                        str += " | | | |";
                }

                collection.Add(str);
            }

            File.WriteAllLines("pftsMoneys_" + ticket + "_" + year + "_" + suffix + ".txt", collection);
        }

        public void writePortfoliosResume(String year)
        {
            List<String> collection = new List<String>();

            collection.Add(createResumeStringFor(" ", "getAver"));
            collection.Add(createResumeStringFor("maxLoss", "calculateMaxLoss"));
            collection.Add(createResumeStringFor("maxMoney", "calculateMaxMoneyValue"));
            collection.Add(createResumeStringFor("endPeriodMoney", "calculateEndPeriodMoneyValue"));

            File.WriteAllLines("pftsResume_" + ticket + "_" + year + "_" + suffix + ".txt", collection);
        }

        protected String createResumeStringFor(String title, string methodName)
        {
            String str = title + "|";

            MethodInfo methodInfo;
            foreach (Machine pft in machines)
            {
                methodInfo = pft.GetType().GetMethod(methodName);
                str += methodInfo.Invoke(pft, null) + "|";
            }

            str += "|";

            methodInfo = GetType().GetMethod(methodName);
            str += methodInfo.Invoke(this, null) + "|";

            return str;
        }

        public void writePortfolioValues(String year)
        {
            foreach (Machine pft in machines)
                pft.writePortfolioValues(year);
        }

        private int getPortfoliosTradesCount()
        {
            int count = 0;
            foreach (Machine pft in machines)
                if (pft.trades.Count > count)
                    count = pft.trades.Count;

            return count;
        }

        public void sift(List<Candle> values)
        {
            SiftValuesStrategy siftStrategie = SiftValuesStrategieFactory.createSiftStrategie(historyParam);
            List<Candle> sifted = siftStrategie.sift(values);

            candles = sifted.ToArray();
        }

        public void countHistoryParam()
        {
            historyParam = countMeanHistoryDeviation();
        }

        private double countMeanHistoryDeviation()
        {
            double mean = 0;

            for (int i = 1; i < candles.Length; i++)
            {
                mean += Math.Abs(candles[i].value - candles[i - 1].value) / candles[i].value;
            }

            return mean / candles.Length;
        }

        public double getCandleValueFor(int start)
        {
            return candles[start].value;
        }
    }
}
