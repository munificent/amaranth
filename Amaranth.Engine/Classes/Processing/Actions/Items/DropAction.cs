using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    public class DropAction : ItemAction
    {
        public DropAction(Hero hero, Item item, int quantity)
            : base(hero, item)
        {
            mQuantity = quantity;
        }

        protected override ActionResult OnProcess()
        {
            //### bob: right now only the hero has an inventory
            Hero hero = (Hero)Entity;

            // move the item to the hero's position
            Item.Position = hero.Position;

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

                Log(LogType.Message, "{subject} drop[s] {object}.", dropped);
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

                Log(LogType.Message, "{subject} unequip[s] {object} and drop[s] {object pronoun}.", dropped);
            }

            // put it on the ground
            Dungeon.Items.Add(dropped);

            return ActionResult.Done;
        }

        private int mQuantity;
    }
}
