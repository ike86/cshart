using System.Collections.Generic;
using DotNetGraph.Core;

namespace Cshart
{
    internal interface IGraph
    {
        List<IDotElement> Elements { get; }
    }
}
