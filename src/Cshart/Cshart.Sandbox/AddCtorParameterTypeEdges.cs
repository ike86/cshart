using System;
using System.Reflection;
using DotNetGraph.Core;
using DotNetGraph.Edge;
using DotNetGraph.SubGraph;

namespace Cshart.Sandbox
{
    public class AddCtorParameterTypeEdges : IEdgeAddingStrategy
    {
        public void AddEdges(DotSubGraph assemblyGraph, Type type, IDotElement typeNode)
        {
            foreach (var ctor in type.GetConstructors(BindingFlags.Public | BindingFlags.Instance))
            {
                foreach (var param in ctor.TryGetParameters())
                {
                    if (assemblyGraph.TryGetTypeNode(() => param.ParameterType)
                        is { } paramTypeNode)
                    {
                        assemblyGraph.Elements.Add(
                            new DotEdge(typeNode, paramTypeNode) {Label = "ctor param"});
                    }
                }
            }
        }
    }
}
