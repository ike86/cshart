using System;
using System.Collections.Generic;
using System.Reflection;
using EnsureThat;

namespace Cshart
{
    public class Node : IGraphElement
    {
        private readonly ICollection<IGraphElement> children = new List<IGraphElement>();

        public Node(string id)
        {
            EnsureArg.IsNotNull(id, nameof(id));

            Id = id;
        }

        public Node(string id, params IGraphElement[] children)
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
            return new Node(CreateId(type, member));
        }

        public static string CreateId(Type type, MemberInfo member)
        {
            return $"{type.FullName}.{member.Name}";
        }

        public void Add(IGraphElement child)
        {
            children.Add(child);
        }
    }
}
