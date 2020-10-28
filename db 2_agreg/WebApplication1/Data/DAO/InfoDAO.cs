using Core.Models;
using Core.Observers;
using Data.Connections;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Data.DAO
{
    public class InfoDAO : IInfoDAO
    {
        private static InfoDAO infoDAO;

        private readonly SqlConnection connection;
        private readonly List<IObserver> observers;

        private InfoDAO()
        {
        }

        private InfoDAO(SqlServerConnection connection)
        {
            this.connection = connection.sqlConnection;
            observers = new List<IObserver>();
        }

        public static InfoDAO GetInstance(SqlServerConnection connection)
        {
            return infoDAO ?? (infoDAO = new InfoDAO(connection));
        }

        public IList<Info> Get(int? id, DateTime? creationDate, string name)
        {
            var infos = new List<Info>();
            var query = $"select Id, CreationDate, Name from Infos where";
            var hasFilter = false;

            if (id != null)
            {
                query += $" Id = {id}";
                hasFilter = true;
            }

            if (creationDate != null)
            {
                if (hasFilter)
                {
                    query += " and";
                }

                query += $" CreationDate = '{creationDate}'";
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
                return infos;
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
                        infos.Add(new Info()
                        {
                            Id = reader.GetInt32(0),
                            CreatonDate = reader.GetDateTime(1),
                            Name = reader.GetString(2)
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

            return infos;
        }

        public IList<Info> GetAll()
        {
            var infos = new List<Info>();
            var query = $"select Id, CreationDate, Name from Infos;";

            try
            {
                connection.Open();

                var command = new SqlCommand(query, connection);
                var reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var Id = reader.GetInt32(0);
                        var CreatonDate = reader.GetDateTime(1);
                        var Name = reader.GetString(2);

                        infos.Add(new Info()
                        {
                            Id = reader.GetInt32(0),
                            CreatonDate = reader.GetDateTime(1),
                            Name = reader.GetString(2)
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

            return infos;
        }

        public void Add(Info info)
        {
            var query = $"insert into Infos (CreationDate, Name) values ('{info.CreatonDate}', '{info.Name}');";
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

            Notify(info);
        }

        public void Delete(int id)
        {
            var query = $"delete from Infos where Id = {id};";
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

        public void Update(int id, Info info)
        {
            var query = $"update Infos set CreationDate = '{info.CreatonDate}', Name = '{info.Name}' where Id = {id};";
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

            Notify(info);
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
