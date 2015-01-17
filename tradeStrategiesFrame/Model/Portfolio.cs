using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using tradeStrategiesFrame.CommissionStrategies;
using tradeStrategiesFrame.DecisionMakingStrategies;
using tradeStrategiesFrame.Factories;
using tradeStrategiesFrame.SiftCanldesStrategies;

namespace tradeStrategiesFrame.Model
{
    class Portfolio
    {
        public String ticket { get; set; }
        public CommissionStrategy commissionStrategy { get; set; }
        public Candle[] candles { get; set; }
        public List<Machine> machines { get; set; }
        public List<Slice> averageMoney { get; set; }
        public String title { get; set; }

        public Portfolio(String ticket, CommissionStrategy commissionStrategy, Candle[] candles)
        {
            averageMoney = new List<Slice>();
            machines = new List<Machine>();

            this.ticket = ticket;
            this.commissionStrategy = commissionStrategy;
            this.candles = candles;
        }

        public void initMachines(String decisionStrategyName, int[] depths)
        {
            foreach (int depth in depths)
                machines.Add(new Machine(decisionStrategyName, 10000000, depth, this));

            title = machines[0].getDecisionStrategyName();
        }

        public void trade(String year)
        {
            for (int i = 0; i < candles.Length - 2; i++)
            {
                Candle candle = candles[i];

                if (isDayChanged(i))
                {
                    addAverageMoney(candle.date, candle.dateIndex);
                    flushResults(year);
                }

                bool onlyCalculate = (candle.date.Year.ToString() != year);

                foreach (Machine machine in machines)
                    machine.trade(i, onlyCalculate);
            }

            flushResults(year);
        }

        protected void flushResults(String year)
        {
            if (machines.Count > 1)
            {
                writeTradeResult(year);
                writeAverageMoney(year);
            }
            else
            {
                writeTradeDetailResult(year);
            }

            writeTradeSummaryResult(year);
        }

        public void addAverageMoney(DateTime dt, int index)
        {
            double averageValue = machines.Sum(machine => machine.computeCurrentMoney());
            double value = Math.Round(averageValue / machines.Count / 100000, 2);

            Slice slice = new Slice(dt, index, value);

            if (averageMoney.Count <= 0 || !slice.hasEqualDate(averageMoney.Last()))
                averageMoney.Add(slice);
        }

        public void writeAverageMoney(String year)
        {
            List<String> collection = averageMoney.Select(slice => slice.print()).ToList();

            File.WriteAllLines("averageMoney_" + ticket + "_" + year + "_" + title + ".txt", collection);
        }

        public void writeTradeResult(String year)
        {
            List<String> collection = new List<String>();

            String str = "";
            foreach (Machine machine in machines)
                str += machine.depth + "|date|money| |";

            collection.Add(str);

            int count = getMaxTradesNumber();
            for (int j = 0; j < count; j++)
            {
                str = "";
                foreach (Machine machine in machines)
                {
                    if (j < machine.trades.Count)
                        str += machine.trades.ElementAt(j).printPreview() + "| |";
                    else
                        str += " | | | |";
                }

                collection.Add(str);
            }

            File.WriteAllLines("machinesMoney_" + ticket + "_" + year + "_" + title + ".csv", collection);
        }

        public void writeTradeSummaryResult(String year)
        {
            List<String> collection = new List<String>();

            collection.Add(createSummaryStringFor(" ", "getDepth"));
            collection.Add(createSummaryStringFor("maxLoss", "computeMaxLoss"));
            collection.Add(createSummaryStringFor("maxMoney", "computeMaxMoney"));
            collection.Add(createSummaryStringFor("endPeriodMoney", "computeEndPeriodMoney"));

            File.WriteAllLines("machinesSummary_" + ticket + "_" + year + "_" + title + ".csv", collection);
        }

        protected String createSummaryStringFor(String title, String methodName)
        {
            String str = title + "|";

            MethodInfo methodInfo;
            foreach (Machine machine in machines)
            {
                methodInfo = machine.GetType().GetMethod(methodName);
                str += methodInfo.Invoke(machine, null) + "|";
            }

            str += "|";

            methodInfo = GetType().GetMethod(methodName);
            str += methodInfo.Invoke(this, null) + "|";

            return str;
        }

        public void writeTradeDetailResult(String year)
        {
            foreach (Machine machine in machines)
                machine.writeTradeResult(year);
        }

        private int getMaxTradesNumber()
        {
            int count = 0;
            foreach (Machine machine in machines)
                if (machine.trades.Count > count)
                    count = machine.trades.Count;

            return count;
        }

        public double countMeanCandleDeviation()
        {
            double mean = 0;
            for (int i = 1; i < candles.Length; i++)
                mean += Math.Abs(candles[i].value - candles[i - 1].value) / candles[i].value;

            return mean / candles.Length;
        }

        public double getCandleValueFor(int start)
        {
            return getCandleFor(start).getValue();
        }

        public Candle getCandleFor(int start)
        {
            return candles[start];
        }

        public bool isDayChanged(int start)
        {
            if (start == 0)
                return false;

            return (candles[start].date.Day != candles[start - 1].date.Day);
        }

        public double computeClosePositionCommission(CommissionRequest request)
        {
            return commissionStrategy.computeClosePositionCommission(request);
        }

        public double computeOpenPositionCommission(CommissionRequest request)
        {
            return commissionStrategy.computeOpenPositionCommission(request);
        }
    }
}
