using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Cshart.Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            var paths = File.ReadAllLines(args[0]);

            var assemblies = paths.Select(p => Assembly.LoadFrom(p));

            foreach (var assembly in assemblies)
            {
                Console.WriteLine($"Loaded {assembly.GetName().Name}");
            }
        }
    }
}
