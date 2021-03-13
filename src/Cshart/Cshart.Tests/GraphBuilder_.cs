using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture.Xunit2;
using FluentAssertions;
using Xunit;

namespace Cshart.Tests
{
    public class GraphBuilder_
    {
        // [Theory, AutoData]
        // public void AddTypes_adds_the_specified_types(
        //     GraphBuilder builder,
        //     IEnumerable<Type> types)
        // {
        //     types = types.ToArray();
        //
        //     builder.AddTypes(types);
        //
        //     builder.Types.Should().BeEquivalentTo(types);
        // }
        //
        // [Theory, AutoData]
        // public void BuildGraph_yields_nodes_with_types_FullNames(
        //     GraphBuilder builder,
        //     IEnumerable<Type> types)
        // {
        //     types = types.ToArray();
        //     builder.AddTypes(types);
        //
        //     var graph = builder.BuildGraph();
        //
        //     var nodeIds = graph.Nodes.Select(n => n.Id);
        //     nodeIds.Should().BeEquivalentTo(types.Select(t => t.FullName));
        // }
    }
}
