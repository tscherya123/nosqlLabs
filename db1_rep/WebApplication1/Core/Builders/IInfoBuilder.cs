using Core.Models;
using System;

namespace Core.Builders
{
    public interface IInfoBuilder : IBuilder<Info>
    {
        IInfoBuilder AddCreationDate(DateTime creationDate);

        IInfoBuilder AddName(string name);
    }
}
