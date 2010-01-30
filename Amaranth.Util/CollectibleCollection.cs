using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Amaranth.Util
{
    public class CollectibleCollection<TCollection, TItem> : Collection<TItem>
        where TCollection : CollectibleCollection<TCollection, TItem>
        where TItem : ICollectible<TCollection, TItem>
    {
        protected override void InsertItem(int index, TItem item)
        {
            item.SetCollection((TCollection)this);

            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            this[index].SetCollection(null);

            base.RemoveItem(index);
        }

        protected override void SetItem(int index, TItem item)
        {
            // forget the old item
            if (this[index] != null)
            {
                this[index].SetCollection(null);
            }

            // get the new one
            item.SetCollection((TCollection)this);

            base.SetItem(index, item);
        }

        protected override void ClearItems()
        {
            foreach (TItem item in this)
            {
                item.SetCollection(null);
            }

            base.ClearItems();
        }
    }
}
