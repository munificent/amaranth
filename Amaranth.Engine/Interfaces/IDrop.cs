using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    /// <summary>
    /// Base interface for a class that can drop one or more objects when invoked.
    /// </summary>
    /// <typeparam name="T">Type of object being dropped.</typeparam>
    public interface IDrop<T>
    {
        IEnumerable<T> Create(int level);
    }

    public static class IDropExtensions
    {
        public static T CreateOne<T>(this IDrop<T> drop, int level)
        {
            foreach (T item in drop.Create(level))
            {
                return item;
            }

            return default(T);
        }
    }
}
