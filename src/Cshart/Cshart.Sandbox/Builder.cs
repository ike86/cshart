#nullable enable
using System;
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
        public static DotGraph Build(Type[] types, string assemblyName)
        {
            var assemblyGraph = new DotSubGraph(assemblyName);
            AddTypes(types, assemblyGraph);

            AddEdges(types, assemblyGraph);
            
            var dotGraph = new DotGraph("foo");
            dotGraph.Elements.Add(assemblyGraph);
            return dotGraph;
        }

        private static void AddTypes(Type[] types, DotSubGraph assemblyGraph)
        {
            foreach (var type in types)
            {
                assemblyGraph.Elements.Add(
                    new DotNode(type.FullName) {Shape = new DotNodeShapeAttribute()});
            }
        }

        private static void AddEdges(Type[] types, DotSubGraph assemblyGraph)
        {
            foreach (var type in types)
            {
                var typeNode = TryGetTypeNode(assemblyGraph, () => type);
                var fields = type.GetFields();
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
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
    }
}
