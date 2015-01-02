using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using tradeStrategiesFrame.Factories;
using tradeStrategiesFrame.SiftValuesStrategies;

namespace tradeStrategiesFrame.Model
{
    class GPortfolio
    {
        public String ticket { get; set; }
        public double comissionValue { get; set; }
        public double historyParam { get; set; }
        public Candle[] siftedValues { get; set; }

        public Portfolio[] pfts { get; set; }
        public List<Stock> averMoneys { get; set; }

        public String suffix { get; set; }

        public GPortfolio(String suffix)
        {
            averMoneys = new List<Stock>();

            this.suffix = suffix;
        }

        public void initPortfolios(List<Candle> values, String ticket, double comissionValue, double historyParam, int[] arrAver, double[] arrTopR2)
        {
            this.ticket = ticket;
            this.comissionValue = comissionValue;
            this.historyParam = historyParam;
            setSiftValues(values);

            averMoneys = new List<Stock>();

            int N_AUTOS = arrAver.Length * arrTopR2.Length;
            pfts = new Portfolio[N_AUTOS];

            for (int i = 0; i < arrTopR2.Length; i++)
            {
                for (int j = 0; j < arrAver.Length; j++)
                {
                    Portfolio pft = new Portfolio(10000000, 500, arrAver[j], arrTopR2[i], this);
                    pfts[i * arrAver.Length + j] = pft;
                }
            }
        }

        public void trade(String year)
        {
            for (int i = 0; i < siftedValues.Length - 2; i++)
            {
                Candle candle = siftedValues[i];

                if (ArrayCount.countGap(siftedValues, i) != 0)
                {
                    addAverageMoneyValue(candle.dt, candle.timeOrder);
                    flushResults(year);
                }

                bool onlyCalculate = (candle.dt.Year.ToString() != year);

                foreach (Portfolio pft in pfts)
                    pft.trade(i, onlyCalculate);
            }

            flushResults(year);
        }

        protected void flushResults(String year)
        {
            if (pfts.Length > 1)
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
            Stock stock = new Stock();
            stock.dt = dt;
            stock.index = index;

            double averValue = 0.0;
            foreach (Portfolio pft in pfts)
                averValue += pft.countStock();

            stock.buyValue = Math.Round(averValue / pfts.Length / 100000, 2);

            if (averMoneys.Count <= 0 || stock.index != averMoneys.Last().index)
                averMoneys.Add(stock);
        }

        public void writeAverageMoneyValue(String year)
        {
            List<String> collection = new List<String>();

            foreach (Stock stock in averMoneys)
                collection.Add(stock.index + "|" + stock.dt.ToString("dd.MM.yyyy HH:mm:ss") + "|" + stock.buyValue);

            File.WriteAllLines("averageMoneys_" + ticket + "_" + year + "_" + suffix + ".txt", collection);
        }

        public void writeProtfoliosMoneys(String year)
        {
            List<String> collection = new List<String>();

            String str = "";
            foreach (Portfolio pft in pfts)
            {
                str += "depth_" + pft.minDepth + " topR2_" + pft.topR2 + "|date|money| |";
            }
            collection.Add(str);

            int count = getPortfoliosTradesCount();
            for (int j = 0; j < count; j++)
            {
                str = "";
                foreach (Portfolio pft in pfts)
                {
                    if (j < pft.trades.Count)
                    {
                        str += pft.trades.ElementAt(j).resumeString() + "| |";
                    }
                    else
                    {
                        str += " | | | |";
                    }
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
            foreach (Portfolio pft in pfts)
            {
                methodInfo = pft.GetType().GetMethod(methodName);
                str += methodInfo.Invoke(pft, null) + "|";
            }

            str += "|";

            methodInfo = GetType().GetMethod(methodName);
            str += methodInfo.Invoke(this, null) + "|";

            return str;
        }

        public String getAver()
        {
            return "aver";
        }

        public double calculateMaxLoss()
        {
            double loss = 0;
            for (int i = 0; i < averMoneys.Count; i++)
            {
                for (int j = i; j < averMoneys.Count; j++)
                {
                    double current = (averMoneys[i].buyValue - averMoneys[j].buyValue) / averMoneys[i].buyValue;
                    if (current > loss)
                        loss = current;
                }
            }

            return Math.Round(loss, 3);
        }

        public double calculateMaxMoneyValue()
        {
            double max = 0;
            foreach (Stock stock in averMoneys)
            {
                if (stock.buyValue > max)
                    max = stock.buyValue;
            }

            return max;
        }

        public double calculateEndPeriodMoneyValue()
        {
            return averMoneys.Last().buyValue;
        }

        public void writePortfolioValues(String year)
        {
            foreach (Portfolio pft in pfts)
                pft.writePortfolioValues(year);
        }

        private int getPortfoliosTradesCount()
        {
            int count = 0;
            foreach (Portfolio pft in pfts)
                if (pft.trades.Count > count)
                    count = pft.trades.Count;

            return count;
        }

        public void setSiftValues(List<Candle> values)
        {
            SiftValuesStrategy siftStrategie = SiftValuesStrategieFactory.createSiftStrategie(historyParam);
            List<Candle> sifted = siftStrategie.sift(values);

            siftedValues = sifted.ToArray();
        }

        public void countHistoryParam()
        {
            historyParam = countMeanHistoryDeviation();
        }

        private double countMeanHistoryDeviation()
        {
            double mean = 0;

            for (int i = 1; i < siftedValues.Length; i++)
            {
                mean += Math.Abs(siftedValues[i].value - siftedValues[i - 1].value) / siftedValues[i].value;
            }

            return mean / siftedValues.Length;
        }

        // delta is the difference beetwen two nearby candles
        public double countMaxDeltaFrom(int index, int count)
        {
            double maxValue = 0;
            for (int i = 0; i < count; i++)
            {
                double curr = Math.Abs(siftedValues[index - i].nonGapValue - siftedValues[index - i - 1].nonGapValue);
                if (curr > maxValue)
                    maxValue = curr;
            }

            return maxValue;
        }

        // delta is the difference beetwen two nearby candles
        public double countMeanDeltaFrom(int index, int count)
        {
            double meanValue = 0;
            for (int i = 0; i < count; i++)
                meanValue += Math.Abs(siftedValues[index - i].nonGapValue - siftedValues[index - i - 1].nonGapValue);

            return meanValue / (count - 1.0);
        }

        public double countAbsoluteMeanDeltaFrom(int index, int count)
        {
            double meanValue = 0;
            for (int i = 0; i < count; i++)
                meanValue += (Math.Abs(siftedValues[index - i].nonGapValue - siftedValues[index - i - 1].nonGapValue)) / siftedValues[index - i].nonGapValue;

            return meanValue / (count - 1.0);
        }
    }
}
