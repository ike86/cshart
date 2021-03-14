using System;
using DotNetGraph.Core;
using DotNetGraph.SubGraph;

namespace Cshart
{
    public interface IEdgeAddingStrategy
    {
        void AddEdges(DotSubGraph assemblyGraph, Type type, IDotElement typeNode);
    }
}
