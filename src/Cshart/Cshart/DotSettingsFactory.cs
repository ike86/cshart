using System;
using System.Collections.Generic;
using Cshart.Edges;
using DotNetGraph.Extensions;
using DotNetGraph.Node;

namespace Cshart
{
    static class DotSettingsFactory
    {
        public static BuildRenderSettings CreateSettings(
            Func<Type, bool> filterTypes,
            Action<Type, DotNode> styleTypeNode)
        {
            return new BuildRenderSettings(
                "dot",
                (ts, cn)
                    => new Builder(ts, cn)
                    {
                        FilterTypes = filterTypes,
                        StyleTypeNode = styleTypeNode,
                        CreateTypeNodeAppender = g => new FlatNamespaceTypeNodeAppender(g),
                        EdgeAddingStrategies =
                            new List<IEdgeAddingStrategy>
                            {
                                new AddFieldReferenceEdges(),
                                new AddInheritanceEdges(),
                                new AddInterfaceImplementationEdges(),
                                new AddCtorParameterTypeEdges()
                            }
                    },
                new CompilerSettings
                {
                    IsIndented = true,
                    ShouldFormatStrings = true,
                },
                "svg");
        }
    }
}
