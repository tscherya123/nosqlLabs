using MongoDB.Driver;
using System;

namespace Data.Connections
{
    public class MongoDbConnection
    {
        private static MongoDbConnection mongoDbConnection;

        public MongoClient Connection { get; private set; }

        private MongoDbConnection()
        {
        }

        private MongoDbConnection(string connectionString)
        {
            try
            {
                Connection = new MongoClient(connectionString);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static MongoDbConnection GetInstance(string connectionString)
        {
            return mongoDbConnection ?? (mongoDbConnection = new MongoDbConnection(connectionString));
        }
    }
}
