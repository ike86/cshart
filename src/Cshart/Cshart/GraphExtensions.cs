using DotNetGraph.Node;

namespace Cshart
{
    internal static class GraphExtensions
    {
        public static void AddMethod(this IGraph graph, Node method)
        {
            graph.Elements.Add(
                new DotNode
                {
                    Identifier = method.Id,
                    Label = method.Id,
                    Shape = DotNodeShape.Ellipse,
                });
        }
    }
}
