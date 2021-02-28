#nullable enable
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
            Func<Type, bool> filterTypes = t
                => t.Name != "QualityControlStore"
                   && !(t.TryGetNamespace()?.EndsWith(".Tagging.QualityControl")
                        ?? false);
            Action<Type, DotNode> styleTypeNode = (type, typeNode) =>
            {
                if (type.TryGetNamespace()?.EndsWith(".Modules.QualityControl")
                    ?? false)
                {
                    typeNode.FillColor = new DotFillColorAttribute(Color.Goldenrod);
                    typeNode.Style = new DotNodeStyleAttribute(DotNodeStyle.Filled);
                }
            };

            // generic logic
            var b = new BigThing(types, chartName, a.DotExePath);
            Func<Func<Type, bool>,Action<Type, DotNode>, BuildRenderSettings> createSettings =
                CreateNeatoSettings;
            b.BuildRenderShow(createSettings(filterTypes, styleTypeNode)); 
        }

        private static BuildRenderSettings CreateNeatoSettings(
            Func<Type, bool> filterTypes,
            Action<Type, DotNode> styleTypeNode)
        {
            return new BuildRenderSettings(
                (ts, cn)
                    => new Builder(ts, cn)
                    {
                        FilterTypes = filterTypes,
                        StyleTypeNode = styleTypeNode,
                        CreateTypeNodeAppender = g => new FlatNamespaceTypeNodeAppender(g),
                        EdgeAddingStrategies =
                            new List<IEdgeAddingStrategy>
                            {
                                new AddFieldReferenceEdges(new[] {new EdgeLenAttribute(2)}),
                                new AddInheritanceEdges(new[] {new EdgeLenAttribute(1)}),
                                new AddInterfaceImplementationEdges(
                                    new[] {new EdgeLenAttribute(4)}),
                                new AddCtorParameterTypeEdges(new[] {new EdgeLenAttribute(3)})
                            }
                    },
                new CompilerSettings
                {
                    IsIndented = true,
                    ShouldFormatStrings = true,
                    ConfigureAttributeCompilers = x => x.Add(new EdgeLenAttributeCompiler())
                },
                "svg",
                "neato");
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
