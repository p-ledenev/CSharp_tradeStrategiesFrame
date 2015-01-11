using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using tradeStrategiesFrame.CommissionStrategies;
using tradeStrategiesFrame.DecisionMakingStrategies;
using tradeStrategiesFrame.Factories;

namespace tradeStrategiesFrame.Model
{
    class Machine
    {
        public double currentMoney { get; set; }
        public double maxMoney { get; set; }
        public bool isTrade { get; set; }
        public DecisionStrategy decisionStrategy { get; set; }
        public Position currentPosition { get; set; }
        public List<Trade> trades { get; set; }
        public List<Slice> averageMoney { get; set; }
        public int depth { get; set; }
        public Portfolio portfolio { get; set; }

        public Machine(String decisionStrategyName, double currentMoney, int depth, Portfolio portfolio)
        {
            this.currentMoney = currentMoney;
            this.maxMoney = currentMoney;
            this.depth = depth;

            isTrade = true;
            this.portfolio = portfolio;

            trades = new List<Trade> { Trade.createEmpty() };
            currentPosition = new Position();

            decisionStrategy = DecisionStrategyFactory.createDecisionStrategie(decisionStrategyName, this);
            decisionStrategy.readParamsFrom(null);
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

        private void closePosition(Candle candle, Position.Direction direction)
        {
            if (currentPosition.isEmpty())
                return;

            bool intraday = getLastOpenPositionTrade().isIntradayFor(candle.date);
            double commission = portfolio.computeClosePositionCommission(new CommissionRequest(currentPosition.tradeValue, currentPosition.volume, intraday));

            currentMoney += currentPosition.computeSignedValue(candle.tradeValue) - commission;

            currentPosition = new Position();
        }

        private void openPosition(Candle candle, Position.Direction direction)
        {
            if (!currentPosition.isEmpty())
                return;

            int volume = (int)Math.Floor(currentMoney / candle.tradeValue);
            currentPosition = new Position(candle.tradeValue, direction, volume);

            double commission = portfolio.computeOpenPositionCommission(new CommissionRequest(candle.tradeValue, volume, false));

            currentMoney -= currentPosition.computeSignedValue() + commission;
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

            averageMoney.Add(new Slice(candle.date, candle.dateIndex, computeCurrentMoney()));

            portfolio.addAverageMoney(candle.date, candle.dateIndex);
        }

        public void trade(int start, bool onlyCalculate)
        {
            TradeSignal signal = decisionStrategy.tradeSignalFor(start);

            if (!onlyCalculate)
                operate(signal, start);

            if (portfolio.isDayChanged(start))
                Console.WriteLine(portfolio.ticket + ": depth: " + depth + "; " +
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

            File.WriteAllLines("tradeResult_" + portfolio.ticket + "_" + year + "_" + portfolio.title + ".txt", collection);
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

        public String getDecisionStrategyName()
        {
            return decisionStrategy.getName();
        }

        // used via reflection
        public double getDepth()
        {
            return depth;
        }

        // used via reflection
        public double computeMaxLoss()
        {
            double loss = 0;
            for (int i = 0; i < averageMoney.Count; i++)
            {
                for (int j = i; j < averageMoney.Count; j++)
                {
                    double current = (averageMoney[i].value - averageMoney[j].value) / averageMoney[i].value;
                    if (current > loss)
                        loss = current;
                }
            }

            return Math.Round(loss, 3);
        }

        // used via reflection
        public double computeMaxMoney()
        {
            double max = 0;
            foreach (Slice slice in averageMoney)
                if (slice.value > max)
                    max = slice.value;

            return computeRelativeMoneyFor(max);
        }

        // used via reflection
        public double computeEndPeriodMoney()
        {
            double currentMoneyValue = (averageMoney.Count <= 0) ? currentMoney : averageMoney.Last().value;

            return computeRelativeMoneyFor(currentMoneyValue);
        }

        protected double computeRelativeMoneyFor(double value)
        {
            double beginMoneyValue = (averageMoney.Count <= 0) ? currentMoney : averageMoney.First().value;

            return 100 + Math.Round((value - beginMoneyValue) / beginMoneyValue * 100, 2);
        }
    }
}
