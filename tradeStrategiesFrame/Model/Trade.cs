using System;

namespace tradeStrategiesFrame.Model
{
    class Trade
    {
        public Stock stock { get; set; }
        public String position { get; set; }
        public double moneyValue { get; set; }

        public static String openPos = "Open";
        public static String closePos = "Close";

        public Trade(DateTime dt, int index, double buyValue, double moneyValue, String mode, String position, int volume)
        {
            stock = new Stock(dt, index, buyValue, mode, volume);

            this.position = position;
            this.moneyValue = moneyValue;
        }

        public String tradeString()
        {
            String str = stock.index + "|" + stock.dt.ToString("dd.MM.yyyy HH:mm:ss") + "|";
            String oprt = (stock.mode == Stock.buy) ? stock.buyValue + "| |" : " |" + stock.buyValue + "|";

            str += (position == Trade.openPos) ? oprt + " | |" : " | |" + oprt;

            return str + Math.Round(moneyValue / 100000, 2);
        }

        public String resumeString()
        {
            return stock.index + "|" + stock.dt.ToString("dd.MM.yyyy HH:mm:ss") + "|" + Math.Round(moneyValue / 100000, 2);
        }

        // Trade day: from 19.00 yesterday to 19.00 torday
        public Boolean inTradeDay(DateTime day)
        {
            if (day.Hour > 19)
                return (stock.dt.Day == day.Day && stock.dt.Hour > 19);

            if (stock.dt.Day == day.Day)
                return true;

            return (stock.dt.Day == day.Day - 1 && stock.dt.Hour > 19);
        }
    }
}
