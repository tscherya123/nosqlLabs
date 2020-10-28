using Data.Settings;

namespace WpfApp1.Configuration
{
    public static class MongoDbConfiguration
    {
        private const string URL1 = "localhost";
        private const string Port1 = "27017";

        private const string URL2 = "localhost";
        private const string Port2 = "27017";

        private const string URL3 = "localhost";
        private const string Port3 = "27017";

        public static MongoDbConnectionSettings Settings
        {
            get
            {
                return new MongoDbConnectionSettings()
                {
                    URL = URL,
                    Port = Port
                };
            }
        }
    }
}
