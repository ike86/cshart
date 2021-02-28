using System;
using System.Collections.Generic;
using DotNetGraph.Extensions;

namespace Cshart.Sandbox
{
    internal class BuildRenderSettings
    {
        public BuildRenderSettings(
            Func<IEnumerable<Type>, string, Builder> createBuilder,
            CompilerSettings compilerSettings,
            string renderFormat,
            string layout)
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
