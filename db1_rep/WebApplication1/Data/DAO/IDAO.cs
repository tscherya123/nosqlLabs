using Core.Observers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.DAO
{
    public interface IDAO<T> : IObservable
    {
        IList<T> GetAll();

        void Add(T entity);

        void Update(int id, T entity);

        void Delete(int id);

        void MigrateTo(IDAO<T> dao)
        {
            var entities = GetAll();

            foreach (var entity in entities)
            {
                dao.Add(entity);
            }
        }
    }
}
