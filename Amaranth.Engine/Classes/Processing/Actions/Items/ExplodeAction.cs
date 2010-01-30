using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    public class ExplodeAction : ItemAction
    {
        public ExplodeAction(Entity entity, Item item, int radius)
            : base(entity, item)
        {
            mRadius = radius;
        }

        protected override ActionResult OnProcess()
        {
            Log(LogType.Message, Item, "{subject} explodes!");

            Hero hero = (Hero)Entity;
            //### bob: hack. figure out where the item is
            Vec pos = Item.Position;
            if (!Dungeon.Items.Contains(Item))
            {
                // it must be on the hero
                pos = hero.Position;
            }

            // remove the item
            if (hero.Inventory.Contains(Item))
            {
                // it's in the inventory
                hero.Inventory.Remove(Item);
            }
            else if (hero.Equipment.Contains(Item))
            {
                // it's in the equipment
                hero.Equipment.Remove(Item);
            }
            else
            {
                // it's in the dungeon
                Dungeon.Items.Remove(Item);
            }

            // explode
            AddAction(new ElementBallAction(Entity, pos, mRadius, new Noun("the explosion"), Item.Attack));

            return ActionResult.Done;
        }

        private int mRadius;
    }
}
