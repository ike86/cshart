using AutoFixture.Xunit2;
using DotNetGraph.Edge;
using DotNetGraph.Node;
using DotNetGraph.SubGraph;
using Xunit;

namespace Cshart.PostProcessing
{
    public class PreferFieldReferenceOverCtorParameter_
    {
        [Theory, AutoData]
        public void PostProcess_keeps_edges_intact(
            PreferFieldReferenceOverCtorParameter sut,
            DotNode a,
            DotNode b)
        {
            var subGraph = new DotSubGraph() {Elements = {a, b, new DotEdge(a, b)}};

            subGraph = sut.PostProcess(subGraph);

            subGraph.Should().BeEquivalentTo( 
                new DotSubGraph() {Elements = {a, b, new DotEdge(a, b)}});
        }
    }
}
