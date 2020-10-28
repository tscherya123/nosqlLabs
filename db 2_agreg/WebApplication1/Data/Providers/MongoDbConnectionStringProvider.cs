using Data.Constants;
using Data.Settings;

namespace Data.Providers
{
    public class MongoDbConnectionStringProvider : IConnectionStringProvider
    {
        private readonly MongoDbConnectionSettings settigns;

        public MongoDbConnectionStringProvider(MongoDbConnectionSettings settigns)
        {
            this.settigns = settigns;
        }

        public string GetConnectionString()
        {
            return string.Format(MongoDbConnectionConstants.ConnectionString, settigns.URL, settigns.Port);
        }
    }
}
