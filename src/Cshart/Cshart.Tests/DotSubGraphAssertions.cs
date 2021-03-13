using DotNetGraph.SubGraph;
using FluentAssertions.Primitives;

namespace Cshart
{
    public class DotSubGraphAssertions
    {
        private readonly ObjectAssertions assertion;

        public DotSubGraphAssertions(DotSubGraph actualValue)
        {
            this.assertion = new ObjectAssertions(actualValue);
        }
        
        public void BeEquivalentTo(
            DotSubGraph expectation,
            string because = "",
            params object[] becauseArgs)
        {
            assertion.BeEquivalentTo(
                expectation,
                config => config.RespectingRuntimeTypes(),
                because,
                becauseArgs);
        }
    }
}
