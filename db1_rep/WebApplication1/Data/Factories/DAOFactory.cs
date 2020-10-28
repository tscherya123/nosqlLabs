using Core.Models;
using Data.Connections;
using Data.DAO;
using Data.MongoDAO;
using System;

namespace Data.Factories
{
    public class DAOFactory
    {
        private readonly SqlServerConnection connection;
        private readonly MongoDbConnection mongoDbConnection;

        public DAOFactory(SqlServerConnection connection = null, MongoDbConnection mongoDbConnection = null)
        {
            this.connection = connection;
            this.mongoDbConnection = mongoDbConnection;
        }

        public T GetConnection<T>(bool isMongo)
        {
            object dao = default;

            if (isMongo)
            {
                if (typeof(T) == typeof(ICreatorDAO))
                {
                    dao = CreatorMongoDAO.GetInstance(mongoDbConnection);
                }
                else if (typeof(T) == typeof(IInfoDAO))
                {
                    dao = InfoMongoDAO.GetInstance(mongoDbConnection);
                }
                else if (typeof(T) == typeof(IItemDAO))
                {
                    dao = ItemMongoDAO.GetInstance(mongoDbConnection);
                }
            }
            else
            {
                if (typeof(T) == typeof(ICreatorDAO))
                {
                    dao = CreatorDAO.GetInstance(connection);
                }
                else if (typeof(T) == typeof(IInfoDAO))
                {
                    dao = InfoDAO.GetInstance(connection);
                }
                else if (typeof(T) == typeof(IItemDAO))
                {
                    dao = ItemDAO.GetInstance(connection);
                }
            }

            return (T)dao;
        }
    }
}
