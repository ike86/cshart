using System;
using System.Collections.Generic;
using System.Linq;

namespace Cshart.Tests
{
    public class GraphBuilder
    {
        private readonly List<Type> types = new List<Type>();

        public IEnumerable<Type> Types => types;

        internal void AddTypes(IEnumerable<Type> types)
        {
            this.types.AddRange(types);
        }

        internal Graph BuildGraph()
        {
            var nodes = types.Select(t => new Node(t.FullName)).ToArray();

            return new Graph(nodes);
        }
    }
}
