using System;
using DotNetGraph.Node;
using DotNetGraph.SubGraph;

namespace Cshart.Nodes
{
    public class DefaultTypeNodeAppender : ITypeNodeAppender
    {
        private readonly DotSubGraph subGraph;

        public DefaultTypeNodeAppender(DotSubGraph subGraph)
        {
            this.subGraph = subGraph;
        }
        public void Append(Type type, DotNode typeNode)
        {
            subGraph.Elements.Add(typeNode);
        }
    }
}
