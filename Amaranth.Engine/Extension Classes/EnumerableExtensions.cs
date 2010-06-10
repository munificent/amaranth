using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bramble.Core;

using Amaranth.Util;

namespace Amaranth.Engine
{
    public static class EnumerableExtensions
    {
        public static T GetAt<T>(this IEnumerable<T> collection, Vec pos) where T : IPosition
        {
            foreach (T item in collection)
            {
                if (item.Position.Equals(pos))
                {
                    return item;
                }
            }

            // no item here
            return default(T);
        }

        /// <summary>
        /// Gets all of the items in the collection at the given position.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection. Must implement <see cref="IPosition"/>.</typeparam>
        /// <param name="collection">The collection of items.</param>
        /// <param name="pos">The position to look at.</param>
        /// <returns>A list of all items in the collection at the given position.</returns>
        public static IList<T> GetAllAt<T>(this IEnumerable<T> collection, Vec pos) where T : IPosition
        {
            List<T> items = new List<T>();

            foreach (T item in collection)
            {
                if (item.Position.Equals(pos))
                {
                    items.Add(item);
                }
            }

            return items;
        }

        /// <summary>
        /// Gets the number of the items in the collection at the given position.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection. Must implement <see cref="IPosition"/>.</typeparam>
        /// <param name="collection">The collection of items.</param>
        /// <param name="pos">The position to look at.</param>
        /// <returns>The number of items in the collection at the given position.</returns>
        public static int CountAt<T>(this IEnumerable<T> collection, Vec pos) where T : IPosition
        {
            return collection.Count(item => item.Position == pos);
        }
    }
}
