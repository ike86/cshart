using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DotNetGraph;
using DotNetGraph.Extensions;

namespace Cshart
{
    class BigThing
    {
        private readonly IEnumerable<Type> types;
        private readonly string chartName;
        private readonly string dotExePath;

        public BigThing(IEnumerable<Type> types, string chartName, string dotExePath)
        {
            this.types = types.ToArray();
            this.chartName = chartName;
            this.dotExePath = dotExePath;
        }
        
        public void BuildRenderShow(BuildRenderSettings settings)
        {
            var dotGraph = settings.CreateBuilder(types, chartName).Build();
            var compiledDotGraph = Compile(dotGraph, settings.CompilerSettings);
            var diagramFileName = $"{chartName}.{settings.Layout}.{settings.RenderFormat}";
            var dotFileFullPath = OutputDotFile(diagramFileName, compiledDotGraph);
            if (!File.Exists(dotFileFullPath))
            {
                Console.WriteLine($"Could not find file {dotFileFullPath}");
                return;
            }

            RenderSvg(
                dotExePath,
                diagramFileName,
                dotFileFullPath,
                settings.Layout,
                settings.RenderFormat);

            OpenDiagram(diagramFileName);
        }

        private static string Compile(DotGraph dotGraph, CompilerSettings compilerSettings)
        {
            Console.WriteLine("Compiling dot graph...");
            var compiledDotGraph = dotGraph.Compile(compilerSettings);
            Console.WriteLine(compiledDotGraph);
            return compiledDotGraph;
        }

        private static string OutputDotFile(string diagramFileName, string compiledDotGraph)
        {
            var dotFileName = $"{diagramFileName}.txt";
            Console.WriteLine($"Writing dot graph into {dotFileName} ...");
            File.WriteAllText(dotFileName, compiledDotGraph);

            var dotFileFullPath = Path.GetFullPath($".\\{dotFileName}");
            Console.WriteLine($"Dot graph can be found at {dotFileFullPath}");
            return dotFileFullPath;
        }

        private static void RenderSvg(
            string dotExePath,
            string diagramFileName,
            string dotFileFullPath,
            string layout,
            string format)
        {
            var process =
                Process.Start(
                    new ProcessStartInfo
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        FileName = dotExePath,
                        //Arguments = $"-T {Format} -o {diagramFileName} \"{dotFileFullPath}\"",
                        Arguments = $"-K{layout} -T {format} -o {diagramFileName} \"{dotFileFullPath}\"",
                    })!;
            Console.WriteLine(process.StandardOutput.ReadToEnd());
            Console.WriteLine(process.StandardError.ReadToEnd());
            process.WaitForExit();
        }

        private static void OpenDiagram(string diagramFileName)
        {
            new Process
                {
                    StartInfo =
                    {
                        UseShellExecute = true,
                        FileName = Path.GetFullPath($".\\{diagramFileName}")
                    }
                }
                .Start();
        }
    }
}
