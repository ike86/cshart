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
        }

        public Action<Type, DotNode> StyleTypeNode { init; private get; }

        public DotGraph Build()
        {
            var assemblyGraph = new DotSubGraph(assemblyName);
            AddTypes(assemblyGraph);

            AddEdges(assemblyGraph);
            
            var dotGraph = new DotGraph("foo", directed: true);
            dotGraph.Elements.Add(assemblyGraph);
            return dotGraph;
        }

        private void AddTypes(DotSubGraph assemblyGraph)
        {
            foreach (var type in types.Where(t => !t.IsCompilerGenerated()))
            {
                var typeNode = new DotNode(type.FullName) {Shape = new DotNodeShapeAttribute()};
                StyleTypeNode(type, typeNode);
                assemblyGraph.Elements.Add(typeNode);
            }
        }

        private void AddEdges(DotSubGraph assemblyGraph)
        {
            foreach (var type in types)
            {
                var typeNode = TryGetTypeNode(assemblyGraph, () => type);
                if (typeNode is not { })
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
            }
        }

        private static IDotElement? TryGetTypeNode(DotSubGraph assemblyGraph, Func<Type> getType)
        {
            try
            {
                return assemblyGraph.Elements.FirstOrDefault(
                    e => e is DotNode n
                         && n.Identifier == getType().FullName);
            }
            catch (Exception ex)
                when (ex is FileNotFoundException
                      || ex is TypeLoadException)
            {
                Console.WriteLine($"Getting type node failed due to {ex}");
                return null;
            }
        }
    }
}
