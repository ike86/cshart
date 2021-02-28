﻿#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
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
            // specific, user logic
            if (LoadArguments(args[0]) is not { } a)
            {
                return;
            }

            var assemblies = TryLoadAssembliesFrom(a.PathOfDirectory);
            var assembly = assemblies.First(x => x.GetName().Name == a.AssemblyName);
            var types = TryGetTypes(assembly).ToArray();
            var chartName = assembly.GetName().Name!;

            var neatoCompilerSettings = new CompilerSettings
            {
                IsIndented = true,
                ShouldFormatStrings = true,
                ConfigureAttributeCompilers = x => x.Add(new EdgeLenAttributeCompiler())
            };
            Func<IEnumerable<Type>, string, Builder> CreateNeatoBuilder = (ts, cn)
                => new Builder(ts, cn)
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
                            new AddInterfaceImplementationEdges(new[]
                            {
                                new EdgeLenAttribute(4)
                            }),
                            new AddCtorParameterTypeEdges(new[] {new EdgeLenAttribute(3)})
                        }
                };
            var neatoRenderFormat = Format;
            var neatoLayout = "neato";

            // generic logic
            var b = new BigThing(types, chartName);
            b.BuildRenderShow(
                CreateNeatoBuilder,
                neatoCompilerSettings,
                neatoRenderFormat,
                a.DotExePath,
                neatoLayout, Format);
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
    }
}
