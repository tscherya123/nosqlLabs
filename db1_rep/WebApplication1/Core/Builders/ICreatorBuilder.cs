using Core.Models;

namespace Core.Builders
{
    public interface ICreatorBuilder : IBuilder<Creator>
    {
        ICreatorBuilder AddName(string name);
    }
}
