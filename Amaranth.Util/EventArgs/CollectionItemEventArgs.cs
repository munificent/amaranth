using System;
using System.Collections.Generic;
using System.Text;

namespace Amaranth.Util
{
    public class CollectionItemEventArgs<T> : EventArgs
    {
        public T Item { get { return mItem; } }
        public int Index { get { return mIndex; } }

        public CollectionItemEventArgs(T item, int index)
        {
            mItem = item;
            mIndex = index;
        }

        private T mItem;
        private int mIndex;
    }
}
