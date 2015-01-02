using System;
using System.IO;

namespace tradeStrategiesFrame.Model
{
    class Stock 
    {
        public DateTime dt { get; set; }
        public int index { get; set; }
        public double buyValue { get; set; }
        public String mode { get; set; }
        public int volume { get; set; }

        public static String buy = "Buy";
        public static String sell = "Sell";
        public static String none = "0";

        public Stock()
        {
            this.dt = DateTime.MinValue;
            buyValue = 0;
            mode = none;
            volume = 0;
            index = -1;
        }

        public Stock(DateTime dt, int index, double buyValue, String mode, int volume)
        {
            this.dt = dt;
            this.index = index;
            this.buyValue = buyValue;
            this.mode = mode;
            this.volume = volume;
        }

        public void setStock(DateTime dt, int index, double buyValue, String mode, int volume)
        {
            this.dt = dt;
            this.index = index;
            this.buyValue = buyValue;
            this.mode = mode;
            this.volume = volume;
        }

        public void clearStock()
        {
            this.dt = DateTime.MinValue;
            buyValue = 0;
            mode = none;
            volume = 0;
            index = -1;
        }

        public void printStock(String fName)
        {
            String str = mode + " " + index + " " + dt.ToString("dd.MM.yyyy HH:mm:ss") + " " + buyValue + " " + volume;
            Console.WriteLine(str);
            
            if (fName != null)
            {
                File.AppendAllText(fName, str);
            }
        }
    }
}
