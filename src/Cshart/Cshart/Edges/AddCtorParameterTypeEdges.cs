using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DotNetGraph.Core;
using DotNetGraph.Edge;
using DotNetGraph.SubGraph;

namespace Cshart.Edges
{
    public class AddCtorParameterTypeEdges : IEdgeAddingStrategy
    {
        public static readonly EdgeTypeAttribute CtorParameterEdgeTypeAttribute =
            new EdgeTypeAttribute("ctor parameter");
        
        private readonly IEnumerable<IDotAttribute> attributes;

        public AddCtorParameterTypeEdges()
            : this(Enumerable.Empty<IDotAttribute>())
        {
        }

        public AddCtorParameterTypeEdges(IEnumerable<IDotAttribute> attributes)
        {
            this.attributes = attributes.ToArray();
        }

        public Action<DotEdge> ConfigureEdge { set; private get; } =
            edge => edge.ArrowHead = DotEdgeArrowType.Open;

        public void AddEdges(DotSubGraph assemblyGraph, Type type, IDotElement typeNode)
        {
            foreach (var ctor in type.GetConstructors(BindingFlags.Public | BindingFlags.Instance))
            {
                foreach (var param in ctor.TryGetParameters())
                {
                    if (assemblyGraph.TryGetTypeNode(() => param.ParameterType)
                        is { } paramTypeNode)
                    {
                        var edge =
                            new EdgeFactory(attributes, ConfigureEdge)
                                .Create(typeNode, paramTypeNode);
                        edge.SetAttribute(CtorParameterEdgeTypeAttribute);
                        assemblyGraph.Elements.Add(edge);
                    }
                }
            }
        }
    }
}
