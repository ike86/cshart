using System;
using System.Collections.Generic;

namespace Cshart
{
    /// <summary>
    /// Represents a chart.
    /// </summary>
    public class Chart
    {
        public IEnumerable<Node> Add(Type type)
        {
            yield return new Node(type.FullName);
        }
    }

    public class Node
    {
        public Node(string id)
        {
            Id = id;
        }

        public string Id { get; }
    }
}