using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    [Serializable]
    public class PickUpAllBehavior : HeroBehavior
    {
        public override bool NeedsUserInput
        {
            get
            {
                // needs input once its done going through the actions
                return (mItems.Count == 0);
            }
        }

        public PickUpAllBehavior(NotNull<Hero> hero)
            : base(hero)
        {
            mItems = new Queue<Item>(hero.Value.Dungeon.Items.GetAllAt(Hero.Position));
        }

        public override Action NextAction()
        {
            Item item = mItems.Dequeue();

            return new PickUpAction(Hero, item);
        }

        private Queue<Item> mItems;
    }
}
