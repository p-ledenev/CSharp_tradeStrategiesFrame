using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using tradeStrategiesFrame.DecisionMakingStrategies;
using tradeStrategiesFrame.Factories;

namespace tradeStrategiesFrame.Model
{
    class Portfolio
    {
        public double moneyValue { get; set; }
        public double moneyMax { get; set; }

        public bool isTrade { get; set; }
        public Stock heap { get; set; }

        public List<Trade> trades { get; set; }

        public DecisionStrategy strategie { get; set; }

        public int minDepth { get; set; }
        public int aver { get; set; }
        public double topR2 { get; set; }

        public GPortfolio parent { get; set; }

        public Portfolio(double moneyValue, int aver, int minDepth, double topR2, GPortfolio parent)
        {

            this.moneyValue = moneyValue;
            this.moneyMax = moneyValue;
            this.aver = aver;

            this.minDepth = minDepth;
            this.topR2 = topR2;

            isTrade = true;
            this.parent = parent;

            heap = new Stock();
            trades = new List<Trade>();

            strategie = DecisionStrategieFactory.createDecisionStrategie(this);
            strategie.readParamsFrom(null);
        }

        public Trade getLastTrade()
        {
            if (trades == null || trades.Count() <= 0)
                return new Trade(new DateTime(), 0, 0, 0, Stock.none, Trade.closePos, 0);

            return trades.Last();
        }

        public double getRearValue(int start)
        {
            return parent.siftedValues[start].nonGapValue;
        }

        public Trade getLastOpenPositionTrade()
        {
            if (trades == null)
                return new Trade(new DateTime(), 0, 0, 0, Stock.none, Trade.closePos, 0);

            for (int i = trades.Count - 1; i >= 0; i--)
                if (Trade.openPos.Equals(trades[i].position))
                    return trades[i];

            return new Trade(new DateTime(), 0, 0, 0, Stock.none, Trade.closePos, 0);
        }

        public double countStock(double value)
        {
            int sgn = 0;
            if (heap.mode == Stock.buy) sgn = 1;
            if (heap.mode == Stock.sell) sgn = -1;

            return moneyValue + sgn * value * heap.volume;
        }

        public double countStock()
        {
            int sgn = 0;
            if (heap.mode == Stock.buy) sgn = 1;
            if (heap.mode == Stock.sell) sgn = -1;

            return moneyValue + sgn * heap.buyValue * heap.volume;
        }

        protected double calculateOpenComission(Candle candle, int volume)
        {
            return 2 * volume * parent.comissionValue;
        }

        protected double calculateCloseComission(Candle candle, int volume)
        {
            if (getLastTrade().inTradeDay(candle.dt))
                return 0;

            return 2 * volume * parent.comissionValue;
        }

        private void closeStock(Candle candle)
        {
            if (heap.volume <= 0 || heap.buyValue <= 0 || heap.mode == Stock.none) return;

            int sgn = (heap.mode == Stock.buy) ? -1 : 1;

            moneyValue = moneyValue - sgn * heap.volume * candle.nextValue - calculateCloseComission(candle, heap.volume);
            heap.clearStock();
        }

        private void openStock(Candle candle, String mode)
        {
            if (heap.volume > 0 || heap.buyValue > 0 || heap.mode != Stock.none) return;

            int sgn = (mode == Stock.buy) ? 1 : -1;
            int volume = (int)Math.Floor(moneyValue / candle.nextValue);

            moneyValue = moneyValue - sgn * volume * candle.nextValue - calculateOpenComission(candle, volume);
            heap.setStock(candle.dt, candle.index, candle.nextValue, mode, volume);
        }

        public void operateStock(String cross, String position, int start)
        {
            if (heap.mode == cross || cross == Stock.none) return;

            if (heap.mode == Stock.none && position == Trade.closePos) return;

            Candle cn = parent.siftedValues[start];

            int vlm = heap.volume;
            closeStock(cn);

            if (position == Trade.openPos)
                openStock(cn, cross);

            Trade trade = new Trade(cn.dt, cn.timeOrder, cn.nextValue, countStock(), cross, position, vlm + heap.volume);
            trades.Add(trade);

            parent.addAverageMoneyValue(cn.dt, cn.timeOrder);
        }

        public void operateClose(int start)
        {
            if (heap.mode == Stock.none) return;

            String cross = (heap.mode == Stock.buy) ? Stock.sell : Stock.buy;

            operateStock(cross, Trade.closePos, start);
        }

        public void trade(int start, bool onlyCalculate)
        {
            TradeSignal signal = strategie.tradeSignalFor(start);

            if (!onlyCalculate)
                operateStock(signal.cross, signal.position, start);

            if (ArrayCount.isDayChanged(parent.siftedValues, start))
                Console.WriteLine(parent.ticket + ": minDepth: " + minDepth + "; topR2: " + topR2 + "; " +
                    parent.siftedValues[start].printDescription() + " " + parent.siftedValues[start].dt + " " + DateTime.Now);
        }

        public void printPortfolio(String fName)
        {
            String str = parent.ticket + ":\n moneyValue: " + moneyValue;
            Console.WriteLine(str);

            if (fName != null)
                File.AppendAllText(fName, str);

            heap.printStock(fName);
        }

        public void writePortfolioValues(String year)
        {
            List<String> collection = new List<String>();
            collection.Add("index|date|siftedValue|nonGap|" + Candle.printDescriptionHead() +
                           " |index|date|openBuy|openSell|closeBuy|closeSell|money");

            Trade[] arrTrades = trades.ToArray();

            int index = 0;
            foreach (Candle sifted in parent.siftedValues)
            {
                String values = sifted.timeOrder + "|" + sifted.dt.ToString("dd.MM.yyyy HH:mm:ss") + "|" +
                                Math.Round(sifted.value, 4) + "|" +
                                Math.Round(sifted.getValue(), 4) + "|" +
                                sifted.printDescription();

                String history = " |";
                if (index < arrTrades.Length)
                    history += arrTrades[index].tradeString();

                collection.Add(values + history);
                index++;
            }

            File.WriteAllLines("portfolioValues_" + parent.ticket + "_" + year + "_s" + topR2 + ".txt", collection);
        }

        public double getAver()
        {
            return minDepth;
        }

        public double calculateMaxLoss()
        {
            double loss = 0;
            for (int i = 0; i < trades.Count; i++)
            {
                for (int j = i; j < trades.Count; j++)
                {
                    double current = (trades[i].moneyValue - trades[j].moneyValue) / trades[i].moneyValue;
                    if (current > loss)
                        loss = current;
                }
            }

            return Math.Round(loss, 3);
        }

        public double calculateMaxMoneyValue()
        {
            double max = 0;
            foreach (Trade trade in trades)
                if (trade.moneyValue > max)
                    max = trade.moneyValue;

            return calculateRelativeMoneyValueFor(max);
        }

        public double calculateEndPeriodMoneyValue()
        {
            double currentMoneyValue = (trades.Count <= 0) ? moneyValue : trades.Last().moneyValue;

            return calculateRelativeMoneyValueFor(currentMoneyValue);
        }

        protected double calculateRelativeMoneyValueFor(double value)
        {
            double beginMoneyValue = (trades.Count <= 0) ? moneyValue : trades.First().moneyValue;

            return 100 + Math.Round((value - beginMoneyValue) / beginMoneyValue * 100, 2);
        }
    }
}
