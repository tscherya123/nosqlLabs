using Core.Models;
using Core.Observers;
using Data.Connections;
using Data.Constants;
using Data.DAO;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Data.MongoDAO
{
    public class InfoMongoDAO : IInfoDAO
    {
        private static InfoMongoDAO infoDAO;

        private readonly MongoClient connection;
        private readonly List<IObserver> observers;
        private readonly IMongoCollection<Info> collection;

        private InfoMongoDAO()
        {
        }

        private InfoMongoDAO(MongoDbConnection connection)
        {
            this.connection = connection.Connection;
            observers = new List<IObserver>();

            var db = this.connection.GetDatabase(MongoDbConnectionConstants.DbName);
            collection = db.GetCollection<Info>(MongoDbConnectionConstants.InfosCollectionName);
        }

        public static InfoMongoDAO GetInstance(MongoDbConnection connection)
        {
            return infoDAO ?? (infoDAO = new InfoMongoDAO(connection));
        }

        public void Add(Info entity)
        {
            for (int i = 0; i < 3; i++)
            {
                bool ok = AddEntity(entity);

                if (ok)
                {
                    break;
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
        }

        private bool AddEntity(Info entity)
        {
            try
            {
                var lastInfo = collection.Aggregate().SortByDescending(c => c.Id).FirstOrDefault();

                entity.Id = lastInfo != null ? lastInfo.Id + 1 : 1;

                collection.InsertOne(entity);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public void AddObserver(IObserver observer)
        {
            observers.Add(observer);
        }

        public void Delete(int id)
        {
            try
            {
                collection.DeleteOne(c => c.Id == id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<Info> Get(int? id, DateTime? creationDate, string name)
        {
            var infos = new List<Info>();

            try
            {
                infos = collection.Find(c => 
                (id == null || c.Id == id) && 
                (creationDate == null || c.CreatonDate == creationDate) &&
                (name == null || c.Name == name)).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return infos;
        }

        public IList<Info> GetAll()
        {
            var infos = new List<Info>();

            try
            {
                infos = collection.Find(i => true).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return infos;
        }

        public void Notify(object obj)
        {
            foreach (var observer in observers)
            {
                observer.Handle(obj);
            }
        }

        public void RemoveObserver(IObserver observer)
        {
            observers.Remove(observer);
        }

        public void Update(int id, Info entity)
        {
            try
            {
                collection.UpdateOne(c => c.Id == id, 
                    Builders<Info>.Update.Set(c => c.Name, entity.Name).Set(c => c.CreatonDate, entity.CreatonDate));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
