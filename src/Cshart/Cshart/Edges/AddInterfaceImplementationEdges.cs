using System;
using System.Collections.Generic;
using System.Linq;
using DotNetGraph.Core;
using DotNetGraph.Edge;
using DotNetGraph.SubGraph;

namespace Cshart.Edges
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
            edge =>
            {
                edge.ArrowHead = DotEdgeArrowType.Open;
                edge.Style = DotEdgeStyle.Dashed;
            };

        public void AddEdges(DotSubGraph assemblyGraph, Type type, IDotElement typeNode)
        {
            foreach (var i in type.GetInterfaces())
            {
                if (assemblyGraph.TryGetTypeNode(() => i) is { } interfaceTypeNode)
                {
                    var edge =
                        new EdgeFactory(attributes, ConfigureEdge)
                            .Create(typeNode, interfaceTypeNode);
                    assemblyGraph.Elements.Add(edge);
                }
            }
        }
    }
}
