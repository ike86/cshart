using System;
using System.Collections.Generic;
using Cshart.Edges;
using DotNetGraph.Extensions;
using DotNetGraph.Node;

namespace Cshart
{
    static class NeatoSettingsFactory
    {
        public static BuildRenderSettings CreateSettings(
            Func<Type, bool> filterTypes,
            Action<Type, DotNode> styleTypeNode)
        {
            return new BuildRenderSettings(
                "neato",
                (ts, cn)
                    => new Builder(ts, cn)
                    {
                        FilterTypes = filterTypes,
                        StyleTypeNode = styleTypeNode,
                        CreateTypeNodeAppender = g => new FlatNamespaceTypeNodeAppender(g),
                        EdgeAddingStrategies =
                            new List<IEdgeAddingStrategy>
                            {
                                new AddFieldReferenceEdges(new[] {new EdgeLenAttribute(2)}),
                                new AddInheritanceEdges(new[] {new EdgeLenAttribute(1)}),
                                new AddInterfaceImplementationEdges(
                                    new[] {new EdgeLenAttribute(4)}),
                                new AddCtorParameterTypeEdges(new[] {new EdgeLenAttribute(3)})
                            }
                    },
                new CompilerSettings
                {
                    IsIndented = true,
                    ShouldFormatStrings = true,
                    ConfigureAttributeCompilers = x => x.Add(new EdgeLenAttributeCompiler())
                },
                "svg");
        }
    }
}
