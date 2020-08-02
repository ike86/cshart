using System.Collections.Generic;
using System.Linq;

namespace Cshart.Tests
{
    internal class Graph
    {
        public Graph(IEnumerable<Node> nodes)
        {
            Nodes = nodes.ToArray();
        }

        public IEnumerable<Node> Nodes { get; }
    }
}
