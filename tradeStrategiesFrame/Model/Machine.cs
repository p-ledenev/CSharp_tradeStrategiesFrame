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
        public List<Slice> averageMoney { get; set; }
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
                if (trades[i].isCloseAndOpenPosition())
                    return trades[i];

            return Trade.createEmpty();
        }

        public double computeCurrentMoney()
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

        private void closePosition(Candle candle, Position.Direction direction)
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

            currentMoney -= currentPosition.computeSignedValue() + calculateOpenComission(candle, volume);
        }

        public void operate(TradeSignal signal, int start)
        {
            if (signal.isNoneDirection())
                return;

            if (currentPosition.isSameDirectionAs(signal.direction))
                return;

            if (currentPosition.isEmpty() && signal.isClosePosition())
                return;

            Candle candle = portfolio.candles[start];

            int closeVolume = currentPosition.volume;
            closePosition(candle, signal.direction);

            if (signal.isCloseAndOpenPosition())
                openPosition(candle, signal.direction);

            trades.Add(new Trade(candle.date, candle.dateIndex, candle.tradeValue, signal.direction, currentPosition.volume + closeVolume, signal.mode));

            averageMoney.Add(new Slice(candle.date, candle.dateIndex,computeCurrentMoney()));

            portfolio.addAverageMoney(candle.date, candle.dateIndex);
        }

        public void trade(int start, bool onlyCalculate)
        {
            TradeSignal signal = strategy.tradeSignalFor(start);

            if (!onlyCalculate)
                operate(signal, start);

            if (portfolio.isDayChanged(start))
                Console.WriteLine(portfolio.ticket + ": minDepth: " + minDepth + "; topR2: " + topR2 + "; " +
                    portfolio.candles[start].printDescription() + " " + portfolio.candles[start].date + " " + DateTime.Now);
        }

        public void writeTradeResult(String year)
        {
            List<String> collection = new List<String>
            {
                "dateIndex|date|candleValue|" + portfolio.getCandleFor(0).printDescriptionHead() +
                " |dateIndex|date|openBuy|openSell|closeBuy|closeSell|" +
                " |dateIndex|date|averageMoney|"
            };

            Trade[] arrTrades = trades.ToArray();
            Slice[] arrMoney = averageMoney.ToArray();

            int index = 0;
            foreach (Candle candle in portfolio.candles)
            {
                String values = candle.dateIndex + "|" + candle.date.ToString("dd.MM.yyyy HH:mm:ss") + "|" +
                                Math.Round(candle.value, 4) + "|" +
                                candle.printDescription();

                String tradesHistory = " |";
                if (index < arrTrades.Length)
                    tradesHistory += arrTrades[index].print();

                String moneyHistory = " |";
                if (index < arrTrades.Length)
                    moneyHistory += arrMoney[index].print();

                collection.Add(values + tradesHistory + moneyHistory);
                index++;
            }

            File.WriteAllLines("portfolioValues_" + portfolio.ticket + "_" + year + "_s" + topR2 + ".txt", collection);
        }

        public Candle[] getCandles()
        {
            return portfolio.candles;
        }

        public int getCandlesLength()
        {
            return portfolio.candles.Length;
        }

        public void addCandleRequisite(String key, String value, int start)
        {
            portfolio.getCandleFor(start).addRequisite(key, value);
        }
    }
}
