using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    public class EquipAction : ItemAction
    {
        public EquipAction(Hero hero, Item item)
            : base(hero, item)
        {
        }

        protected override ActionResult OnProcess()
        {
            // only the hero can equip and unequip
            Hero hero = (Hero)Entity;

            // if an equipped item was chosen, unequip it
            if (hero.Equipment.Contains(Item))
            {
                return new UnequipAction(hero, Item);
            }

            // split the stack being equipped if needed
            Item equipItem = Item;

            bool remove = false;
            int stackQuantity = hero.Equipment.GetStackCount(equipItem);
            if (stackQuantity == equipItem.Quantity)
            {
                // entire stack can be equipped
                remove = true;
            }
            else if (stackQuantity > 0)
            {
                // part of the stack can be equipped
                equipItem = equipItem.SplitStack(stackQuantity);
            }
            else if (stackQuantity == -1)
            {
                // can't add any to a full stack, so bail
                return Fail("The stack is full.");
            }
            else if (equipItem.Quantity > 1)
            {
                // more than one, so split the stack
                equipItem = equipItem.SplitStack(1);
            }
            else
            {
                // equipping a single item
                remove = true;
            }

            // remove the item from its current location
            if (remove)
            {
                // only one, so remove it
                if (hero.Inventory.Contains(equipItem))
                {
                    hero.Inventory.Remove(equipItem);
                }
                else
                {
                    Dungeon.Items.Remove(equipItem);

                    // pick it up
                    Log(LogType.Message, "{subject} pick[s] up {object}.", equipItem);
                }
            }

            // equip the item
            Item unequipped = hero.Equipment.Equip(equipItem);

            // if we had to unequip an item, note it
            if (unequipped != null)
            {
                // try to stack it
                int stacked = hero.Inventory.Stack(unequipped);

                // try to pick up what we couldn't stack
                if (unequipped.Quantity > 0)
                {
                    // move the unequipped item to the inventory
                    if (!hero.Inventory.IsFull)
                    {
                        hero.Inventory.Add(unequipped);

                        Log(LogType.Message, "{subject} unequip[s] {object}.", unequipped);
                    }
                    else
                    {
                        // no room, so drop on ground
                        Dungeon.Items.Add(unequipped);

                        Log(LogType.Message, "{subject} drop[s] {object} on the ground.", unequipped);
                    }
                }
                else
                {
                    Log(LogType.Message, "{subject} unequip[s] {object}.", unequipped);
                }
            }

            // note the new equipment
            Log(LogType.Message, "{subject} equip[s] {object}.", equipItem);

            // light it automatically if it's a light source
            if ((equipItem.Type.ChargeType == ChargeType.Light) && !equipItem.GivesOffLight)
            {
                AddAction(new UseLightAction(Entity, equipItem));
            }

            return ActionResult.Done;
        }
    }
}
