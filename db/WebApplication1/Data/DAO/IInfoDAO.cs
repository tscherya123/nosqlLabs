using Core.Models;
using System;
using System.Collections.Generic;

namespace Data.DAO
{
    public interface IInfoDAO : IDAO<Info>
    {
        IList<Info> Get(int? id, DateTime? creationDate, string name);
    }
}
