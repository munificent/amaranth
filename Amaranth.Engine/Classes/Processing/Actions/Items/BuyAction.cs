using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    public class BuyAction : ItemAction
    {
        public BuyAction(NotNull<Hero> hero, NotNull<Item> item, int quantity, NotNull<Store> store)
            : base(hero, item)
        {
            mQuantity = quantity;
            mStore = store;
        }

        protected override ActionResult OnProcess()
        {
            Hero hero = (Hero)Entity;

            // spend the currency
            int price = mStore.GetBuyPrice(hero, Item);
            hero.Currency -= mQuantity * price;

            Log(LogType.Message, "{subject} buy[s] {object}.", Item);

            // pick up the newly purchased items
            AddAction(new PickUpAction(hero, Item, mQuantity, mStore));

            return ActionResult.Done;
        }

        private int mQuantity;
        private Store mStore;
    }
}
