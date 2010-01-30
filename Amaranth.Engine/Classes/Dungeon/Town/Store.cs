using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// One store in the <see cref="Town"/>.
    /// </summary>
    [Serializable]
    public class Store : INoun, INamed
    {
        #region INoun Members

        public string NounText { get { return mType.Name; } }
        public Person Person { get { return Person.Third; } }
        public string Pronoun { get { return "it"; } }
        public string Possessive { get { return "its"; } }

        #endregion

        #region INamed Members

        public string Name { get { return mType.Name; } }

        #endregion

        /// <summary>
        /// Gets the contents of the Store.
        /// </summary>
        public Inventory Items { get { return mInventory; } }

        public Store(StoreType type)
        {
            mType = type;
            mInventory = new Inventory();

            // initially populate it
            for (int i = 0; i < 10; i++)
            {
                UpdateInventory();
            }
        }

        /// <summary>
        /// Churns the store's inventory a bit. Should be called more frequently the longer it's been
        /// since the Hero last visited.
        /// </summary>
        public void UpdateInventory()
        {
            for (int i = 0; i < Rng.Int(10); i++)
            {
               // drop some new items
                foreach (Item item in mType.Value.Drop.Create(mType.Value.Depth))
                {
                    // as the store gets more full, remove items
                    if (mInventory.Count > Rng.Int(mInventory.Max))
                    {
                        mInventory.RemoveAt(Rng.Int(mInventory.Count));
                    }

                    mInventory.Stack(item);

                    if (item.Quantity > 0)
                    {
                        mInventory.Add(item);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the price to buy one of the given <see cref="Item"/> from this <see cref="Store"/>.
        /// Takes into account <see cref="Charisma"/>, etc.
        /// </summary>
        /// <param name="item">The Item being purchased from the store.</param>
        /// <returns>The unit price for one of the Item.</returns>
        public int GetBuyPrice(Hero hero, Item item)
        {
            float price = item.Price * BuyMultiplier * hero.Stats.Charisma.BuyPriceMultiplier;

            return (int)Math.Max(1.0f, price);
        }

        /// <summary>
        /// Gets the price to sell one of the given <see cref="Item"/> from this <see cref="Store"/>.
        /// Takes into account <see cref="Charisma"/>, etc.
        /// </summary>
        /// <param name="item">The Item being sold to the store.</param>
        /// <returns>The unit price for one of the Item.</returns>
        public int GetSellPrice(Hero hero, Item item)
        {
            float price = item.Price * SellMultiplier * hero.Stats.Charisma.SellPriceMultiplier;

            return (int)Math.Max(1.0f, price);
        }

        private const float BuyMultiplier = 1.0f;
        private const float SellMultiplier = 0.5f;

        private Inventory mInventory;
        private StoreTypeRef mType;
    }
}
