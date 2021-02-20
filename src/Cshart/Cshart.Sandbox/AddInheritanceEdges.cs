using System;
using DotNetGraph.Core;
using DotNetGraph.Edge;
using DotNetGraph.SubGraph;

namespace Cshart.Sandbox
{
    public class AddInheritanceEdges : IEdgeAddingStrategy
    {
        public void AddEdges(DotSubGraph assemblyGraph, Type type, IDotElement typeNode)
        {
            if (assemblyGraph.TryGetTypeNode(() => type.BaseType!) is { } baseTypeNode)
            {
                assemblyGraph.Elements.Add(
                    new DotEdge(typeNode, baseTypeNode) {Label = "inherits"});
            }
        }
    }
}
