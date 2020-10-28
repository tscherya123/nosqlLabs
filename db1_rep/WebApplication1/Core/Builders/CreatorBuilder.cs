using Core.Models;

namespace Core.Builders
{
    public class CreatorBuilder : ICreatorBuilder
    {
        private readonly Creator creator;

        public CreatorBuilder() : this(new Creator())
        {
        }

        public CreatorBuilder(Creator creator)
        {
            this.creator = creator;
        }

        public ICreatorBuilder AddName(string name)
        {
            creator.Name = name;

            return this;
        }

        public Creator Build()
        {
            return creator;
        }
    }
}
