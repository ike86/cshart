using System.Collections.Generic;

namespace Cshart
{
    public interface IGraphElement
    {
        string Id { get; }

        IEnumerable<IGraphElement> Children { get; }

        void Add(IGraphElement child);
    }
}
