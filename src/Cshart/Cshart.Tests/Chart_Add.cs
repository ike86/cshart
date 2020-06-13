using AutoFixture.Xunit2;
using FluentAssertions;
using Xunit;
using static Cshart.Tests.TestingDsl;

namespace Cshart.Tests
{
    public class Chart_Add
    {
        [Theory, AutoData]
        public void Yields_singular_node_for_empty_type(Chart chart)
        {
            var nodes = chart.Add(typeof(Empty));

            nodes.Should().BeEquivalentTo(TypeNode<Empty>());
        }

        [Theory, AutoData]
        public void Yields_cluster_for_non_empty_type(Chart chart)
        {
            var nodes = chart.Add(typeof(WithMethods));
            var caller = nameof(WithMethods.Api);
            var calleeOne = "StepOne";
            var calleeTwo = "StepTwo";

            nodes.Should().BeEquivalentTo(
                TypeNode<WithMethods>(caller, calleeOne, calleeTwo)
                .WithEdge(caller, calleeOne)
                .WithEdge(caller, calleeTwo));
        }

        // TODO yield calls between methods

        // TODO yield multiple types

        // TODO yield relations between types and methods

        private class Empty
        {
        }

        private class WithMethods
        {
            public void Api()
            {
                StepOne();
                StepTwo();
            }

            private void StepOne()
            {
            }

            private void StepTwo()
            {
            }
        }
    }
}
