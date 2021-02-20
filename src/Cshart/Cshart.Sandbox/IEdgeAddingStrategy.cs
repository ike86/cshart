using System;
using DotNetGraph.Core;
using DotNetGraph.SubGraph;

namespace Cshart.Sandbox
{
    public interface IEdgeAddingStrategy
    {
        void AddEdges(DotSubGraph assemblyGraph, Type type, IDotElement typeNode);
    }
}
