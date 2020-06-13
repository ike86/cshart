using System.Linq;

namespace Cshart.Tests
{
    internal static class TypedNode
    {
        public static TypedNode<T> Create<T>() => new TypedNode<T>(Node.FromType(typeof(T)));
    }

    internal class TypedNode<T> : Node
    {
        public TypedNode(Node node)
            : base(node.Id, node.Children.ToArray())
        {
        }

        public TypedNode(string id)
            : base(id)
        {
        }

        public TypedNode(string id, params Node[] children)
            : base(id, children)
        {
        }
    }
}
