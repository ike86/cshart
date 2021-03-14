#nullable enable
using System;
using System.IO;
using System.Linq;
using DotNetGraph.Core;
using DotNetGraph.Node;
using DotNetGraph.SubGraph;

namespace Cshart
{
    public static class DotSubGraphExtensions
    {
        public static IDotElement? TryGetTypeNode(
            this DotSubGraph assemblyGraph,
            Func<Type> getType)
        {
            try
            {
                if (getType() is not { } type)
                {
                    return null;
                }

                return assemblyGraph.TryFindNode(n => n.Identifier == type.FullName);
            }
            catch (Exception ex)
                when (ex is FileNotFoundException
                      || ex is TypeLoadException)
            {
                Console.WriteLine($"Getting type node failed due to {ex}");
                return null;
            }
        }

        public static DotNode? TryFindNode(this DotSubGraph subGraph, Func<DotNode, bool> predicate)
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
