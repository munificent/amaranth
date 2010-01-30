using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    /// <summary>
    /// Base class for Actions involving an Entity and an Item.
    /// </summary>
    public abstract class ItemAction : Action
    {
        public Item Item { get { return mItem; } }

        public ItemAction(Entity entity, Item item)
            : base(entity)
        {
            mItem = item;
        }

        public ItemAction(Game game, Item item)
            : base(game)
        {
            mItem = item;
        }

        private Item mItem;
    }
}
