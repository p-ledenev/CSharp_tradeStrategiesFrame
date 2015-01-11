﻿using System;
using System.Collections.Generic;
using System.IO;
using tradeStrategiesFrame.Model;

namespace tradeStrategiesFrame.DataSources
{
    class CsvDataSource : DataSource
    {
        public override List<Candle> readCandlesFrom(String fileName)
        {
            List<Candle> candles = new List<Candle>();
            StreamReader reader = new StreamReader(fileName);

            String line = "";
            for (int i = 0; line != null; line = reader.ReadLine())
            {
                if (line == "") 
                    continue;

                String[] data = line.Split('|');
                candles.Add(createCandle(data, i));
            }

            reader.Close();

            return candles;
        }

        protected Candle createCandle(String[] data, int dateIndex)
        {
            DateTime date = DateTime.Parse(data[0]);
            double value = Double.Parse(data[4].Replace(".", ","));
            int volume = Int32.Parse(data[5].Replace(".", ","));

            return new Candle(value, volume, date, dateIndex);
        }
    }
}
