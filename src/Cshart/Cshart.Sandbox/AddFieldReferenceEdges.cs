using System;
using System.Linq;
using DotNetGraph.Core;
using DotNetGraph.Edge;
using DotNetGraph.SubGraph;

namespace Cshart.Sandbox
{
    public class AddFieldReferenceEdges : IEdgeAddingStrategy
    {
        public void AddEdges(DotSubGraph assemblyGraph, Type type, IDotElement typeNode)
        {
            var fields = type.TryGetFields().ToArray();
            foreach (var field in fields)
            {
                if (assemblyGraph.TryGetTypeNode(() => field.FieldType) is { } fieldTypeNode)
                {
                    assemblyGraph.Elements.Add(
                        new DotEdge(typeNode, fieldTypeNode) {Label = "contains"});
                }
            }
        }
    }
}
