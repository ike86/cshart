using DotNetGraph;

namespace Cshart
{
    internal class Graph : DotGraph, IGraph
    {
        public Graph()
        {
        }

        public Graph(string identifier)
            : base(identifier, directed: true)
        {
        }
    }
}
