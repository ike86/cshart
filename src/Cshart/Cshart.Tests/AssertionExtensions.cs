using DotNetGraph.SubGraph;

namespace Cshart
{
    public static class AssertionExtensions
    {
        public static DotSubGraphAssertions Should(this DotSubGraph actualValue) =>
            new(actualValue);
    }
}
