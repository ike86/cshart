#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using DotNetGraph;
using DotNetGraph.Attributes;
using DotNetGraph.Node;
using DotNetGraph.SubGraph;

namespace Cshart.Sandbox
{
    public class Builder
    {
        private readonly IEnumerable<Type> types;
        private readonly string assemblyName;

        public Builder(IEnumerable<Type> types, string assemblyName)
        {
            this.types = types.ToArray();
            this.assemblyName = assemblyName;

            StyleTypeNode = (_, _) => { };
            CreateTypeNodeAppender = g => new DefaultTypeNodeAppender(g);
            FilterTypes = t => true;
        }

        public Action<Type, DotNode> StyleTypeNode { init; private get; }

        public Func<DotSubGraph, ITypeNodeAppender> CreateTypeNodeAppender { init; private get; }

        public Func<Type, bool> FilterTypes { init; private get; }

        public ICollection<IEdgeAddingStrategy> EdgeAddingStrategies { init; private get; } =
            new List<IEdgeAddingStrategy>
            {
                new AddFieldReferenceEdges(),
                new AddInheritanceEdges(),
                new AddInterfaceImplementationEdges(),
                new AddCtorParameterTypeEdges()
            };

        public DotGraph Build()
        {
            var assemblyGraph = new DotSubGraph(assemblyName)
            {
                Label = assemblyName,
                Style = new DotSubGraphStyleAttribute(DotSubGraphStyle.Rounded),
            };

            AddTypes(assemblyGraph);

            AddEdges(assemblyGraph);

            var dotGraph = new DotGraph("foo", directed: true);
            dotGraph.Elements.Add(assemblyGraph);
            return dotGraph;
        }

        private void AddTypes(DotSubGraph assemblyGraph)
        {
            var typeNodeAppender = CreateTypeNodeAppender(assemblyGraph);
            foreach (var type in FilteredTypes())
            {
                var typeNode = new DotNode(type.FullName) {Shape = new DotNodeShapeAttribute()};
                StyleTypeNode(type, typeNode);

                typeNodeAppender.Append(type, typeNode);
            }
        }

        private IEnumerable<Type> FilteredTypes()
        {
            return types
                .Where(t => !t.IsCompilerGenerated())
                .Where(FilterTypes);
        }

        private void AddEdges(DotSubGraph assemblyGraph)
        {
            foreach (var type in types)
            {
                var typeNode = assemblyGraph.TryGetTypeNode(() => type);
                if (typeNode is null)
                {
                    continue;
                }

                foreach (var strategy in EdgeAddingStrategies)
                {
                    strategy.AddEdges(assemblyGraph, type, typeNode);
                }
            }
        }
    }
}
