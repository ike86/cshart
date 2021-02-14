using System;
using DotNetGraph.Node;

namespace Cshart.Sandbox
{
    public interface ITypeNodeAppender
    {
        void Append(Type type, DotNode typeNode);
    }
}
