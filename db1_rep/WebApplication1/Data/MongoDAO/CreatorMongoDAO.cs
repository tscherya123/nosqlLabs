using Core.Models;
using Core.Observers;
using Data.Connections;
using Data.Constants;
using Data.DAO;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Data.MongoDAO
{
    public class CreatorMongoDAO : ICreatorDAO
    {
        private static CreatorMongoDAO creatorDAO;

        private readonly MongoClient connection;
        private readonly List<IObserver> observers;
        private readonly IMongoCollection<Creator> collection;

        private CreatorMongoDAO()
        {
        }

        private CreatorMongoDAO(MongoDbConnection connection)
        {
            this.connection = connection.Connection;
            observers = new List<IObserver>();

            var db = this.connection.GetDatabase(MongoDbConnectionConstants.DbName);
            collection = db.GetCollection<Creator>(MongoDbConnectionConstants.CreatorsCollectionName);
        }

        public static CreatorMongoDAO GetInstance(MongoDbConnection connection)
        {
            return creatorDAO ?? (creatorDAO = new CreatorMongoDAO(connection));
        }

        public void Add(Creator entity)
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

        private bool AddEntity(Creator entity)
        {
            try
            {
                var lastCreator = collection.Aggregate().SortByDescending(c => c.Id).FirstOrDefault();

                entity.Id = lastCreator != null ? lastCreator.Id + 1 : 1;

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

        public IList<Creator> Get(int? id, string name)
        {
            var creators = new List<Creator>();

            try
            {
                if (id != null && name == null)
                {
                    creators = collection.Find(c => c.Id == id).ToList();
                }
                else if (id == null && name != null)
                {
                    creators = collection.Find(c => c.Name == name).ToList();
                }
                else if (id != null && name != null)
                {
                    creators = collection.Find(c => c.Id == id && c.Name == name).ToList();
                }
                else
                {
                    creators = collection.Find(c => true).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return creators;
        }

        public IList<Creator> GetAll()
        {
            var creators = new List<Creator>();

            try
            {
                creators = collection.Find(c => true).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return creators;
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

        public void Update(int id, Creator entity)
        {
            try
            {
                collection.UpdateOne(c => c.Id == id, Builders<Creator>.Update.Set(c => c.Name, entity.Name));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
