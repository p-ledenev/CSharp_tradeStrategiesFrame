using System;

namespace tradeStrategiesFrame.Model
{
    class TradeSignal
    {
        public enum Mode { Close, CloseAndOpen };

        public Position.Direction direction { set; get; }
        public TradeSignal.Mode mode { set; get; }

        public TradeSignal(Position.Direction direction, TradeSignal.Mode mode)
        {
            this.mode = mode;
            this.direction = direction;
        }

        public static TradeSignal forClosePosition(Position.Direction direction)
        {
            Position.Direction reverseDirection = Position.Direction.None;

            if (Position.Direction.Buy.Equals(direction)) reverseDirection = Position.Direction.Sell;
            if (Position.Direction.Sell.Equals(direction)) reverseDirection = Position.Direction.Buy;

            return new TradeSignal(reverseDirection, Mode.Close);
        }

        public static TradeSignal forCloseAndOpenPosition(Position.Direction direction)
        {
            return new TradeSignal(direction, Mode.CloseAndOpen);
        }

        public Boolean isNoneDirection()
        {
            return Position.Direction.None.Equals(direction);
        }

        public Boolean isClosePosition()
        {
            return Mode.Close.Equals(mode);
        }

        public Boolean isCloseAndOpenPosition()
        {
            return Mode.CloseAndOpen.Equals(mode);
        }
    }
}
