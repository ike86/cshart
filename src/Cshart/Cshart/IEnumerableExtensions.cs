using System.Collections.Generic;

namespace Cshart
{
    /// <summary>
    /// Contains extension methods for <see cref="IEnumerable{T}"/>.
    /// </summary>
    internal static class IEnumerableExtensions
    {
        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> with only one element.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="IEnumerable{T}"/>.</typeparam>
        /// <param name="element">The only element.</param>
        /// <returns>Returns an <see cref="IEnumerable{T}"/> with only one element.</returns>
        public static IEnumerable<T> Yield<T>(this T element)
        {
            yield return element;
        }
    }
}
