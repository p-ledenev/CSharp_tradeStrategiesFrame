using System;
using System.ComponentModel;
using System.IO;

namespace tradeStrategiesFrame.Model
{
    class Trade
    {
        public enum Mode { Close, CloseAndOpen };

        public Position position { get; set; }
        public DateTime date { get; set; }
        public int dateIndex { get; set; }
        public Mode mode { get; set; }

        public static Trade createEmpty()
        {
            return new Trade(new DateTime(), 0, 0, Position.Direction.None, 0, Mode.Close);
        }

        public Trade(DateTime date, int dateIndex, double tradeValue, Position.Direction direction, int volume, Mode mode)
        {
            position = new Position(tradeValue, direction, volume);

            this.date = date;
            this.dateIndex = dateIndex;
            this.mode = mode;
        }

        public String print()
        {
            String result = dateIndex+ "|" + date.ToString("dd.MM.yyyy HH:mm:ss");
            String operation = (position.isBuy()) ? position.tradeValue + "| " : " |" + position.tradeValue;

            result += (isCloseAndOpenPosition()) ? operation + " | " : " | |" + operation;

            return result + "|";
        }

        public String printPreview()
        {
            return dateIndex + "|" + date.ToString("dd.MM.yyyy HH:mm:ss") + "|";
        }

        public Double countSignedValue()
        {
            return position.computeSignedValue();
        }

        // Trade day: from 19.00 yesterday to 19.00 torday
        public Boolean inTradeDay(DateTime date)
        {
            if (date.Hour > 19)
                return (this.date.Day == date.Day && this.date.Hour > 19);

            if (this.date.Day == date.Day)
                return true;

            return (this.date.Day == date.Day - 1 && this.date.Hour > 19);
        }

        public Boolean isSameDirectionAs(Position.Direction direction)
        {
            return position.isSameDirectionAs(direction);
        }

        public Boolean isBuy()
        {
            return position.isBuy();
        }

        public Boolean isSell()
        {
            return position.isSell();
        }

        public int getVolume()
        {
            return position.volume;
        }

        public Position.Direction getDirection()
        {
            return position.direction;
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
