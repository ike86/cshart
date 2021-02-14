using System;

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
    }
}
