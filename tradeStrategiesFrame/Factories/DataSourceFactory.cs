using tradeStrategiesFrame.DataSources;

namespace tradeStrategiesFrame.Factories
{
    class DataSourceFactory
    {
        public static DataSource createDataSource()
        {
            return createCsvDataSource();
        }

        protected static DataSource createCsvDataSource()
        {
            return new CsvDataSource();
        }
    }
}
