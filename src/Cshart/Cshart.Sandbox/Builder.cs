#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
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
        }
        
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
                if (type.TryGetNamespace()?.EndsWith(".Modules.QualityControl") ?? false)
                {
                    typeNode.FillColor = new DotFillColorAttribute(Color.Goldenrod);
                    typeNode.Style = new DotNodeStyleAttribute(DotNodeStyle.Filled);
                }

                assemblyGraph.Elements.Add(
                    typeNode);
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

                var fields = TryGetFields(type).ToArray();
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

        private static IEnumerable<FieldInfo> TryGetFields(Type type)
        {
            return TryGetFieldInfos(type, BindingFlags.Instance | BindingFlags.Public)
                .Concat(TryGetFieldInfos(type, BindingFlags.Instance | BindingFlags.NonPublic))
                .Concat(TryGetFieldInfos(type, BindingFlags.Static | BindingFlags.Public))
                .Concat(TryGetFieldInfos(type, BindingFlags.Static | BindingFlags.NonPublic));

            static FieldInfo[] TryGetFieldInfos(Type type, BindingFlags f)
            {
                try
                {
                    return type.GetFields(f);
                }
                catch (FileNotFoundException ex)
                {
                    Console.WriteLine($"Skipping {f} fields of {type.FullName} due to {ex}");
                    return Array.Empty<FieldInfo>();
                }
            }
        }
    }
}
