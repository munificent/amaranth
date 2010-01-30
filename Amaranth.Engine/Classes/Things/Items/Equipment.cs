using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// A collection of equipped items. Unlike <see cref="Inventory"/> which is flexible list,
    /// Equipment is a fixed size collection with each slot holding a certain item type.
    /// </summary>
    [Serializable]
    public class Equipment : IItemCollection, IEnumerable<Item>
    {
        public readonly GameEvent<Item, EventArgs> ItemEquipped = new GameEvent<Item, EventArgs>();
        public readonly GameEvent<Item, EventArgs> ItemChanged = new GameEvent<Item, EventArgs>();

        /// <summary>
        /// Gets the number of slots in the equipment.
        /// </summary>
        public int Count { get { return mItems.Length; } }

        /// <summary>
        /// Gets the maximum number of items. Since Equipment is fixed size, it's the
        /// same as the count.
        /// </summary>
        public int Max { get { return Count; } }

        /// <summary>
        /// Gets the equipped melee weapon, or null if none is equipped.
        /// </summary>
        public Item MeleeWeapon { get { return this["Melee Weapon"]; } }

        /// <summary>
        /// Gets the equipped item at the given index.
        /// </summary>
        public Item this[int index]
        {
            get
            {
                // allow out of bounds checks
                if (index >= Count) return null;

                return mItems[index];
            }
        }

        /// <summary>
        /// Gets the equipped item in the first slot with the given category.
        /// </summary>
        public Item this[string category]
        {
            get
            {
                for (int i = 0; i < mCategories.Length; i++)
                {
                    if (mCategories[i] == category)
                    {
                        return mItems[i];
                    }
                }

                // category was not found
                return null;
            }
        }

        public Equipment()
        {
            //### bob: temp. should be data driven
            // fill in the allowed category for each slot
            mCategories = new string[]
                {
                    "Melee Weapon",
                    "Missile Weapon",
                    "Ammunition",
                    "Ring",
                    "Ring",
                    "Necklace",
                    "Light Source",
                    "Torso",
                    "Cloak",
                    "Shield",
                    "Helm",
                    "Gloves",
                    "Boots"
                };

            mItems = new Item[mCategories.Length];
        }

        /// <summary>
        /// Equips the item in an available slot. If an item is already in that slot, 
        /// it will be removed and returns.
        /// </summary>
        public Item Equip(Item item)
        {
            // look for an empty slot
            int slot = -1;
            for (int i = 0; i < Count; i++)
            {
                if ((mItems[i] == null) && (mCategories[i] == item.Type.Category))
                {
                    // found an empty slot of the right type
                    slot = i;
                    break;
                }
            }

            if (slot == -1)
            {
                // no empty slot, so look for any slot of the right type
                for (int i = 0; i < Count; i++)
                {
                    if (mCategories[i] == item.Type.Category)
                    {
                        // found a slot of the right type
                        slot = i;
                        break;
                    }
                }
            }

            // should have one
            if (slot == -1) throw new InvalidOperationException("Cannot equip an item of category \"" + item.Type.Category + "\".");

            // get the previous item
            Item unequip = mItems[slot];

            if ((unequip != null) && unequip.CanStack(item))
            {
                item.Stack(unequip, unequip.Quantity);
                unequip = null;
            }

            // put in the new one
            mItems[slot] = item;

            // register the event handlers
            if (unequip != null)
            {
                item.Changed -= Item_Changed;
            }

            item.Changed += Item_Changed;

            ItemEquipped.Raise(item, EventArgs.Empty);

            return unequip;
        }

        /// <summary>
        /// Removes the given equipped <see cref="Item"/> from its slot in the equipment.
        /// </summary>
        /// <param name="item">The equipped Item to unequip.</param>
        public void Remove(NotNull<Item> item)
        {
            if (!mItems.Contains(item.Value)) throw new InvalidOperationException("Cannot unequip an item that is not currently equipped.");

            // find the slot
            int index = mItems.IndexOfFirst((thisItem) => item.Value == thisItem);

            mItems[index].Changed -= Item_Changed;

            // empty it
            mItems[index] = null;

            ItemEquipped.Raise(item, EventArgs.Empty);
        }

        public bool CanEquip(NotNull<Item> item)
        {
            // see if there is a category that matches the item
            return mCategories.Any((category) => category == item.Value.Type.Category);
        }

        /// <summary>
        /// Gets whether the given equippable Item can have an entire stack equipped, or just
        /// a single Item. For example, a stack of arrows can be equipped, but not a stack
        /// of swords.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool CanStack(NotNull<Item> item)
        {
            //### bob: hack. should be data driven
            return (item.Value.Type.Category == "Ammunition");
        }

        /// <summary>
        /// Gets the quantity of the given item that can be stacked with the current
        /// equipment.
        /// </summary>
        /// <param name="item">The Item being stacked.</param>
        /// <returns>The amount of the Item that can be stacked, or zero if the
        /// Item is not stackable.</returns>
        public int GetStackCount(NotNull<Item> item)
        {
            // only handle stackable items
            if (!CanStack(item)) return 0;

            for (int i = 0; i < Count; i++)
            {
                if (mCategories[i] == item.Value.Type.Category)
                {
                    // not merging stacks, so can stack the whole thing
                    if ((mItems[i] == null) || !mItems[i].CanStack(item)) return item.Value.Quantity;

                    //### bob: hackish, -1 = stack is full
                    if (mItems[i].Quantity == Item.MaxQuantity) return -1;

                    // can stack, so just make sure the stack doesn't get too big
                    return Math.Min(Item.MaxQuantity - mItems[i].Quantity, item.Value.Quantity);
                }
            }

            throw new ArgumentException("Category not found.");
        }

        /// <summary>
        /// Iterates through all of the matching equipped items.
        /// </summary>
        /// <param name="includeMeleeWeapon"></param>
        /// <param name="includeMissileWeapons"></param>
        /// <returns></returns>
        public IEnumerable<Item> GetEquipped(bool includeMeleeWeapon, bool includeMissileWeapons)
        {
            foreach (Item item in this)
            {
                if (item != null)
                {
                    // skip over the other weapon type
                    if (!includeMeleeWeapon && (item.Type.Category == "Melee Weapon")) continue;
                    if (!includeMissileWeapons && (item.Type.Category == "Missile Weapon")) continue;
                    if (!includeMissileWeapons && (item.Type.Category == "Ammunition")) continue;

                    yield return item;
                }
            }
        }

        private void Item_Changed(object sender, EventArgs e)
        {
            ItemChanged.Raise((Item)sender, EventArgs.Empty);
        }

        #region IEnumerable<Item> Members

        public IEnumerator<Item> GetEnumerator()
        {
            return ((IEnumerable<Item>)mItems).GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        #endregion

        #region IItemCollection Members

        public string GetCategory(int index)
        {
            if (index < Count)
            {
                return mCategories[index];
            }

            return String.Empty;
        }

        #endregion

        private string[] mCategories;
        private Item[] mItems;
    }
}
