#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using DotNetGraph;
using DotNetGraph.Extensions;

namespace Cshart.Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            if (LoadArguments(args[0]) is not { } a)
            {
                return;
            }

            var assemblies = TryLoadAssembliesFrom(a.PathOfDirectory);

            var assembly = assemblies.First(x => x.GetName().Name == a.AssemblyName);

            var types = TryGetTypes(assembly).ToArray();
            var assemblyName = assembly.GetName().Name!;
            var dotGraph = Builder.Build(types, assemblyName);

            var compiledDotGraph = Compile(dotGraph);
            var diagramFileName = $"{a.AssemblyName}.svg";
            var dotFileFullPath = OutputDotFile(diagramFileName, compiledDotGraph);
            if (!File.Exists(dotFileFullPath))
            {
                Console.WriteLine($"Could not find file {dotFileFullPath}");
                return;
            }

            RenderSvg(a.DotExePath, diagramFileName, dotFileFullPath);
        }

        private static Arguments? LoadArguments(string arg)
        {
            var arguments = File.ReadAllLines(arg);

            var pathOfDirectory = arguments[0];
            var assemblyName = arguments[1];
            var dotExePath = arguments[2];

            if (!File.Exists(dotExePath))
            {
                Console.WriteLine($"Could not find file {dotExePath}");
                return null;
            }

            return new Arguments(pathOfDirectory, assemblyName, dotExePath);
        }

        private record Arguments(string PathOfDirectory, string AssemblyName, string DotExePath);

        private static IEnumerable<Assembly> TryLoadAssembliesFrom(string pathOfDirectory)
        {
            var filePaths = Directory.EnumerateFiles(pathOfDirectory);
            foreach (var filePath in filePaths.Where(p => p.EndsWith(".dll")))
            {
                if (TryLoadAssemblyFrom(filePath) is { } assembly)
                {
                    yield return assembly;
                }
            }

            static Assembly? TryLoadAssemblyFrom(string filePath)
            {
                try
                {
                    var a = Assembly.LoadFrom(filePath);
                    Console.WriteLine($"Loaded {a.GetName().Name}");
                    return a;
                }
                catch (BadImageFormatException)
                {
                    Console.WriteLine($"Could not load from {filePath}");
                    return null;
                }
            }
        }

        private static IEnumerable<Type> TryGetTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                foreach (var loaderException in ex.LoaderExceptions)
                {
                    Console.WriteLine(loaderException);
                }

                return ex.Types.Where(t => t is { })!;
            }
        }

        private static string Compile(DotGraph dotGraph)
        {
            Console.WriteLine("Compiling dot graph...");
            var compiledDotGraph = dotGraph.Compile(indented: true, formatStrings: true);
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
            string dotFileFullPath)
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
                        Arguments = $"-T svg -o {diagramFileName} \"{dotFileFullPath}\"",
                    })!;
            Console.WriteLine(process.StandardOutput.ReadToEnd());
            Console.WriteLine(process.StandardError.ReadToEnd());
            process.WaitForExit();
        }
    }
}
