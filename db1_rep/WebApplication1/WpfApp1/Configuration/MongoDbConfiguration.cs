using Data.Settings;

namespace WpfApp1.Configuration
{
    public static class MongoDbConfiguration
    {
        private const string URL1 = "localhost";
        private const string Port1 = "27001";
        private const string URL2 = "localhost";
        private const string Port2 = "27002";
        private const string URL3 = "localhost";
        private const string Port3 = "27003";

        public static MongoDbConnectionSettings Settings
        {
            get
            {
                return new MongoDbConnectionSettings()
                {
                    URL1 = URL1,
                    Port1 = Port1,
                    URL2 = URL2,
                    Port2 = Port2,
                    URL3 = URL3,
                    Port3 = Port3,
                };
            }
        }
    }
}
