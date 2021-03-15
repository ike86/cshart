using System;
using System.Linq;
using DotNetGraph.Attributes;
using DotNetGraph.Node;
using DotNetGraph.SubGraph;

namespace Cshart.Nodes
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
            typeNode.Label = type.Name;
            var typeNamespace = type.TryGetNamespace() ?? "no namespace";
            var namespaceCluster =
                subGraph.Elements
                    .OfType<DotSubGraph>()
                    .FirstOrDefault(x => x.Identifier == $"cluster_{typeNamespace}")
                ?? new DotSubGraph($"cluster_{typeNamespace}")
                {
                    Label = new DotLabelAttribute(typeNamespace),
                    Style = new DotSubGraphStyleAttribute(DotSubGraphStyle.Rounded),
                };
            namespaceCluster.Elements.Add(typeNode);

            subGraph.Elements.Add(namespaceCluster);
        }
    }
}
