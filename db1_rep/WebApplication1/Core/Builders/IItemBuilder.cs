using Core.Models;
using System.Collections.Generic;

namespace Core.Builders
{
    public interface IItemBuilder : IBuilder<Item>
    {
        IItemBuilder AddCreator(Creator creator);

        IItemBuilder AddCreators(IList<Creator> creators);

        IItemBuilder AddInfo(Info info);
    }
}
