using Core.Models;
using System;
using System.Collections.Generic;

namespace Data.DAO
{
    public interface IItemDAO : IDAO<Item>
    {
        IList<Item> Get(int? id, Info info, Creator creator);
    }
}
