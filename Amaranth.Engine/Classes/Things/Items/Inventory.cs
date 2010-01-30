using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    [Serializable]
    public class Inventory : ICollection<Item>, IItemCollection
    {
        public readonly GameEvent<Item, EventArgs> ItemAdded = new GameEvent<Item, EventArgs>();
        public readonly GameEvent<Item, EventArgs> ItemRemoved = new GameEvent<Item, EventArgs>();
        public readonly GameEvent<Item, EventArgs> ItemChanged = new GameEvent<Item, EventArgs>();

        public Item this[int index]
        {
            get
            {
                // allow out of bounds checks
                if (index >= Count) return null;

                return mItems[index];
            }
        }

        public int Max { get { return 20; } }

        public bool IsFull { get { return Count == Max; } }

        public Inventory()
        {
            mItems = new List<Item>();
        }

        public int IndexOf(Item item)
        {
            return mItems.IndexOf(item);
        }

        /// <summary>
        /// Tries to stack the given <see cref="Item"/> with an Item in the Inventory.
        /// </summary>
        /// <param name="item">The Item to stack.</param>
        /// <param name="quantity">The quantity of that Item to stack.</param>
        /// <returns>The quantity that was removed from the given item and added to
        /// stacks in the inventory.</returns>
        public int Stack(NotNull<Item> item, int quantity)
        {
            // don't stack more than we have
            quantity = Math.Min(quantity, item.Value.Quantity);

            int stacked = 0;

            foreach (Item thisItem in this)
            {
                int thisStack = thisItem.Stack(item.Value, quantity - stacked);

                if (thisStack > 0)
                {
                    stacked += thisStack;

                    ItemChanged.Raise(thisItem, EventArgs.Empty);
                }
            }

            return stacked;
        }

        public int Stack(NotNull<Item> item)
        {
            return Stack(item, item.Value.Quantity);
        }

        public void RemoveAt(int index)
        {
            Item item = mItems[index];

            item.Changed -= Item_Changed;

            mItems.RemoveAt(index);

            ItemRemoved.Raise(item, EventArgs.Empty);
        }

        private void Item_Changed(object sender, EventArgs e)
        {
            ItemChanged.Raise((Item)sender, EventArgs.Empty);
        }

        #region ICollection<Item> Members

        public void Add(Item item)
        {
            if (Count >= Max) throw new InvalidOperationException("Cannot add any more items to the inventory.");

            mItems.Add(item);
            mItems.Sort();

            item.Changed += Item_Changed;

            ItemAdded.Raise(item, EventArgs.Empty);
        }

        public void Clear()
        {
            // go through remove so the events get raised
            while (mItems.Count > 0)
            {
                RemoveAt(mItems.Count - 1);
            }
        }

        public bool Contains(Item item)
        {
            return mItems.Contains(item);
        }

        public void CopyTo(Item[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count { get { return mItems.Count; } }

        public bool IsReadOnly { get { return false; } }

        public bool Remove(Item item)
        {
            item.Changed -= Item_Changed;

            bool result = mItems.Remove(item);

            ItemRemoved.Raise(item, EventArgs.Empty);

            return result;
        }

        #endregion

        #region IEnumerable<Item> Members

        public IEnumerator<Item> GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        #endregion

        #region IItemCollection Members

        public string GetCategory(int index)
        {
            // inventory slots are not categorized
            return String.Empty;
        }

        #endregion

        private readonly List<Item> mItems;
    }
}
