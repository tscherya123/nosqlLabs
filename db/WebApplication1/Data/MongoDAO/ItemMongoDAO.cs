using Core.Models;
using Core.Observers;
using Data.Connections;
using Data.Constants;
using Data.DAO;
using Data.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Data.MongoDAO
{
    public class ItemMongoDAO : IItemDAO
    {
        private static ItemMongoDAO itemDAO;

        private readonly MongoClient connection;
        private readonly List<IObserver> observers;
        private readonly IMongoCollection<ItemMongoModel> collection;
        private readonly IMongoCollection<ItemCreatorMongoModel> itemCreatorCollection;
        private readonly IMongoCollection<Creator> creatorsCollection;
        private readonly IMongoCollection<Info> infosCollection;

        private ItemMongoDAO()
        {
        }

        private ItemMongoDAO(MongoDbConnection connection)
        {
            this.connection = connection.Connection;
            observers = new List<IObserver>();

            var db = this.connection.GetDatabase(MongoDbConnectionConstants.DbName);
            collection = db.GetCollection<ItemMongoModel>(MongoDbConnectionConstants.ItemsCollection);
            itemCreatorCollection = db.GetCollection<ItemCreatorMongoModel>(MongoDbConnectionConstants.ItemCreatorCollection);
            creatorsCollection = db.GetCollection<Creator>(MongoDbConnectionConstants.CreatorsCollectionName);
            infosCollection = db.GetCollection<Info>(MongoDbConnectionConstants.InfosCollectionName);
        }

        public static ItemMongoDAO GetInstance(MongoDbConnection connection)
        {
            return itemDAO ?? (itemDAO = new ItemMongoDAO(connection));
        }

        public void Add(Item entity)
        {
            var item = new ItemMongoModel()
            {
                Info = entity.Info.Id
            };
            var creators = entity.Creators.Select(c => new ItemCreatorMongoModel()
            {
                Creator = c.Id
            }).ToList();

            try
            {
                var lastItem = collection.Aggregate().SortByDescending(c => c.Id).FirstOrDefault();
                item.Id = lastItem != null ? lastItem.Id + 1 : 1;
                creators.ForEach(c => c.Item = item.Id);

                collection.InsertOne(item);
                itemCreatorCollection.InsertMany(creators);
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
                itemCreatorCollection.DeleteMany(i => i.Item == id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<Item> Get(int? id, Info info, Creator creator)
        {
            var items = new List<Item>();


            try
            {
                var infos = infosCollection.Find(i => true).ToList();
                var creators = creatorsCollection.Find(c => true).ToList();
                var itemCreators = itemCreatorCollection.Find(c => true).ToList();

                collection.Find(c => id == null || c.Id == id).ToList().ForEach(c => items.Add(new Item()
                {
                    Id = c.Id,
                    Info = infos.FirstOrDefault(i => i.Id == c.Info)
                }));

                if (info != null)
                {
                    items = items.Where(c => c.Info.Id == info.Id).ToList();
                }

                items.ForEach(c => 
                c.Creators = creators.Where(r => itemCreators.Where(q => q.Item == c.Id).FirstOrDefault(w => w.Creator == r.Id) != null).ToList());

                if (creator != null)
                {
                    items = items.Where(c => c.Creators.Contains(creator)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return items;
        }

        public IList<Item> GetAll()
        {
            var items = new List<Item>();

            try
            {
                var infos = infosCollection.Find(i => true).ToList();
                var creators = creatorsCollection.Find(c => true).ToList();
                var itemCreators = itemCreatorCollection.Find(c => true).ToList();

                items = collection.Find(c => true).ToList().Select(c => new Item()
                {
                    Id = c.Id,
                    Info = infos.FirstOrDefault(i => i.Id == c.Info),
                    Creators = creators.Where(r => itemCreators.Where(q => q.Item == c.Id).FirstOrDefault(w => w.Creator == r.Id) != null).ToList()
                }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return items;
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

        public void Update(int id, Item entity)
        {
            try
            {
                collection.UpdateOne(c => c.Id == id,
                    Builders<ItemMongoModel>.Update.Set(c => c.Info, entity.Info.Id));

                itemCreatorCollection.DeleteMany(c => c.Item == id);
                itemCreatorCollection.InsertMany(entity.Creators.Select(c => new ItemCreatorMongoModel()
                {
                    Item = id,
                    Creator = c.Id
                }));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
