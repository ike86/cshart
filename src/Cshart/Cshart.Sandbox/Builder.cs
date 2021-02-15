#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotNetGraph;
using DotNetGraph.Attributes;
using DotNetGraph.Core;
using DotNetGraph.Edge;
using DotNetGraph.Node;
using DotNetGraph.SubGraph;

namespace Cshart.Sandbox
{
    class Builder
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
                var typeNode = TryGetTypeNode(assemblyGraph, () => type);
                if (typeNode is null)
                {
                    continue;
                }

                var fields = type.TryGetFields().ToArray();
                foreach (var field in fields)
                {
                    if (TryGetTypeNode(assemblyGraph, () => field.FieldType) is { } fieldTypeNode)
                    {
                        assemblyGraph.Elements.Add(new DotEdge(typeNode, fieldTypeNode));
                    }
                }

                if (TryGetTypeNode(assemblyGraph, () => type.BaseType!) is { } baseTypeNode)
                {
                    assemblyGraph.Elements.Add(
                        new DotEdge(typeNode, baseTypeNode) {Label = "inherits"});
                }

                foreach (var i in type.GetInterfaces())
                {
                    if (TryGetTypeNode(assemblyGraph, () => i) is { } interfaceTypeNode)
                    {
                        assemblyGraph.Elements.Add(
                            new DotEdge(typeNode, interfaceTypeNode) {Label = "implements"});
                    }
                }
            }
        }

        private static IDotElement? TryGetTypeNode(DotSubGraph assemblyGraph, Func<Type> getType)
        {
            try
            {
                if (getType() is not { } type)
                {
                    return null;
                }

                return TryFindNode(assemblyGraph, n => n.Identifier == type.FullName);
            }
            catch (Exception ex)
                when (ex is FileNotFoundException
                      || ex is TypeLoadException)
            {
                Console.WriteLine($"Getting type node failed due to {ex}");
                return null;
            }
        }

        private static DotNode? TryFindNode(DotSubGraph subGraph, Func<DotNode, bool> predicate)
        {
            var maybeNode =
                subGraph.Elements
                    .OfType<DotNode>()
                    .FirstOrDefault(predicate);
            if (maybeNode is { })
            {
                return maybeNode;
            }

            return subGraph.Elements
                .OfType<DotSubGraph>()
                .Select(g => TryFindNode(g, predicate))
                .FirstOrDefault(n => n is { });

        }
    }
}
