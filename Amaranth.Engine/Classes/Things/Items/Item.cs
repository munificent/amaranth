using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    [Flags]
    public enum CreateItemOptions
    {
        None        = 0,
        ForcePower  = 1
    }

    public static class CreateItemOptionsExtensions
    {
        public static bool IsSet(this CreateItemOptions options, CreateItemOptions flags)
        {
            return (options & flags) == flags;
        }
    }
    
    [Flags]
    public enum ItemStringOptions
    {
        None,

        ShowBonuses,
        ShowQuantity,
        ShowCharges,

        Default = ShowBonuses | ShowQuantity | ShowCharges
    }

    public static class ItemStringOptionsExtensions
    {
        public static bool IsSet(this ItemStringOptions options, ItemStringOptions flags)
        {
            return (options & flags) == flags;
        }
    }

    [Serializable]
    public class Item : Thing, IComparable<Item>, ICollectible<ItemCollection, Item>
    {
        public const int MaxQuantity = 99;

        public static Item Random(Vec pos, ItemType type, int level)
        {
            Item item = new Item(pos, type);

            // let the level wander
            level = Rng.WalkLevel(level);

            // randomly give it a power
            if (Rng.Int(150) <= level)
            {
                PowerType powerType = type.Content.Powers.Random(level, item.Type.Supercategory, item.Type.Subcategory);

                if (powerType != null)
                {
                    item.mPower = new Power(powerType);
                }
            }

            return item;
        }

        internal event EventHandler Changed;

        #region INoun Members

        public override string NounText { get { return Name; } }
        public override Person Person { get { return Person.Third; } }
        public override string Pronoun { get { return "it"; } }
        public override string Possessive { get { return "its"; } }

        #endregion

        #region INamed Members

        public override string Name { get { return ToString(); } }

        #endregion

        public ItemType Type { get { return mType.Value; } }

        public int Quantity
        {
            get { return mQuantity; }
            set
            {
                if (mQuantity != value)
                {
                    mQuantity = value;
                    if (Changed != null) Changed(this, EventArgs.Empty);
                }
            }
        }

        public int Charges
        {
            get { return mCharges; }
            set
            {
                if (mCharges != value)
                {
                    mCharges = value;

                    if (Changed != null) Changed(this, EventArgs.Empty);
                }
            }
        }

        public Attack Attack
        {
            get
            {
                // get the base attack from the type
                Attack attack = Type.Attack;

                // apply the brand if there is one
                if ((mPower != null) && (mPower.Type.Element != null))
                {
                    attack = new Attack(attack, mPower.Type.Element.Value);
                }

                return attack;
            }
        }

        /// <summary>
        /// Gets the total armor provided by this item including base armor and
        /// any bonus.
        /// </summary>
        public int TotalArmor { get { return Type.Armor + ArmorBonus; } }

        /// <summary>
        /// Gets the strike bonus provided by this item.
        /// </summary>
        public int StrikeBonus
        {
            get
            {
                int bonus = 0;
                
                if (Attack != null) bonus += Attack.StrikeBonus;
                if (mPower != null) bonus += mPower.StrikeBonus;

                return bonus;
            }
        }

        /// <summary>
        /// Gets the damage bonus provided by this item.
        /// </summary>
        public float DamageBonus
        {
            get
            {
                float bonus = 1.0f;

                if (Attack != null) bonus *= Attack.DamageBonus;
                if (mPower != null) bonus *= mPower.DamageBonus;

                return bonus;
            }
        }

        /// <summary>
        /// Gets the armor bonus provided by this item.
        /// </summary>
        public int ArmorBonus
        {
            get
            {
                if (mPower != null) return mPower.ArmorBonus;

                return 0;
            }
        }

        /// <summary>
        /// Gets the <see cref="Stat"/> bonus provided by this item. Look at the
        /// flags to see which Stats are affected.
        /// </summary>
        public int StatBonus
        {
            get
            {
                if (mPower != null) return mPower.StatBonus;

                return 0;
            }
        }

        /// <summary>
        /// Gets the <see cref="Speed"/> bonus provided by this item.
        /// </summary>
        public int SpeedBonus
        {
            get
            {
                if (mPower != null) return mPower.SpeedBonus;

                return 0;
            }
        }

        /// <summary>
        /// Gets the raw price of this item, unadjusted by Charisma.
        /// </summary>
        public int Price { get { return Type.Price; } }

        /// <summary>
        /// Gets whether this item can be used.
        /// </summary>
        public bool CanUse { get { return Type.ChargeType != ChargeType.None; } }

        /// <summary>
        /// Gets the Power this item has.
        /// </summary>
        public Power Power { get { return mPower; } }

        /// <summary>
        /// Gets the radius of light emitted by this Item.
        /// </summary>
        public override int LightRadius
        {
            get
            {
                // no light radius for unlit lights
                if ((Type.ChargeType == ChargeType.Light) && (mCharges <= 0)) return -1;

                return Type.LightRadius;
            }
        }

        /// <summary>
        /// Gets the Dungeon that contains this Item. Will return <c>null</c> if the Item is being carried
        /// by an <see cref="Entity"/>.
        /// </summary>
        public override Dungeon Dungeon { get { return mCollection.Dungeon; } }

        /// <summary>
        /// Gets all of the flags this Item has, including both intrinsic and <see cref="Power"/>-derived
        /// flags.
        /// </summary>
        public IFlagCollection Flags
        {
            get
            {
                if (mPower == null) return Type.Flags;

                return new MergedFlagCollection(Type.Flags, mPower.Flags);
            }
        }

        public Item(Vec pos, ItemType type, Power power, int quantity, int charges)
            : base(pos)
        {
            mType = type;
            mPower = power;
            mQuantity = quantity;
            mCharges = charges;
        }

        public Item(Vec pos, ItemType type, int quantity) : this(pos, type, null, quantity, type.Charges.Roll()) { }

        public Item(Vec pos, ItemType type) : this(pos, type, type.Quantity.Roll()) { }

        public Item(Item clone, int quantity, int charges) : this(clone.Position, clone.Type, clone.Power, quantity, charges) { }

        /// <summary>
        /// Gives the Item a chance to perform idle actions.
        /// </summary>
        public Action TakeTurn(Game game)
        {
            switch (Type.ChargeType)
            {
                case ChargeType.Light:
                    if (mCharges > 0) return new BurnAction(game, this);
                    break;
            }

            return null;
        }

        public bool CanStack(Item item)
        {
            // types and powers must match
            if (Type != item.Type) return false;
            if (mPower != item.mPower) return false;

            //### bob: hackish, do not stack things with charges so we don't have
            // to compare charges. will need a cleaner solution at some point
            if (Type.ChargeType == ChargeType.Light) return false;
            if (Type.ChargeType == ChargeType.Multi) return false;

            return true;
        }

        /// <summary>
        /// Tries to combine the given item and this one into one stack.
        /// </summary>
        /// <param name="item">The item to stack with this one. Its quantity
        /// will be reduced.</param>
        /// <param name="quantity">The quantity to take from the given Item
        /// and add to this one.</param>
        /// <returns>The quantity stacked with this Item.</returns>
        public int Stack(Item item, int quantity)
        {
            if (!CanStack(item)) return 0;

            // quantity is limited by quantity being stacked and max stack size
            quantity = Math2.Min(item.mQuantity, quantity, MaxQuantity - mQuantity);

            // match
            mQuantity += quantity;
            item.mQuantity -= quantity;
            return quantity;
        }

        /// <summary>
        /// Splits this Item into two stacks.
        /// </summary>
        /// <param name="quantity">The quantity the created stack should have. This
        /// Item's quantity will be reduced by that amount.</param>
        /// <returns>The new stack.</returns>
        public Item SplitStack(int quantity)
        {
            if ((quantity <= 0) || (quantity > mQuantity)) throw new ArgumentOutOfRangeException("quantity");

            // divide the charges evenly between the stacks
            int charges = (mCharges * quantity) / mQuantity;

            Item split = new Item(this, quantity, charges);
            Quantity -= quantity;
            Charges -= charges;

            return split;
        }

        public override void Hit(Action action, Hit hit)
        {
            // if the item does something when hit, invoke it
            ItemScript script = Type.GetHitScript(hit.Attack.Element);
            if (script != null)
            {
                script.Invoke(action.Entity, this, action, null);
            }

            // apply elemental effects
            if (((hit.Attack.Element == Element.Water) ||
                 (hit.Attack.Element == Element.Acid)) && Flags.Has("dissolves"))
            {
                //### bob: should get a saving throw
                //### bob: hack. assumes it is on the ground
                action.Log(LogType.Message, this, "{subject} dissolve[s].");
                mCollection.Remove(this);
            }
            else if ((hit.Attack.Element == Element.Fire) && Flags.Has("burns up"))
            {
                //### bob: should get a saving throw
                //### bob: hack. assumes it is on the ground
                action.Log(LogType.Message, this, "{subject} burn[s] up.");
                mCollection.Remove(this);
            }
            else if ((hit.Attack.Element == Element.Fire) && Flags.Has("melts"))
            {
                //### bob: should get a saving throw
                //### bob: hack. assumes it is on the ground
                action.Log(LogType.Message, this, "{subject} melt[s].");
                mCollection.Remove(this);
            }
        }

        public bool Resists(Element element)
        {
            return Flags.Has("resist:" + element.ToString().ToLower());
        }

        public int GetStatBonus(Stat stat)
        {
            if (Flags.Has("raise:" + stat.Name.ToLower()))
            {
                return StatBonus;
            }

            // bonus does not apply to the given stat
            return 0;
        }

        public override string ToString()
        {
            return ToString(ItemStringOptions.Default);
        }

        public string ToString(int quantity)
        {
            return ToString(quantity, ItemStringOptions.Default);
        }

        public string ToString(ItemStringOptions options)
        {
            return ToString(Quantity, options);
        }

        public string ToString(int quantity, ItemStringOptions options)
        {
            StringBuilder builder = new StringBuilder();

            // show the quantity
            if (options.IsSet(ItemStringOptions.ShowQuantity))
            {
                if (quantity > 1)
                {
                    builder.Append(quantity.ToString());
                    builder.Append(" ");
                }
                else
                {
                    builder.Append("a ");
                }
            }

            // show the prefix power
            if ((mPower != null) && mPower.Type.IsPrefix)
            {
                builder.Append(mPower.Name);
                builder.Append(" ");
            }

            // show the name
            if (options.IsSet(ItemStringOptions.ShowQuantity))
            {
                if (quantity > 1)
                {
                    builder.Append(Type.Noun.Plural);
                }
                else
                {
                    builder.Append(Type.Noun.Singular);
                }
            }
            else
            {
                builder.Append(Type.Noun.Singular);
            }

            // show the suffix power
            if ((mPower != null) && !mPower.Type.IsPrefix)
            {
                builder.Append(" ");
                builder.Append(mPower.Name);
            }

            if (options.IsSet(ItemStringOptions.ShowBonuses))
            {
                // show the weapon stats
                if ((Type.Attack != null) || (StrikeBonus != 0) || (DamageBonus != 1.0f))
                {
                    bool needsSpace = false;
                    builder.Append(" ^k(^m");

                    if (Type.Attack != null)
                    {
                        builder.Append(Type.Attack.Damage.ToString());
                        needsSpace = true;
                    }

                    if (StrikeBonus != 0)
                    {
                        if (needsSpace) builder.Append(" ");

                        builder.Append(StrikeBonus.ToString("+##;-##;0"));
                        needsSpace = true;
                    }

                    if (DamageBonus != 1.0f)
                    {
                        if (needsSpace) builder.Append(" ");

                        builder.AppendFormat("x{0}", DamageBonus);
                    }

                    builder.Append("^k)^-");
                }

                // show the armor stats
                if ((Type.Armor != 0) || (ArmorBonus != 0))
                {
                    bool needsSpace = false;
                    builder.Append(" ^k[^m");

                    if (Type.Armor != 0)
                    {
                        builder.Append(Type.Armor.ToString());
                        needsSpace = true;
                    }

                    if (ArmorBonus != 0)
                    {
                        if (needsSpace) builder.Append(" ");

                        builder.Append(ArmorBonus.ToString("+##;-##;0"));
                    }

                    builder.Append("^k]^-");
                }

                // show the stat bonus
                if (StatBonus != 0)
                {
                    builder.Append(" ^k(^m");
                    builder.Append(StatBonus.ToString("+##;-##;0"));
                    builder.Append("^k)^-");
                }

                // show the speed bonus
                if (SpeedBonus != 0)
                {
                    builder.Append(" ^k(^m");
                    builder.Append(SpeedBonus.ToString("+##;-##;0"));
                    builder.Append(" speed^k)^-");
                }
            }

            if (options.IsSet(ItemStringOptions.ShowCharges))
            {
                switch (Type.ChargeType)
                {
                    case ChargeType.Light:
                        if (mCharges == 0)
                        {
                            builder.Append(" ^k(^mempty^k)^-");
                        }
                        else if (mCharges > 0)
                        {
                            builder.AppendFormat(" ^k(^ylit^m {0} left^k)^-", mCharges);
                        }
                        else
                        {
                            builder.AppendFormat(" ^k(^m{0} left^k)^-", -mCharges);
                        }
                        break;

                    case ChargeType.Multi:
                        if (mCharges == 0)
                        {
                            builder.Append(" ^k(^mempty^k)^-");
                        }
                        else
                        {
                            builder.AppendFormat(" ^k(^m{0} charges^k)^-", mCharges);
                        }
                        break;
                }
            }

            return builder.ToString();
        }

        #region IComparable<Item> Members

        public int CompareTo(Item other)
        {
            // first sort by type
            int compare = Type.CompareTo(other.Type);
            if (compare != 0) return compare;

            // if those are the same, sort by quantity (greatest first)
            return -mQuantity.CompareTo(other.mQuantity);
        }

        #endregion

        #region ICollectible<ItemCollection,Item> Members

        void ICollectible<ItemCollection, Item>.SetCollection(ItemCollection collection)
        {
            mCollection = collection;
        }

        #endregion

        private ItemTypeRef mType;
        private ItemCollection mCollection;

        private Power mPower;
        private int mQuantity;
        private int mCharges;
    }
}
