using System;
using System.Collections.Generic;
using System.Linq;
using DotNetGraph.Core;
using DotNetGraph.Edge;
using DotNetGraph.SubGraph;

namespace Cshart.Sandbox
{
    public class AddInterfaceImplementationEdges : IEdgeAddingStrategy
    {
        private readonly IEnumerable<IDotAttribute> attributes;

        public AddInterfaceImplementationEdges()
            : this(Enumerable.Empty<IDotAttribute>())
        {
        }

        public AddInterfaceImplementationEdges(IEnumerable<IDotAttribute> attributes)
        {
            this.attributes = attributes.ToArray();
        }

        public Action<DotEdge> ConfigureEdge { set; private get; } =
            edge => edge.Label = "implements";

        public void AddEdges(DotSubGraph assemblyGraph, Type type, IDotElement typeNode)
        {
            foreach (var i in type.GetInterfaces())
            {
                if (assemblyGraph.TryGetTypeNode(() => i) is { } interfaceTypeNode)
                {
                    var edge = new DotEdge(typeNode, interfaceTypeNode);
                    ConfigureEdge(edge);
                    edge = attributes.Aggregate(edge, (e, a) =>
                    {
                        e.SetAttribute(a);
                        return e;
                    });
                    assemblyGraph.Elements.Add(
                        edge);
                }
            }
        }
    }
}
