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
            var node = Node.FromType(typeof(T));
            node.Add(MemberNode<T>(".ctor"));
            foreach (var methodName in methodNames)
            {
                node.Add(MemberNode<T>(methodName));
            }

            return node;
        }

        private static Node MemberNode<T>(string memberName)
        {
            return Node.FromMember(typeof(T), new FakeMemberInfo(memberName));
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
