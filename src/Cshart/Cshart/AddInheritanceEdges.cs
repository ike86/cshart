﻿using System;
using System.Collections.Generic;
using System.Linq;
using DotNetGraph.Core;
using DotNetGraph.Edge;
using DotNetGraph.SubGraph;

namespace Cshart
{
    public class AddInheritanceEdges : IEdgeAddingStrategy
    {
        private readonly IEnumerable<IDotAttribute> attributes;

        public AddInheritanceEdges()
            : this(Enumerable.Empty<IDotAttribute>())
        {
        }

        public AddInheritanceEdges(IEnumerable<IDotAttribute> attributes)
        {
            this.attributes = attributes.ToArray();
        }

        public Action<DotEdge> ConfigureEdge { set; private get; } =
            edge => edge.ArrowHead = DotEdgeArrowType.Normal;

        public void AddEdges(DotSubGraph assemblyGraph, Type type, IDotElement typeNode)
        {
            if (assemblyGraph.TryGetTypeNode(() => type.BaseType!) is { } baseTypeNode)
            {
                var edge =
                    new EdgeFactory(attributes, ConfigureEdge)
                        .Create(typeNode, baseTypeNode);
                assemblyGraph.Elements.Add(edge);
            }
        }
    }
}
