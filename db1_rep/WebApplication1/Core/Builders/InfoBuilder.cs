using Core.Models;
using System;

namespace Core.Builders
{
    public class InfoBuilder : IInfoBuilder
    {
        private readonly Info info;

        public InfoBuilder() : this(new Info())
        {
        }

        public InfoBuilder(Info info)
        {
            this.info = info;
        }

        public IInfoBuilder AddCreationDate(DateTime creationDate)
        {
            info.CreatonDate = creationDate;

            return this;
        }

        public IInfoBuilder AddName(string name)
        {
            info.Name = name;

            return this;
        }

        public Info Build()
        {
            return info;
        }
    }
}
