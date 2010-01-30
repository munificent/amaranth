using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    public class PickUpAction : ItemAction
    {
        public PickUpAction(NotNull<Hero> hero, NotNull<Item> item, int quantity, NotNull<Store> store)
            : base(hero, item)
        {
            mQuantity = quantity;
            mStore = store;
        }

        public PickUpAction(NotNull<Hero> hero, NotNull<Item> item, int quantity)
            : base(hero, item)
        {
            mQuantity = quantity;
        }

        public PickUpAction(NotNull<Hero> hero, NotNull<Item> item)
            : this(hero, item, item.Value.Quantity)
        {
        }

        protected override ActionResult OnProcess()
        {
            //### bob: right now only the hero has an inventory
            Hero hero = (Hero)Entity;

            // try to stack some or all of it
            int stacked = hero.Inventory.Stack(Item, mQuantity);

            // if entire item was stacked with existing inventory items, remove it
            RemoveItemIfEmpty();

            // stacked the entire desired quantity of item
            if (stacked == mQuantity)
            {
                Item stackedItem = new Item(Item, stacked, Item.Charges);

                Log(LogType.Message, "{subject} pick[s] up {object}.", stackedItem);
                return ActionResult.Done;
            }

            // if we got here, there's still some left to pick up
            if (!hero.Inventory.IsFull)
            {
                // take the item
                Item split = Item.SplitStack(mQuantity - stacked);

                RemoveItemIfEmpty();
                hero.Inventory.Add(split);

                Log(LogType.Message, "{subject} pick[s] up {object}.", split);
                return ActionResult.Done;
            }
            else
            {
                return Fail("{subject} do[es]n't have room to pick up {object}.", Item);
            }
        }

        private void RemoveItemIfEmpty()
        {
            if (Item.Quantity == 0)
            {
                if (mStore == null)
                {
                    // remove from the dungeon
                    Dungeon.Items.Remove(Item);
                }
                else
                {
                    // remove from the store
                    mStore.Items.Remove(Item);
                }
            }
        }

        private int mQuantity;
        private Store mStore;
    }
}
