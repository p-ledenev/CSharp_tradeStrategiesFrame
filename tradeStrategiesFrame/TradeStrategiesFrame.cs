using System;
using System.Collections.Generic;
using System.IO;
using tradeStrategiesFrame.CommissionStrategies;
using tradeStrategiesFrame.DecisionMakingStrategies;
using tradeStrategiesFrame.Factories;
using tradeStrategiesFrame.Model;
using tradeStrategiesFrame.SiftCanldesStrategies;

namespace tradeStrategiesFrame
{
    internal class TradeStrategiesFrame
    {
        public static List<Candle> readValuesFrom(String fName)
        {
            List<Candle> values = new List<Candle>();
            StreamReader reader = new StreamReader(fName);

            String str = "";
            for (int i = 0; str != null; str = reader.ReadLine())
            {
                if (str != "")
                {
                    String[] candle = str.Split(new char[] { '|' });
                    values.Add(new Candle(candle, i++));
                }
            }

            reader.Close();

            return values;
        }

        public static Candle[] sift(List<Candle> candles, double siftStep)
        {
            SiftCandlesStrategy siftStrategie = SiftCandlesStrategyFactory.createSiftStrategie(siftStep);
            List<Candle> sifted = siftStrategie.sift(candles);

            return sifted.ToArray();
        }

        public static void Main(string[] args)
        {
            String[] years = new String[] { "2014" }; // { "2010", "2011", "2012", "2013", "2014" };
            int[] depths = //new int[] { 50, 75, 100, 125, 150, 175, 200};
                // new int[] { 50, 75, 100, 125, 150, 175, 200, 225 };
                new int[] { 50, 75, 100, 125, 150, 175, 200, 225, 250, 275, 300, 325, 350, 375, 400, 425 };

            foreach (String year in years)
            {
                StreamReader reader = new StreamReader("tickets.txt");

                String strInitData;
                while ((strInitData = reader.ReadLine()) != null)
                {
                    String[] arrInitData = strInitData.Split(new char[1] { ';' });
                    List<Candle> candles = readValuesFrom("source\\forts\\" + year + "\\" + arrInitData[0] + "_1min_" + year + ".txt");

                    CommissionStrategy commissionStrategy = CommissionStrategyFactory.createConstantCommissionStrategie(Double.Parse(arrInitData[1]));
                    
                    Portfolio portfolio = new Portfolio(arrInitData[0], commissionStrategy, sift(candles, Double.Parse(arrInitData[2])));

                    portfolio.initMachines(depths);

                    portfolio.trade(year);
                }

                reader.Close();
            }
        }
    }
}