using System.Linq;
using DotNetGraph;
using DotNetGraph.Node;

namespace Cshart
{
    public static class GraphToDotGraphMapper
    {
        internal static DotGraph MapToDotGraph(this Graph graph)
        {
            var dotGraph = new DotGraph();
            dotGraph.Elements.AddRange(
                graph.Nodes.Select(n => new DotNode(n.Id)));
            return dotGraph;
        }
    }
}
