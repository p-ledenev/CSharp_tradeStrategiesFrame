using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using tradeStrategiesFrame.DecisionMakingStrategies;
using tradeStrategiesFrame.Factories;

namespace tradeStrategiesFrame.Model
{
    class Machine
    {
        public double currentMoney { get; set; }
        public double maxMoney { get; set; }
        public bool isTrade { get; set; }
        public Position currentPosition { get; set; }
        public List<Trade> trades { get; set; }
        public DecisionStrategy strategy { get; set; }
        public int minDepth { get; set; }
        public int aver { get; set; }
        public double topR2 { get; set; }
        public Portfolio portfolio { get; set; }

        public Machine(double currentMoney, int aver, int minDepth, double topR2, Portfolio portfolio)
        {
            this.currentMoney = currentMoney;
            this.maxMoney = currentMoney;
            this.aver = aver;

            this.minDepth = minDepth;
            this.topR2 = topR2;

            isTrade = true;
            this.portfolio = portfolio;

            trades = new List<Trade> { Trade.createEmpty() };
            currentPosition = new Position();

            strategy = DecisionStrategieFactory.createDecisionStrategie(this);
            strategy.readParamsFrom(null);
        }

        public Trade getLastTrade()
        {
            return trades.Last();
        }

        public double getCandleValueFor(int start)
        {
            return portfolio.getCandleValueFor(start);
        }

        public Trade getLastOpenPositionTrade()
        {
            for (int i = trades.Count - 1; i >= 0; i--)
                if (trades[i].isOpenPosition)
                    return trades[i];

            return Trade.createEmpty();
        }

        public double countStock()
        {
            if (currentPosition.isNone())
                return currentMoney;

            return currentMoney + currentPosition.computeSignedValue();
        }

        protected double calculateOpenComission(Candle candle, int volume)
        {
            return 2 * volume * portfolio.comission;
        }

        protected double calculateCloseComission(Candle candle, int volume)
        {
            if (getLastTrade().inTradeDay(candle.date))
                return 0;

            return 2 * volume * portfolio.comission;
        }

        private void closePosition(Candle candle)
        {
            if (currentPosition.isEmpty())
                return;

            currentMoney += currentPosition.computeSignedValue(candle.tradeValue) - calculateCloseComission(candle, currentPosition.volume);
            currentPosition = new Position();
        }

        private void openPosition(Candle candle, Position.Direction direction)
        {
            if (!currentPosition.isEmpty())
                return;

            int volume = (int)Math.Floor(currentMoney / candle.tradeValue);
            currentPosition = new Position(candle.tradeValue, direction, volume);

            currentMoney -= currentPosition.computeSignedValue() - calculateOpenComission(candle, volume);
        }

        public void operateStock(TradeSignal signal, int start)
        {
            Trade lastTrade = getLastTrade();

            if (signal.isNoneDirection())
                return;

            if (currentPosition.isSameDirectionAs(signal.direction))
                return;

            if (currentPosition.isEmpty() && signal.isClosePosition())
                return;

            Candle candle = portfolio.candles[start];

            closePosition(candle);
            trades.Add(new Trade(candle.date, candle.dateIndex, candle.tradeValue, signal.direction, currentPosition.volume, false));

            if (signal.isCloseAndOpenPosition())
            {
                openPosition(candle, signal.direction);
                trades.Add(new Trade(candle.date, candle.dateIndex, candle.tradeValue, signal.direction, currentPosition.volume, true));
            }

            portfolio.addAverageMoneyValue(candle.date, candle.dateIndex);
        }

        public void trade(int start, bool onlyCalculate)
        {
            TradeSignal signal = strategy.tradeSignalFor(start);

            if (!onlyCalculate)
                operateStock(signal, start);

            if (ArrayCount.isDayChanged(portfolio.candles, start))
                Console.WriteLine(portfolio.ticket + ": minDepth: " + minDepth + "; topR2: " + topR2 + "; " +
                    portfolio.candles[start].printDescription() + " " + portfolio.candles[start].date + " " + DateTime.Now);
        }

        public void writePortfolioValues(String year)
        {
            List<String> collection = new List<String>();
            collection.Add("index|date|siftedValue|nonGap|" + Candle.printDescriptionHead() +
                           " |index|date|openBuy|openSell|closeBuy|closeSell|money");

            Trade[] arrTrades = trades.ToArray();

            int index = 0;
            foreach (Candle sifted in portfolio.candles)
            {
                String values = sifted.dateIndex + "|" + sifted.date.ToString("dd.MM.yyyy HH:mm:ss") + "|" +
                                Math.Round(sifted.value, 4) + "|" +
                                Math.Round(sifted.getValue(), 4) + "|" +
                                sifted.printDescription();

                String history = " |";
                if (index < arrTrades.Length)
                    history += arrTrades[index].tradeString();

                collection.Add(values + history);
                index++;
            }

            File.WriteAllLines("portfolioValues_" + portfolio.ticket + "_" + year + "_s" + topR2 + ".txt", collection);
        }
    }
}
