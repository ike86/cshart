using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Cshart
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
        
        public static IEnumerable<FieldInfo> TryGetFields(this Type type)
        {
            return TryGetFieldInfos(type, BindingFlags.Instance | BindingFlags.Public)
                .Concat(TryGetFieldInfos(type, BindingFlags.Instance | BindingFlags.NonPublic))
                .Concat(TryGetFieldInfos(type, BindingFlags.Static | BindingFlags.Public))
                .Concat(TryGetFieldInfos(type, BindingFlags.Static | BindingFlags.NonPublic));

            static FieldInfo[] TryGetFieldInfos(Type type, BindingFlags f)
            {
                try
                {
                    return type.GetFields(f);
                }
                catch (FileNotFoundException ex)
                {
                    Console.WriteLine($"Skipping {f} fields of {type.FullName} due to {ex}");
                    return Array.Empty<FieldInfo>();
                }
            }
        }

        public static IEnumerable<ParameterInfo> TryGetParameters(this ConstructorInfo ctor)
        {
            try
            {
                return ctor.GetParameters();
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Skipping parameters of a constructor type {ctor.DeclaringType} due to {ex}");
                return Array.Empty<ParameterInfo>();
            }
        }
    }
}
