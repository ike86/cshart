using System.Linq;
using AutoFixture.Xunit2;
using DotNetGraph.Node;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Cshart.Tests
{
    public static class ExploratoryTests_
    {
        public class FluentAssertions
        {
            // FluentAssertion will never compare the actual types
            // see https://github.com/fluentassertions/fluentassertions/issues/978#issuecomment-441060261
            [Theory, AutoData]
            public void Considers_Subgraph_equivalent_even_when_elements_are_different(
                string graphId,
                DotNode dotNode1,
                DotNode dotNode2)
            {
                var graph1 = new SubGraph(graphId);
                graph1.Elements.Add(dotNode1);

                var graph2 = new SubGraph(graphId);
                graph2.Elements.Add(dotNode2);

                using var a = new AssertionScope();
                graph1.Should().BeEquivalentTo(graph2);
                var cluster = graph1.Elements.First() as DotNode;
                var cluster2 = graph2.Elements.First() as DotNode;
                cluster.Should().NotBeEquivalentTo(cluster2);
            }

            [Theory, AutoData]
            public void Considers_Subgraph_unequal_when_runtime_types_should_be_checked(
                string graphId,
                DotNode dotNode1,
                DotNode dotNode2)
            {
                var graph1 = new SubGraph(graphId);
                graph1.Elements.Add(dotNode1);

                var graph2 = new SubGraph(graphId);
                graph2.Elements.Add(dotNode2);

                graph1.Should().NotBeEquivalentTo(graph2, options => options.RespectingRuntimeTypes());
            }
        }
    }
}
