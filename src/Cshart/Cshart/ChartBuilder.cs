using System;
using System.Collections.Generic;

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
    }
}
