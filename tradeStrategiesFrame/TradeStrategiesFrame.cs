﻿using System;
using System.Collections.Generic;
using System.IO;
using tradeStrategiesFrame.CommissionStrategies;
using tradeStrategiesFrame.DataSources;
using tradeStrategiesFrame.Factories;
using tradeStrategiesFrame.Model;
using tradeStrategiesFrame.Settings;
using tradeStrategiesFrame.SiftCanldesStrategies;

namespace tradeStrategiesFrame
{
    class TradeStrategiesFrame
    {
        public static Candle[] siftCandles(List<Candle> candles, double siftStep)
        {
            SiftCandlesStrategy siftStrategie = SiftCandlesStrategyFactory.createSiftStrategie(siftStep);
            List<Candle> sifted = siftStrategie.sift(candles);

            return sifted.ToArray();
        }

        public static List<Candle> readCandles(String fileName)
        {
            DataSource source = DataSourceFactory.createDataSource();

            return source.readCandlesFrom(fileName);
        }

        public static List<InitialSettings> readSettings()
        {
            List<InitialSettings> settings = new List<InitialSettings>();

            StreamReader reader = new StreamReader("settings.txt");

            String line;
            while ((line = reader.ReadLine()) != null)
                settings.Add(InitialSettings.createFrom(line));

            return settings;
        }

        public static void Main(string[] args)
        {
            List<InitialSettings> tradeSettings = readSettings();

            foreach (InitialSettings settings in tradeSettings)
            {
                foreach (String year in settings.years)
                {
                    List<Candle> data = readCandles("sources\\" + year + "\\" + settings.ticket + "_" + settings.timeFrame + ".txt");
                    Candle[] candles = siftCandles(data, settings.siftStep);

                    CommissionStrategy commissionStrategy = CommissionStrategyFactory.createConstantCommissionStrategy(settings.commission);

                    Portfolio portfolio = new Portfolio(settings.ticket, commissionStrategy, candles);

                    portfolio.initMachines(settings.decisionStrategyName, settings.depths);

                    portfolio.trade(year);
                }

            }
        }
    }
}