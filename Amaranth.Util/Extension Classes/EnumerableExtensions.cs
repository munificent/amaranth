using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Util
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Returns the index in the enumerable of the first item that matches the predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="predicate"></param>
        /// <returns>The index of the first matching item, or -1 if none matched.</returns>
        public static int IndexOfFirst<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
        {
            int index = 0;
            foreach (T item in collection)
            {
                if (predicate(item))
                {
                    return index;
                }
                index++;
            }

            // none matched
            return -1;
        }

        public static bool Contains<T>(this IEnumerable<T> collection, string name) where T : INamed
        {
            return collection.Any(item => item.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        }

        public static T Find<T>(this IEnumerable<T> collection, string name) where T : INamed
        {
            return collection.First(item => item.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
