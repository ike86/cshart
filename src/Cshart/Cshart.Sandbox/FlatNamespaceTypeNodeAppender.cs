using System;
using System.Linq;
using DotNetGraph.Attributes;
using DotNetGraph.Node;
using DotNetGraph.SubGraph;

namespace Cshart.Sandbox
{
    public class FlatNamespaceTypeNodeAppender : ITypeNodeAppender
    {
        private readonly DotSubGraph subGraph;

        public FlatNamespaceTypeNodeAppender(DotSubGraph subGraph)
        {
            this.subGraph = subGraph;
        }

        public void Append(Type type, DotNode typeNode)
        {
            var typeNamespace = type.TryGetNamespace() ?? "no namespace";
            var namespaceCluster =
                subGraph.Elements
                    .OfType<DotSubGraph>()
                    .FirstOrDefault(x => x.Identifier == typeNamespace)
                ?? new DotSubGraph(typeNamespace)
                {
                    Label = new DotLabelAttribute(typeNamespace)
                };
            namespaceCluster.Elements.Add(typeNode);

            subGraph.Elements.Add(namespaceCluster);
        }
    }
}
