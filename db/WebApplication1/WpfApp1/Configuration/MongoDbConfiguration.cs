using Data.Settings;

namespace WpfApp1.Configuration
{
    public static class MongoDbConfiguration
    {
        private const string URL = "localhost";
        private const string Port = "27017";

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
