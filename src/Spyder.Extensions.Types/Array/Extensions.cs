using System.Collections.Generic;
using System.Linq;

namespace Spyder.Extensions.Types.Array
{
    public static class Extensions
    {
        /// <summary>
        /// Appends the given objects to the original array source.
        /// </summary>
        /// <typeparam name="T">The specified type of the array</typeparam>
        /// <param name="source">The original array of values</param>
        /// <param name="toAdd">The values to append to the source.</param>
        /// <returns>The concatenated array of the specified type</returns>
        public static T[] Append<T>(this IEnumerable<T> source, params T[] toAdd) => source.Concat(toAdd).ToArray();

        /// <summary>
        /// Prepend the given objects to the original source array
        /// </summary>
        /// <typeparam name="T">The specified type of the array</typeparam>
        /// <param name="source">The original array of values</param>
        /// <param name="toAdd">The values to append to the source.</param>
        /// <returns>The prepended array of the specified type.</returns>
        public static T[] Prepend<T>(this T[] source, params T[] toAdd) => toAdd.Append(source);
    }
}