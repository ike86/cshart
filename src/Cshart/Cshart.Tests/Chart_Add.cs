using System.Linq;
using AutoFixture.Xunit2;
using DotNetGraph.Node;
using DotNetGraph.SubGraph;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;
using static Cshart.Tests.TestingDsl;

namespace Cshart.Tests
{
    public static class Chart_
    {
        public class Add_
        {
            [Theory, AutoData]
            public void Yields_singular_node_for_empty_type(Chart chart)
            {
                var nodes = chart.Add(typeof(Empty));

                nodes.Should().BeEquivalentTo(TypeNode<Empty>());
            }

            [Theory, AutoData]
            public void Yields_cluster_for_non_empty_type(Chart chart)
            {
                var nodes = chart.Add(typeof(WithMethods));
                var caller = nameof(WithMethods.Api);
                var calleeOne = "StepOne";
                var calleeTwo = "StepTwo";

                nodes.Should().BeEquivalentTo(
                    TypeNode<WithMethods>(caller, calleeOne, calleeTwo)
                    .WithEdge(caller, calleeOne)
                    .WithEdge(caller, calleeTwo));
            }
        }

        public class ConvertToDotGraph_
        {
            // TODO fix false positive test
            [Theory, AutoData]
            public void Yields_a_graph_with_a_cluster_with_a_node_for_emtpy_type(
                Chart chart)
            {
                var node = TypeNode<Empty>();

                var graph = chart.ConvertToDotGraph(node.Yield());

                graph.Should().BeEquivalentTo(
                    RootGraph(
                        TypeCluster(
                            node
                            ,
                            new DotNode(node.Children.Single().Id)
                            )));
            }


            [Theory, AutoData]
            public void Test(string id)
            {
                var node = TypeNode<Empty>();
                var typeCluster = new DotSubGraph(id);
                typeCluster.Elements.Add(new DotNode(node.Children.Single().Id));

                var typeCluster2 = new DotSubGraph(id);

                using var a = new AssertionScope();
                typeCluster.Should().BeEquivalentTo(typeCluster2);
                typeCluster2.Should().BeEquivalentTo(typeCluster);
            }

        }

        // TODO write first e2e test
        // - map between internal- and DotNetGraph model

        // TODO yield multiple types

        // TODO yield relations between types and methods

        private class Empty
        {
        }

        private class WithMethods
        {
            public void Api()
            {
                StepOne();
                StepTwo();
            }

            private void StepOne()
            {
            }

            private void StepTwo()
            {
            }
        }
    }
}
