using DotNetGraph.SubGraph;

namespace Cshart
{
    internal class SubGraph : DotSubGraph, IGraph
    {
        public SubGraph(string identifier)
            : base(identifier)
        {
        }
    }
}
