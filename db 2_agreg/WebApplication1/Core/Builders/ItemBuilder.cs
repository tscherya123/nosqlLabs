using Core.Models;
using System.Collections.Generic;

namespace Core.Builders
{
    public class ItemBuilder : IItemBuilder
    {
        private readonly Item item;

        public ItemBuilder() : this(new Item())
        {
        }

        public ItemBuilder(Item item)
        {
            this.item = item;
        }

        public IItemBuilder AddCreator(Creator creator)
        {
            item.Creators.Add(creator);

            return this;
        }

        public IItemBuilder AddCreators(IList<Creator> creators)
        {
            foreach (var creator in creators)
            {
                item.Creators.Add(creator);
            }

            return this;
        }

        public IItemBuilder AddInfo(Info info)
        {
            item.Info = info;

            return this;
        }

        public Item Build()
        {
            return item;
        }
    }
}
