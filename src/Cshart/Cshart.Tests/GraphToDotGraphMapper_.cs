using System.Linq;
using AutoFixture.Xunit2;
using DotNetGraph.Node;
using FluentAssertions;
using Xunit;

namespace Cshart.Tests
{
    public class GraphToDotGraphMapper_
    {
        [Theory, AutoData]
        public void MapToDotGraph_yields_nodes_with_ids_of_graph_nodes(
            Graph graph)
        {
            var dotGraph = graph.MapToDotGraph();

            dotGraph.Elements.Should().BeEquivalentTo(
                graph.Nodes.Select(n => new DotNode(n.Id)));
        }
    }
}
