using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    public interface IItemCollection : IEnumerable<Item>
    {
        int Count { get; }
        int Max { get; }

        Item this[int index] { get; }

        string GetCategory(int index);
    }

    public static class ItemCollectionExtensions
    {
        public static int IndexOf(this IItemCollection collection, Item item)
        {
            int index = 0;
            foreach (Item thisItem in collection)
            {
                if (thisItem == item) return index;
                index++;
            }

            return -1;
        }
    }
}
