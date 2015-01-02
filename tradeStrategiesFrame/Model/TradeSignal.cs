using System;

namespace tradeStrategiesFrame.Model
{
    class TradeSignal
    {
        public String cross { set; get; }
        public String position { set; get; }

        public TradeSignal(String cross, String position)
        {
            this.cross = cross;
            this.position = position;
        }

        public static TradeSignal closePosition(String cross)
        {
            String reCross = Stock.none;

            if (Stock.buy.Equals(cross)) reCross = Stock.sell;
            if (Stock.sell.Equals(cross)) reCross = Stock.buy;

            return new TradeSignal(reCross, Trade.closePos);
        }

        public static TradeSignal openPosition(String cross)
        {
            return new TradeSignal(cross, Trade.openPos);
        }
    }
}
