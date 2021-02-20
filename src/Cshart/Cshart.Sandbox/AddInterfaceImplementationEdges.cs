using System;
using DotNetGraph.Core;
using DotNetGraph.Edge;
using DotNetGraph.SubGraph;

namespace Cshart.Sandbox
{
    public class AddInterfaceImplementationEdges : IEdgeAddingStrategy
    {
        public void AddEdges(DotSubGraph assemblyGraph, Type type, IDotElement typeNode)
        {
            foreach (var i in type.GetInterfaces())
            {
                if (assemblyGraph.TryGetTypeNode(() => i) is { } interfaceTypeNode)
                {
                    assemblyGraph.Elements.Add(
                        new DotEdge(typeNode, interfaceTypeNode) {Label = "implements"});
                }
            }
        }
    }
}
