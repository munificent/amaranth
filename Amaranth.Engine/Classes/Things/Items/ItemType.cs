using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// A generatable class of items in the game. For example, all "daggers" or "potions of healing" share the same ItemType.
    /// Comparable to a Monster's Race.
    /// </summary>
    public class ItemType : ContentType, IComparable<ItemType>
    {
        public override string Name { get { return mName.Singular; } }

        public FlagCollection Flags { get { return mFlags; } }

        /// <summary>
        /// Gets the leaf-most category that this ItemType has.
        /// </summary>
        public string Category
        {
            get
            {
                if (!String.IsNullOrEmpty(mSubcategory)) return mSubcategory;

                return mCategory;
            }
        }

        public Noun Noun { get { return mName; } }

        public string Supercategory { get { return mCategory; } }
        public string Subcategory { get { return mSubcategory; } }
        public object Appearance { get { return mAppearance; } }

        /// <summary>
        /// Gets the <see cref="Roller"/> used to roll the quantity of items 
        /// of this type.
        /// </summary>
        public Roller Quantity { get { return mQuantity; } }

        /// <summary>
        /// Gets the <see cref="Roller"/> used to roll the starting charges of items 
        /// of this type.
        /// </summary>
        public Roller Charges { get { return mCharges; } }

        /// <summary>
        /// Gets the <see cref="Attack"/> used to roll this item's attack.
        /// Used for damage for weapons, intensity of effect for some
        /// potions, etc. May be null if the item does not use it.
        /// </summary>
        public Attack Attack { get { return mAttack; } }

        /// <summary>
        /// Gets the amount of protected afforded by this item.
        /// </summary>
        public int Armor { get { return mArmor; } }

        /// <summary>
        /// Gets the radius of light emitted by this Item.
        /// </summary>
        public int LightRadius { get { return mLightRadius; } }

        public ChargeType ChargeType { get { return mChargeType; } }

        public ItemTarget Target { get { return mTarget; } }

        public ItemScript Use { get { return mUse; } }

        public int Price { get { return mPrice; } }

        /// <summary>
        /// Gets the type of ammunition this item either is or fires.
        /// </summary>
        public string Ammunition { get { return mAmmunition; } }

        public ItemType(Content content, string name, string category, string subcategory,
            object appearance, Roller quantity, Roller charges, Attack attack, int armor,
            ChargeType chargeType, ItemScript use, int price)
            : base(content)
        {
            mName = new Noun(name);
            mCategory = category;
            mSubcategory = subcategory;
            mAppearance = appearance;
            mQuantity = quantity;
            mCharges = charges;
            mAttack = attack;
            mArmor = armor;
            mChargeType = chargeType;
            mUse = use;
            mPrice = price;
        }

        public ItemScript GetHitScript(Element element)
        {
            if (mHitScripts.ContainsKey(element)) return mHitScripts[element];

            // no script
            return null;
        }

        public override string ToString()
        {
            return mName.Singular;
        }

        public void SetLightRadius(int light) { mLightRadius = light; }

        public void SetTarget(ItemTarget target) { mTarget = target; }

        public void SetAmmunition(string ammunition) { mAmmunition = ammunition; }

        public void SetHitScript(Element element, ItemScript script)
        {
            if (script != null)
            {
                mHitScripts[element] = script;
            }
        }

        #region IComparable<ItemType> Members

        public int CompareTo(ItemType other)
        {
            // compare the categories first
            int category = mCategory.CompareTo(other.mCategory);
            if (category != 0) return category;

            // then the prices
            if (mPrice != other.mPrice) return mPrice.CompareTo(other.mPrice);

            // then the names
            return Name.CompareTo(other.Name);
        }

        #endregion

        private Noun mName;
        private string mCategory;
        private string mSubcategory;
        private object mAppearance;
        private Roller mQuantity;
        private Roller mCharges;
        private Attack mAttack;
        private int mArmor;
        private int mLightRadius = -1; // default to no light
        private ChargeType mChargeType;
        private ItemTarget mTarget = ItemTarget.None;
        private ItemScript mUse;
        private int mPrice;
        private string mAmmunition;
        private readonly FlagCollection mFlags = new FlagCollection();
        private readonly Dictionary<Element, ItemScript> mHitScripts = new Dictionary<Element, ItemScript>();
    }
}
