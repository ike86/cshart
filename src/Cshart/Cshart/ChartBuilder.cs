using System;
using System.Collections.Generic;
using System.Linq;

namespace Cshart.Tests
{
    public class ChartBuilder
    {
        private readonly List<Type> types = new List<Type>();

        public IEnumerable<Type> Types => types;

        internal void AddTypes(IEnumerable<Type> types)
        {
            this.types.AddRange(types);
        }

        internal Graph BuildGraph()
        {
            return new Graph()
            {
                Nodes = types.Select(t => new Node(t.FullName)).ToArray(),
            };
        }
    }
}
