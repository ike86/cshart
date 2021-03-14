using System;
using System.Collections.Generic;
using DotNetGraph.Extensions;

namespace Cshart
{
    internal class BuildRenderSettings
    {
        public BuildRenderSettings(
            string layout,
            Func<IEnumerable<Type>, string, Builder> createBuilder,
            CompilerSettings compilerSettings,
            string renderFormat)
        {
            CreateBuilder = createBuilder;
            CompilerSettings = compilerSettings;
            RenderFormat = renderFormat;
            Layout = layout;
        }

        public Func<IEnumerable<Type>, string, Builder> CreateBuilder { get; private set; }

        public CompilerSettings CompilerSettings { get; private set; }

        public string RenderFormat { get; private set; }

        public string Layout { get; private set; }
    }
}
