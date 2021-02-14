using System;
using DotNetGraph;
using DotNetGraph.Attributes;
using DotNetGraph.Node;
using DotNetGraph.SubGraph;

namespace Cshart.Sandbox
{
    class Builder
    {
        public static DotGraph Build(Type[] types, string? assemblyName)
        {
            var assemblyGraph = new DotSubGraph(assemblyName);
            foreach (var type in types)
            {
                assemblyGraph.Elements.Add(
                    new DotNode(type.FullName) {Shape = new DotNodeShapeAttribute()});
            }

            var dotGraph = new DotGraph("foo");
            dotGraph.Elements.Add(assemblyGraph);
            return dotGraph;
        }
    }
}
