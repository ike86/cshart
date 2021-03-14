#nullable enable
using System.Collections.Generic;
using System.Linq;
using Cshart.Sandbox;
using DotNetGraph.Edge;
using DotNetGraph.SubGraph;

namespace Cshart.PostProcessing
{
    using static AddFieldReferenceEdges;
    using static  AddCtorParameterTypeEdges;
    
    public class PreferFieldReferenceOverCtorParameter
    {
        public DotSubGraph PostProcess(DotSubGraph subGraph)
        {
            var edges =
                subGraph.Elements
                    .OfType<DotEdge>();

            foreach (var edge in GetEdgesToHide(edges))
            {
                edge.Style = (edge.Style ?? new DotEdgeStyle()).Style | DotEdgeStyle.Invis;
            }

            return subGraph;
        }

        private IEnumerable<DotEdge> GetEdgesToHide(IEnumerable<DotEdge> edges)
        {
            var edgesBetweenSameNodes = edges.GroupBy(e => new {e.Left, e.Right});
            foreach (var gr in edgesBetweenSameNodes)
            {
                var ctorParameterEdges =
                    gr.Where(e => e.Attributes.Any(a => a == CtorParameterEdgeTypeAttribute))
                        .ToArray();
                if (gr.Any(e => e.Attributes.Any(a => a == FieldReferenceEdgeTypeAttribute))
                    && ctorParameterEdges.Any())
                {
                    foreach (var edge in ctorParameterEdges)
                    {
                        yield return edge;
                    }
                }
            }
        }
    }
}
