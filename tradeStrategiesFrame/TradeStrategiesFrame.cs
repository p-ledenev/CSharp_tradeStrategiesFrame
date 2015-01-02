using System;
using System.Collections.Generic;
using System.IO;
using tradeStrategiesFrame.Model;

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
                    String[] candle = str.Split(new char[] {'|'});
                    values.Add(new Candle(candle, i++));
                }
            }

            reader.Close();

            return values;
        }

        public static void Main(string[] args)
        {
            String[] years = new String[] {"2014"}; // { "2010", "2011", "2012", "2013", "2014" };
            Double[] arrTopR2 = new Double[] {1.0};
            int[] arrDepth = //new int[] { 50, 75, 100, 125, 150, 175, 200};
                // new int[] { 50, 75, 100, 125, 150, 175, 200, 225 };
                new int[] {50, 75, 100, 125, 150, 175, 200, 225, 250, 275, 300, 325, 350, 375, 400, 425};

            GPortfolio gpft = new GPortfolio("dev");

            String ticket;
            foreach (String year in years)
            {
                StreamReader reader = new StreamReader("tickets.txt");

                while ((ticket = reader.ReadLine()) != null)
                {
                    String[] tickets = ticket.Split(new char[1] {';'});
                    List<Candle> values =
                        readValuesFrom("source\\forts\\" + year + "\\" + tickets[0] + "_1min_" + year + ".txt");

                    gpft.initPortfolios(values, tickets[0], Double.Parse(tickets[1]), Double.Parse(tickets[2]), arrDepth,
                        arrTopR2);

                    gpft.trade(year);
                }

                reader.Close();
            }
        }
    }
}