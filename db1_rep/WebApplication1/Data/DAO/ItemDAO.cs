using Core.Models;
using Core.Observers;
using Data.Connections;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Data.DAO
{
    public class ItemDAO : IItemDAO
    {
        private static ItemDAO itemDAO;

        private readonly SqlConnection connection;
        private readonly List<IObserver> observers;

        private ItemDAO()
        {
        }

        private ItemDAO(SqlServerConnection connection)
        {
            this.connection = connection.sqlConnection;
            observers = new List<IObserver>();
        }

        public static ItemDAO GetInstance(SqlServerConnection connection)
        {
            return itemDAO ?? (itemDAO = new ItemDAO(connection));
        }

        public IList<Item> Get(int? id, Info info, Creator creator)
        {
            var items = new List<Item>();
            var query1 = $"select  Items.Id, Infos.Id as InfoId, CreationDate, Name from Items FULL OUTER join Infos on Items.Info_id = Infos.Id;";
            var query2 = "select Creators.Id, Creators.Name from ItemCreator join Creators on Creators.Id = ItemCreator.Creator_id where ItemCreator.Item_id = {0};";

            try
            {
                connection.Open();

                var command = new SqlCommand(query1, connection);
                var reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (reader.IsDBNull(0))
                        {
                            continue;
                        }

                        var item = new Item();

                        item.Id = reader.GetInt32(0);
                        item.Info = new Info();

                        item.Info.Id = reader.GetInt32(1);
                        item.Info.CreatonDate = reader.GetDateTime(2);
                        item.Info.Name = reader.GetString(3);

                        items.Add(item);
                    }
                }

                connection.Close();

                for (var i = 0; i < items.Count; i++)
                {
                    var creators = new List<Creator>();
                    var command2 = new SqlCommand(string.Format(query2, items[i].Id), connection);

                    connection.Open();

                    using (var reader2 = command2.ExecuteReader())
                    {
                        while (reader2.Read())
                        {
                            creators.Add(new Creator()
                            {
                                Id = reader2.GetInt32(0),
                                Name = reader2.GetString(1)
                            });
                        }
                    }

                    items[i].Creators = creators;

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                connection.Close();
            }

            if (id != null)
            {
                items = items.Where(i => i.Id == id).ToList();
            }

            if (info != null)
            {
                items = items.Where(i => i.Info.Id == info.Id).ToList();
            }

            if (creator != null)
            {
                items = items.Where(i => i.Creators.FirstOrDefault(c => c.Id == creator.Id) != null).ToList();
            }

            return items;
        }

        public IList<Item> GetAll()
        {
            var items = new List<Item>();
            var query1 = $"select  Items.Id, Infos.Id as InfoId, CreationDate, Name from Items FULL OUTER join Infos on Items.Info_id = Infos.Id;";
            var query2 = "select Creators.Id, Creators.Name from ItemCreator join Creators on Creators.Id = ItemCreator.Creator_id where ItemCreator.Item_id = {0};";

            try
            {
                connection.Open();

                var command = new SqlCommand(query1, connection);
                var reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (reader.IsDBNull(0))
                        {
                            continue;
                        }

                        var item = new Item();

                        item.Id = reader.GetInt32(0);
                        item.Info = new Info();

                        item.Info.Id = reader.GetInt32(1);
                        item.Info.CreatonDate = reader.GetDateTime(2);
                        item.Info.Name = reader.GetString(3);

                        items.Add(item);
                    }
                }

                connection.Close();

                for (var i = 0; i < items.Count; i++)
                {
                    var creators = new List<Creator>();
                    var command2 = new SqlCommand(string.Format(query2, items[i].Id), connection);

                    connection.Open();

                    using (var reader2 = command2.ExecuteReader())
                    {
                        while (reader2.Read())
                        {
                            creators.Add(new Creator()
                            {
                                Id = reader2.GetInt32(0),
                                Name = reader2.GetString(1)
                            });
                        }
                    }

                    items[i].Creators = creators;

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                connection.Close();
            }

            return items;
        }

        public void Add(Item item)
        {
            var query1 = $"insert into Items (Info_Id) values ({item.Info.Id});SELECT SCOPE_IDENTITY();";
            var query2 = "insert into ItemCreator (Item_id, Creator_Id) values ({0}, {1});";
            SqlTransaction transaction = null;

            try
            {
                connection.Open();
                transaction = connection.BeginTransaction();
                var command = new SqlCommand(query1, connection, transaction);
                var id = (int)(decimal)command.ExecuteScalarAsync().Result;

                foreach (var creator in item.Creators)
                {
                    command = new SqlCommand(string.Format(query2, id, creator.Id), connection, transaction);
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception ex2)
                {
                }
            }
            finally
            {
                connection.Close();
            }

            Notify(item);
        }

        public void Delete(int id)
        {
            var query1 = $"delete from ItemCreator where Item_id = {id};";
            var query2 = $"delete from Items where Id = {id};";
            SqlTransaction transaction = null;

            try
            {
                connection.Open();
                transaction = connection.BeginTransaction();

                var command = new SqlCommand(query1, connection, transaction);
                command.ExecuteNonQuery();

                command = new SqlCommand(query2, connection, transaction);
                command.ExecuteNonQuery();

                transaction.Commit();
            }
            catch (Exception ex)
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception ex2)
                {
                }
            }
            finally
            {
                connection.Close();
            }

            Notify(id);
        }

        public void Update(int id, Item item)
        {
            var query1 = $"update Items set Info_id = {item.Info.Id} where Id = {id};";
            var query2 = $"delete from ItemCreator where Item_id = {id};";
            var query3 = "insert into ItemCreator (Item_id, Creator_Id) values ({0}, {1});";
            SqlTransaction transaction = null;

            try
            {
                connection.Open();
                transaction = connection.BeginTransaction();

                var command = new SqlCommand(query1, connection, transaction);
                command.ExecuteNonQuery();

                command = new SqlCommand(query2, connection, transaction);
                command.ExecuteNonQuery();

                foreach (var creator in item.Creators)
                {
                    command = new SqlCommand(string.Format(query3, id, creator.Id), connection, transaction);
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception ex2)
                {
                }
            }
            finally
            {
                connection.Close();
            }

            Notify(item);
        }

        public void AddObserver(IObserver observer)
        {
            observers.Add(observer);
        }

        public void RemoveObserver(IObserver observer)
        {
            observers.Remove(observer);
        }

        public void Notify(object obj)
        {
            foreach (var observer in observers)
            {
                observer.Handle(obj);
            }
        }
    }
}
