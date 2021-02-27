﻿#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using DotNetGraph;
using DotNetGraph.Attributes;
using DotNetGraph.Extensions;
using DotNetGraph.Node;

namespace Cshart.Sandbox
{
    class Program
    {
        private const string Format = "svg";
        
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
            var dotGraph =
                new Builder(types, assemblyName)
                    {
                        FilterTypes =
                            t => t.Name != "QualityControlStore"
                                 && !(t.TryGetNamespace()?.EndsWith(".Tagging.QualityControl")
                                      ?? false),
                        StyleTypeNode = (type, typeNode) =>
                        {
                            if (type.TryGetNamespace()?.EndsWith(".Modules.QualityControl")
                                ?? false)
                            {
                                typeNode.FillColor = new DotFillColorAttribute(Color.Goldenrod);
                                typeNode.Style = new DotNodeStyleAttribute(DotNodeStyle.Filled);
                            }
                        },
                        CreateTypeNodeAppender = g => new FlatNamespaceTypeNodeAppender(g),
                        EdgeAddingStrategies =
                            new List<IEdgeAddingStrategy>
                            {
                                new AddFieldReferenceEdges(new[] {new EdgeLenAttribute(2)}),
                                new AddInheritanceEdges(new[] {new EdgeLenAttribute(1)}),
                                new AddInterfaceImplementationEdges(new[] {new EdgeLenAttribute(4)}),
                                new AddCtorParameterTypeEdges()
                            }
                    }
                    .Build();
            
            var compiledDotGraph = Compile(dotGraph);
            var diagramFileName = $"{a.AssemblyName}.{Format}";
            var dotFileFullPath = OutputDotFile(diagramFileName, compiledDotGraph);
            if (!File.Exists(dotFileFullPath))
            {
                Console.WriteLine($"Could not find file {dotFileFullPath}");
                return;
            }

            // var dotFileName = $"{diagramFileName}.txt";
            // var dotFileFullPath = Path.GetFullPath($".\\{dotFileName}");
            RenderSvg(a.DotExePath, diagramFileName, dotFileFullPath);

            OpenDiagram(diagramFileName);
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
            var compiledDotGraph =
                dotGraph.Compile(
                        new CompilerSettings
                            {
                                IsIndented = true,
                                ShouldFormatStrings = true,
                                ConfigureAttributeCompilers = x => x.Add(new EdgeLenAttributeCompiler())
                            })
                .Replace(@"[label=""ctor param""]", @"[label=""ctor param"",len=3]");

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
                        //Arguments = $"-T {Format} -o {diagramFileName} \"{dotFileFullPath}\"",
                        Arguments = $"-Kneato -T {Format} -o {diagramFileName} \"{dotFileFullPath}\"",
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
