using System.Collections.Generic;

namespace Core.Models
{
    public class Item
    {
        public int Id { get; set; }

        public IList<Creator> Creators { get; set; }

        public Info Info { get; set; }
    }
}
