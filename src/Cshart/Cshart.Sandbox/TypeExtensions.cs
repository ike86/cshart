#nullable enable
using System;
using System.IO;

namespace Cshart.Sandbox
{
    public static class TypeExtensions
    {
        public static bool IsCompilerGenerated(this Type type)
        {
            return IsUnspeakable(type.Name);

            static bool IsUnspeakable(string name)
            {
                return name.Contains("<")
                       || name.Contains(">")
                       || name.Contains("=");
            }
        }

        public static string? TryGetNamespace(this Type type)
        {
            try
            {
                return type.Namespace;
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
    }
}
