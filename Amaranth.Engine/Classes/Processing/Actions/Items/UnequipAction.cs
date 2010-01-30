using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    public class UnequipAction : ItemAction
    {
        public UnequipAction(Hero hero, Item item)
            : base(hero, item)
        {
        }

        protected override ActionResult OnProcess()
        {
            Hero hero = (Hero)Entity;

            // unequip the item
            hero.Equipment.Remove(Item);

            // try to stack it
            hero.Inventory.Stack(Item);

            // try to pick up what we couldn't stack
            if (Item.Quantity > 0)
            {
                // move the unequipped item to the inventory
                if (!hero.Inventory.IsFull)
                {
                    hero.Inventory.Add(Item);

                    Log(LogType.Message, "{subject} unequip[s] {object}.", Item);
                }
                else
                {
                    // no room, so drop on ground
                    Dungeon.Items.Add(Item);

                    Log(LogType.Message, "{subject} drop[s] {object} on the ground.", Item);
                }
            }
            else
            {
                Log(LogType.Message, "{subject} unequip[s] {object}.", Item);
            }
            
            // extinguish it automatically if it's a light source
            if ((Item.Type.ChargeType == ChargeType.Light) && Item.GivesOffLight)
            {
                AddAction(new UseLightAction(Entity, Item));
            }

            return ActionResult.Done;
        }
    }
}
