using Core.Models;
using System.Collections.Generic;

namespace Data.DAO
{
    public interface ICreatorDAO : IDAO<Creator>
    {
        IList<Creator> Get(int? id, string name);
    }
}
