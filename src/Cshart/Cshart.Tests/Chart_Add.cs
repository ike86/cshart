using System.Linq;
using AutoFixture.Xunit2;
using FluentAssertions;
using Xunit;

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

            nodes.Should().BeEquivalentTo(
                TypeNode<WithMethods>(nameof(WithMethods.Api), "StepOne", "StepTwo"));
        }

        private static Node TypeNode<T>(params string[] methodNames)
        {
            return
                new Node(
                    typeof(T).FullName,
                    MemberNode<T>(".ctor").Yield()
                    .Concat(methodNames.Select(MemberNode<T>))
                    .ToArray());
        }

        private static Node MemberNode<T>(string memberName)
        {
            return new Node($"{typeof(T).FullName}.{memberName}");
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
