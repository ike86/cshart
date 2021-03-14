using System;
using DotNetGraph.Node;

namespace Cshart
{
    public interface ITypeNodeAppender
    {
        void Append(Type type, DotNode typeNode);
    }
}
