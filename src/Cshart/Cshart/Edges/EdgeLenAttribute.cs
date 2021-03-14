using DotNetGraph.Compiler;
using DotNetGraph.Core;

namespace Cshart.Edges
{
    public class EdgeLenAttribute : IDotAttribute
    {
        public EdgeLenAttribute(int len)
        {
            Len = len;
        }

        public int Len { get; }
    }

    public class EdgeLenAttributeCompiler : AttributeCompilerBase<EdgeLenAttribute>
    {
        protected override string OnAttributeTypeMatch(EdgeLenAttribute attribute)
        {
            return $"len={attribute.Len}";
        }
    }
}
