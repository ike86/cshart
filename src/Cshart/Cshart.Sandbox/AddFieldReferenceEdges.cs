using System;
using System.Collections.Generic;
using System.Linq;
using DotNetGraph.Core;
using DotNetGraph.Edge;
using DotNetGraph.SubGraph;

namespace Cshart.Sandbox
{
    public class AddFieldReferenceEdges : IEdgeAddingStrategy
    {
        private readonly IEnumerable<IDotAttribute> attributes;

        public AddFieldReferenceEdges() : this(Enumerable.Empty<IDotAttribute>())
        {
        }

        public AddFieldReferenceEdges(IEnumerable<IDotAttribute> attributes)
        {
            this.attributes = attributes.ToArray();
        }
        
        public void AddEdges(DotSubGraph assemblyGraph, Type type, IDotElement typeNode)
        {
            var fields = type.TryGetFields().ToArray();
            foreach (var field in fields)
            {
                if (assemblyGraph.TryGetTypeNode(() => field.FieldType) is { } fieldTypeNode)
                {
                    var edge = new DotEdge(typeNode, fieldTypeNode) {Label = "contains"};
                    edge = attributes.Aggregate(edge, (e, a) =>
                    {
                        e.SetAttribute(a);
                        return e;
                    });
                    assemblyGraph.Elements.Add(edge);
                }
            }
        }
    }
}
