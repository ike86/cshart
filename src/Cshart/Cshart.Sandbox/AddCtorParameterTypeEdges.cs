using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DotNetGraph.Core;
using DotNetGraph.Edge;
using DotNetGraph.SubGraph;

namespace Cshart.Sandbox
{
    public class AddCtorParameterTypeEdges : IEdgeAddingStrategy
    {
        private readonly IEnumerable<IDotAttribute> attributes;

        public AddCtorParameterTypeEdges()
            : this(Enumerable.Empty<IDotAttribute>())
        {
            
        }

        public AddCtorParameterTypeEdges(IEnumerable<IDotAttribute> attributes)
        {
            this.attributes = attributes.ToArray();
        }

        public void AddEdges(DotSubGraph assemblyGraph, Type type, IDotElement typeNode)
        {
            foreach (var ctor in type.GetConstructors(BindingFlags.Public | BindingFlags.Instance))
            {
                foreach (var param in ctor.TryGetParameters())
                {
                    if (assemblyGraph.TryGetTypeNode(() => param.ParameterType)
                        is { } paramTypeNode)
                    {
                        var edge = new DotEdge(typeNode, paramTypeNode) {Label = "ctor param"};
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
}
