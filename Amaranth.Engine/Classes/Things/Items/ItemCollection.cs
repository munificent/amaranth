using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    [Serializable]
    public class ItemCollection : ThingCollection<Item>
    {
        /// <summary>
        /// Gets the <see cref="Dungeon"/> that owns this collection.
        /// </summary>
        public Dungeon Dungeon { get { return mDungeon; } }

        public ItemCollection(Dungeon dungeon)
        {
            mDungeon = dungeon;
        }

        protected override void OnItemAdded(Item item)
        {
            base.OnItemAdded(item);

            ((ICollectible<ItemCollection, Item>)item).SetCollection(this);

            // if the item gives off light, refresh
            if (item.GivesOffLight)
            {
                mDungeon.DirtyLighting();
            }
        }

        protected override void OnItemRemoved(Item item)
        {
            base.OnItemRemoved(item);

            ((ICollectible<ItemCollection, Item>)item).SetCollection(this);

            // if the item gives off light, refresh
            if (item.GivesOffLight)
            {
                mDungeon.DirtyLighting();
            }
        }

        private Dungeon mDungeon;
    }
}
