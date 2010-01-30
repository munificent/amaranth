using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// <see cref="IDrop"/> that drops a single <see cref="Item"/> of a given <see cref="ItemType"/>.
    /// </summary>
    public class ItemDrop : IDrop<Item>
    {
        public ItemDrop(NotNull<ItemType> itemType, Roller quantity)
        {
            mItemType = itemType;
            mQuantity = quantity;
        }

        public ItemDrop(NotNull<ItemType> itemType)
            : this(itemType, null)
        {
        }

        public IEnumerable<Item> Create(int level)
        {
            Item item = Item.Random(Vec.Zero, mItemType, level);

            if (mQuantity != null)
            {
                // override the quantity
                item.Quantity = mQuantity.Roll();
            }

            yield return item;
        }

        private ItemType mItemType;
        private Roller mQuantity;
    }
}
