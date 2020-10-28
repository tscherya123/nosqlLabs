using Data.Connections;
using Data.Providers;
using Data.Settings;
using MongoDB.Driver;
using System;

namespace Data.Factories
{
    public class ConnectionFactory
    {
        private readonly string connectionString;
        private readonly string mongoConnectionString;

        public ConnectionFactory(SqlServerConntectionSettings conntectionSettings = null, MongoDbConnectionSettings mongoSettings = null)
        {
            connectionString = new SqlServerConnectionStringProvider(conntectionSettings).GetConnectionString();
            mongoConnectionString = new MongoDbConnectionStringProvider(mongoSettings).GetConnectionString();
        }

        public SqlServerConnection GetConnection()
        {
            return SqlServerConnection.GetInstance(connectionString);
        }

        public MongoDbConnection GetMongoClient()
        {
            return MongoDbConnection.GetInstance(mongoConnectionString);
        }
    }
}
