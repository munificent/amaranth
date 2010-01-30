using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    [Serializable]
    public class ThingCollection<T> : Collection<T> where T : IPosition
    {
        //### bob: instead of events here, just add a TileDirty event to game?
        public readonly GameEvent<T, EventArgs> ItemAdded = new GameEvent<T, EventArgs>();
        public readonly GameEvent<T, EventArgs> ItemRemoved = new GameEvent<T, EventArgs>();

        protected override void ClearItems()
        {
            // remove one at a time so the events are raised correctly
            while (Count > 0)
            {
                RemoveAt(Count - 1);
            }
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);

            OnItemAdded(item);
        }

        protected override void RemoveItem(int index)
        {
            T item = this[index];

            base.RemoveItem(index);

            OnItemRemoved(item);
        }

        protected override void SetItem(int index, T item)
        {
            throw new NotSupportedException();
        }

        protected virtual void OnItemAdded(T item)
        {
            ItemAdded.Raise(item, EventArgs.Empty);
        }

        protected virtual void OnItemRemoved(T item)
        {
            ItemRemoved.Raise(item, EventArgs.Empty);
        }
    }
}
