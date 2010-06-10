using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bramble.Core;

using Amaranth.Util;
using Amaranth.Engine;

namespace Amaranth.TermApp
{
    public class ItemsOnGroundCollection : IItemCollection
    {
        public ItemsOnGroundCollection(Dungeon dungeon, Vec position)
        {
            mDungeon = dungeon;
            mPosition = position;
        }

        #region IItemCollection Members

        public int Count { get { return mDungeon.Items.GetAllAt(mPosition).Count; } }

        public int Max { get { return mDungeon.Items.GetAllAt(mPosition).Count; } }

        public Item this[int index]
        {
            get
            {
                IList<Item> items = mDungeon.Items.GetAllAt(mPosition);

                // bail if out of bounds
                if (index >= items.Count) return null;

                // return the item
                return items[index];
            }
        }

        public string GetCategory(int index)
        {
            // ground slots are not categorized
            return String.Empty;
        }

        #endregion

        #region IEnumerable<Item> Members

        public IEnumerator<Item> GetEnumerator()
        {
            return mDungeon.Items.GetAllAt(mPosition).GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        private Dungeon mDungeon;
        private Vec mPosition;
    }
}
