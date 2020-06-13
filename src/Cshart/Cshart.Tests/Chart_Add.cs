using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Cshart.Tests
{
    public class Chart_Add
    {
        [Fact]
        public void Yields_singular_node_for_empty_type()
        {
            var chart = new Chart();

            IEnumerable<Node> nodes = chart.Add(typeof(EmptyType));

            nodes.Should().BeEquivalentTo(
                new Node(typeof(EmptyType).FullName));
        }

        private class EmptyType
        {
        }
    }
}
