using System;
using System.Collections.Generic;
using System.Reflection;
using EnsureThat;

namespace Cshart
{
    /// <summary>
    /// Represents a chart.
    /// </summary>
    public class Chart
    {
        public IEnumerable<IGraphElement> Add(Type type)
        {
            EnsureArg.IsNotNull(type, nameof(type));

            var node = Node.FromType(type);
            foreach (var member
                in type.GetMembers(
                    BindingFlags.DeclaredOnly
                    | BindingFlags.Public
                    | BindingFlags.NonPublic
                    | BindingFlags.Instance))
            {
                node.Add(Node.FromMember(type, member));
            }

            yield return node;
        }
    }

    public interface IGraphElement
    {
        string Id { get; }

        IEnumerable<IGraphElement> Children { get; }

        void Add(IGraphElement child);
    }

    public class Node : IGraphElement
    {
        private readonly ICollection<IGraphElement> children = new List<IGraphElement>();

        public Node(string id)
        {
            EnsureArg.IsNotNull(id, nameof(id));

            Id = id;
        }

        public Node(string id, params Node[] children)
            : this(id)
        {
            foreach (var child in children)
            {
                Add(child);
            }
        }

        public string Id { get; }

        public IEnumerable<IGraphElement> Children => children;

        public static Node FromType(Type type) => new Node(type.FullName);

        public static Node FromMember(Type type, MemberInfo member)
        {
            return new Node($"{type.FullName}.{member.Name}");
        }

        public void Add(IGraphElement child)
        {
            children.Add(child);
        }
    }
}
