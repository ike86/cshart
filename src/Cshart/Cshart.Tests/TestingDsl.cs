using System;

namespace Cshart.Tests
{
    internal static class TestingDsl
    {
        public static TypedNode<T> TypeNode<T>(params string[] methodNames)
        {
            var node = TypedNode.Create<T>();
            node.Add(MemberNode<T>(".ctor"));
            foreach (var methodName in methodNames)
            {
                node.Add(MemberNode<T>(methodName));
            }

            return node;
        }

        public static Node MemberNode<T>(string memberName)
        {
            return Node.FromMember(typeof(T), new FakeMemberInfo(memberName));
        }

        public static TypedNode<T> WithEdge<T>(this TypedNode<T> node, string fromId, string toId)
        {
            node.Add(
                new Edge(
                    node,
                    CreateId(typeof(T), fromId),
                    CreateId(typeof(T), toId)));
            return node;
        }

        private static string CreateId(Type type, string memberId)
        {
            return Node.CreateId(type, new FakeMemberInfo(memberId));
        }
    }
}
