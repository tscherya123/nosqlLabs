using Core.Models;
using Core.Momento;
using Core.Observers;
using Data.Connections;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Data.DAO
{
    public class CreatorDAO : ICreatorDAO
    {
        private static CreatorDAO creatorDAO;

        private readonly SqlConnection connection;
        private readonly List<IObserver> observers;
        private readonly IDictionary<int, Stack<CareTaker>> careTakers;

        private CreatorDAO()
        {
        }

        private CreatorDAO(SqlServerConnection connection)
        {
            this.connection = connection.sqlConnection;
            observers = new List<IObserver>();
            careTakers = new Dictionary<int, Stack<CareTaker>>();
        }

        public static CreatorDAO GetInstance(SqlServerConnection connection)
        {
            return creatorDAO ?? (creatorDAO = new CreatorDAO(connection));
        }

        public IList<Creator> Get(int? id, string name)
        {
            var cretors = new List<Creator>();
            var query = $"select Id, Name from Creators where";
            var hasFilter = false;

            if (id != null)
            {
                query += $" Id = {id}";
                hasFilter = true;
            }

            if (name != null)
            {
                if (hasFilter)
                {
                    query += " and";
                }

                query += $" Name like '{name}'";
                hasFilter = true;
            }

            if (!hasFilter)
            {
                return cretors;
            }
            else
            {
                query += $";";
            }

            try
            {
                connection.Open();

                var command = new SqlCommand(query, connection);
                var reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        cretors.Add(new Creator()
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                connection.Close();
            }

            return cretors;
        }

        public IList<Creator> GetAll()
        {
            var creators = new List<Creator>();
            var query = $"select Id, Name from Creators;";


            try
            {
                connection.Open();

                var command = new SqlCommand(query, connection);
                var reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        creators.Add(new Creator()
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                connection.Close();
            }

            return creators;
        }

        public void Add(Creator creator)
        {
            var query = $"insert into Creators (Name) values ('{creator.Name}');";
            SqlTransaction transaction = null;

            try
            {
                connection.Open();
                transaction = connection.BeginTransaction();
                var command = new SqlCommand(query, connection, transaction);

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

            Notify(creator);
        }

        public void Delete(int id)
        {
            try
            {
                careTakers.Remove(id);
            }
            catch
            {
            }

            var query = $"delete from Creators where Id = {id};";
            SqlTransaction transaction = null;

            try
            {
                connection.Open();
                transaction = connection.BeginTransaction();
                var command = new SqlCommand(query, connection, transaction);

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

        public void Update(int id, Creator creator)
        {
            if (!AddMomento(id))
            {
                return;
            }

            UpdateCreator(id, creator);

            Notify(creator);
        }

        private void UpdateCreator(int id, Creator creator)
        {
            var query = $"update Creators set Name = '{creator.Name}' where Id = {id};";
            SqlTransaction transaction = null;

            try
            {
                connection.Open();
                transaction = connection.BeginTransaction();
                var command = new SqlCommand(query, connection, transaction);

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
        }

        private bool AddMomento(int id)
        {
            var oldCreator = Get(id, null).FirstOrDefault();

            if (oldCreator == null)
            {
                return false;
            }

            var careTaker = new CareTaker()
            {
                Momento = new Momento(oldCreator.Id, oldCreator.Name)
            };

            try
            {
                careTakers[id].Push(careTaker);
            }
            catch
            {
                careTakers.Add(id, new Stack<CareTaker>(new List<CareTaker>()
                {
                    careTaker
                }));
            }

            return true;
        }

        public void Restore(int id)
        {
            try
            {
                if (careTakers[id].Count > 0)
                {
                    var careTaker = careTakers[id].Pop();
                    var creator = new Creator();
                    creator.SetMomento(careTaker.Momento);

                    UpdateCreator(id, creator);

                    Notify(id);
                }
            }
            catch
            {
            }
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
