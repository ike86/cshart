using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture.Xunit2;
using FluentAssertions;
using Xunit;

namespace Cshart.Tests
{
    public class ChartBuilder_
    {
        [Theory, AutoData]
        public void AddTypes_adds_the_specified_types(
            ChartBuilder builder,
            IEnumerable<Type> types)
        {
            types = types.ToArray();

            builder.AddTypes(types);

            builder.Types.Should().BeEquivalentTo(types);
        }
    }
}
