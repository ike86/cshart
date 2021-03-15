﻿using DotNetGraph.Core;

namespace Cshart.Edges
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