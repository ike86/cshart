using System;
using System.Collections.Generic;
using System.Linq;
using DotNetGraph.Core;
using DotNetGraph.Edge;

namespace Cshart
{
    public class EdgeFactory
    {
        private readonly IEnumerable<IDotAttribute> attributes;
        private readonly Action<DotEdge> configureEdge;

        public EdgeFactory(IEnumerable<IDotAttribute> attributes, Action<DotEdge> configureEdge)
        {
            this.attributes = attributes;
            this.configureEdge = configureEdge;
        }

        public DotEdge Create(IDotElement typeNode, IDotElement paramTypeNode)
        {
            var edge = new DotEdge(typeNode, paramTypeNode);
            configureEdge(edge);
            edge = attributes.Aggregate(edge, (e, a) =>
            {
                e.SetAttribute(a);
                return e;
            });
            return edge;
        }
    }
}
