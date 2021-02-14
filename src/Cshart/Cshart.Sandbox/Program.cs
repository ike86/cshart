﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DotNetGraph;
using DotNetGraph.Attributes;
using DotNetGraph.Extensions;
using DotNetGraph.Node;
using DotNetGraph.SubGraph;

namespace Cshart.Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            var arguments = File.ReadAllLines(args[0]);

            var pathOfDirectory = arguments[0];
            var filePaths = Directory.EnumerateFiles(pathOfDirectory);
            var assemblies = TryLoadAssemblies(filePaths);

            var assemblyName = arguments[1];
            var assembly = assemblies.First(a => a.GetName().Name == assemblyName);

            var types = TryGetTypes(assembly).ToArray();
            var assemblyGraph = new DotSubGraph(assembly.GetName().Name);
            foreach (var type in types)
            {
                assemblyGraph.Elements.Add(
                    new DotNode(type.FullName) { Shape = new DotNodeShapeAttribute(DotNodeShape.Box) });
            }
            
            var dotGraph = new DotGraph("foo");
            dotGraph.Elements.Add(assemblyGraph);

            Console.Write(dotGraph.Compile(indented: true, formatStrings: true));
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

                return ex.Types.Where(t => t is { });
            }
        }

        private static IEnumerable<Assembly> TryLoadAssemblies(IEnumerable<string> filePaths)
        {
            foreach (var filePath in filePaths.Where(p => p.EndsWith(".dll")))
            {
                Assembly a = null;
                try
                {
                    a = Assembly.LoadFrom(filePath);
                    Console.WriteLine($"Loaded {a.GetName().Name}");
                }
                catch (BadImageFormatException)
                {
                    Console.WriteLine($"Could not load from {filePath}");
                }

                if (a is { })
                {
                    yield return a;
                }
            }
        }
    }
}
