using AutoFixture.Xunit2;
using Cshart.Edges;
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
        
        [Theory, AutoData]
        public void PostProcess_hides_ctorParameter_edge_when_has_a_fieldReference_duplicate(
            PreferFieldReferenceOverCtorParameter sut,
            DotNode a,
            DotNode b)
        {
            var ctorParameterEdge = CreateCtorParameterEdge(a, b);
            var fieldReferenceEdge = CreateFieldReferenceEdge(a, b);
            var subGraph =
                new DotSubGraph() {Elements = {a, b, ctorParameterEdge, fieldReferenceEdge}};

            subGraph = sut.PostProcess(subGraph);

            var hiddenCtorParameterEdge = CreateCtorParameterEdge(a, b);
            hiddenCtorParameterEdge.Style = DotEdgeStyle.Invis;
            subGraph.Should().BeEquivalentTo(
                new DotSubGraph() {Elements = {a, b, hiddenCtorParameterEdge, fieldReferenceEdge}});
        }

        private static DotEdge CreateFieldReferenceEdge(DotNode a, DotNode b)
        {
            var fieldReferenceEdge = new DotEdge(a, b);
            fieldReferenceEdge.SetAttribute(
                AddFieldReferenceEdges.FieldReferenceEdgeTypeAttribute);
            return fieldReferenceEdge;
        }

        private static DotEdge CreateCtorParameterEdge(DotNode a, DotNode b)
        {
            var ctorParameterEdge = new DotEdge(a, b);
            ctorParameterEdge.SetAttribute(
                AddCtorParameterTypeEdges.CtorParameterEdgeTypeAttribute);
            return ctorParameterEdge;
        }
    }
}
