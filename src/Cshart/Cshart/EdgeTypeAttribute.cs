using DotNetGraph.Core;

namespace Cshart
{
    public class EdgeTypeAttribute : IDotAttribute
    {
        public EdgeTypeAttribute(string type)
        {
            Type = type;
        }

        public string Type { get; }
    }
}
