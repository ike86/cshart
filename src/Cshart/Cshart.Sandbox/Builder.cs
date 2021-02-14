#nullable enable
using System;
using DotNetGraph;
using DotNetGraph.Attributes;
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
    }
}
