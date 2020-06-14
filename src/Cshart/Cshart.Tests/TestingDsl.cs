using System;
using DotNetGraph;
using DotNetGraph.Core;
using DotNetGraph.SubGraph;

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

        public static DotGraph RootGraph(params IDotElement[] elements)
        {
            var root = new DotGraph("root", directed: true);
            root.Elements.AddRange(elements);
            return root;
        }

        public static DotSubGraph TypeCluster<T>(
            TypedNode<T> typedNode,
            params IDotElement[] elements)
        {
            var typeCluster = new DotSubGraph(typedNode.Id);
            typeCluster.Elements.AddRange(elements);
            return typeCluster;
        }
    }
}
