using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    public class SellAction : ItemAction
    {
        public SellAction(Hero hero, Item item, int quantity, int price, Store store)
            : base(hero, item)
        {
            mQuantity = quantity;
            mPrice = price;
            mStore = store;
        }

        protected override ActionResult OnProcess()
        {
            //### bob: right now only the hero has an inventory
            Hero hero = (Hero)Entity;

            Item dropped = Item;

            // drop the item
            if (hero.Inventory.Contains(Item))
            {
                // from the inventory
                if (mQuantity == Item.Quantity)
                {
                    // dropping the whole stack
                    hero.Inventory.Remove(Item);
                }
                else
                {
                    // just dropping some
                    dropped = Item.SplitStack(mQuantity);
                }

                Log(LogType.Message, "{subject} sell[s] {object}.", dropped);
            }
            else
            {
                // from the equipment
                if (mQuantity == Item.Quantity)
                {
                    // dropping the whole stack
                    hero.Equipment.Remove(Item);
                }
                else
                {
                    // just dropping some
                    dropped = Item.SplitStack(mQuantity);
                }

                Log(LogType.Message, "{subject} unequip[s] {object} and sell[s] {object pronoun}.", dropped);
            }

            // put it in the store
            mStore.Items.Stack(dropped);
            if (dropped.Quantity > 0)
            {
                mStore.Items.Add(dropped);
            }

            // gain the currency
            hero.Currency += mQuantity * mPrice;

            return ActionResult.Done;
        }

        private int mQuantity;
        private int mPrice;
        private Store mStore;
    }
}
