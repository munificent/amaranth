using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// A Race defines a type of <see cref="Monster"/>: blue dragon, troll, etc.
    /// </summary>
    public class Race : ContentBase, IComparable<Race>
    {
        public static Race Random(Dungeon dungeon, int level, bool allowUniques)
        {
            // let the level wander
            level = Rng.WalkLevel(level).Clamp(1, 100);

            // pick a race
            Race race = null;
            while (race == null)
            {
                Race thisRace = dungeon.Game.Content.Races.RandomSoft(level);

                // don't generate multiple uniques
                if (thisRace.IsUnique)
                {
                    if (!allowUniques) continue;

                    // already dead
                    if (dungeon.Game.Hero.HasSlain(thisRace)) continue;

                    // already on the dungeon
                    if (dungeon.Entities.OfType<Monster>().Any(thisMonster => thisMonster.Race == thisRace)) continue;
                }

                // if we got here, it's ok
                race = thisRace;
            }

            return race;
        }

        public override string Name { get { return mName; } }

        public object Appearance { get { return mAppearance; } }

        public int Speed { get { return mSpeed; } }

        public Roller Health { get { return mHealth; } }

        public IList<Attack> Attacks { get { return mAttacks; } }

        public IList<Move> Moves { get { return mMoves; } }

        public int Depth { get { return mDepth; } }

        public IDrop<Item> Drop { get { return mDrop; } }

        public bool IsUnique { get { return mFlags.IsSet(RaceFlags.Unique); } }
        public bool OpensDoors { get { return mFlags.IsSet(RaceFlags.OpensDoors); } }
        public bool IsBoss { get { return mFlags.IsSet(RaceFlags.Boss); } }

        public int LightRadius { get { return mLightRadius; } }

        /// <summary>
        /// Gets the average size of the group of monsters generated for this race. Will be 1
        /// for races that do not appear in groups.
        /// </summary>
        public int NumberInGroup
        {
            get
            {
                switch (mGroupSize)
                {
                    case GroupSize.Single: return 1;
                    case GroupSize.Group: return 6;
                    case GroupSize.Pack: return 12;
                    case GroupSize.Swarm: return 18;
                    case GroupSize.Horde: return 30;
                    default: throw new UnexpectedEnumValueException(mGroupSize);
                }
            }
        }

        public string[] Groups { get { return mGroups; } }

        public IList<Item> PlaceDrop(Dungeon dungeon, Vec dropPosition)
        {
            IList<Item> items = new List<Item>();

            if (Drop != null)
            {
                foreach (Item item in Drop.Create(Depth))
                {
                    Vec position = dungeon.GetOpenItemPosition(dropPosition);
                    item.ForcePosition(position);
                    dungeon.Items.Add(item);

                    items.Add(item);
                }
            }

            return items;
        }

        /// <summary>
        /// Gets a colored text description of this Monster.
        /// </summary>
        /// <remarks>
        /// Most of the text is left uncolored. Attributes that make a monster more dangerous
        /// (i.e. give it a > 1.0 exp multiplier) are usually colored warm, and attributes that
        /// make a monster weaker are cool. Primary attributes are red or green. Flags are cyan
        /// or orange. Neutral but important attributes are white or another color.
        /// </remarks>
        public string GetDescription(Hero hero)
        {
            StringBuilder builder = new StringBuilder();

            // flavor description
            builder.Append(mDescription);

            // level and exp
            builder.Append(" It is normally found on level ^w" + mDepth + "^- and is worth ^b");
            builder.Append(hero.GetExperience(this).ToString("F1"));
            builder.Append("^- experience.");

            // speed and pursuit
            string moves = String.Empty;
            string pursue = String.Empty;
            string withoutSpeed = String.Empty;

            switch (mSpeed)
            {
                case 0: moves = " It moves ^gincredibly slowly^-"; break;
                case 1: moves = " It moves ^gvery slowly^-"; break;
                case 2: moves = " It moves ^gquite slowly^-"; break;
                case 3: moves = " It moves ^gslowly^-"; break;
                case 4: moves = " It moves ^gsomewhat slowly^-"; break;
                case 5: moves = " It moves ^gslightly slowly^-"; break;
                case 6: break; // normal speed
                case 7: moves = " It moves ^ra little quickly^-"; break;
                case 8: moves = " It moves ^rsomewhat quickly^-"; break;
                case 9: moves = " It moves ^rquickly^-"; break;
                case 10: moves = " It moves ^rquite quickly^-"; break;
                case 11: moves = " It moves ^rvery quickly^-"; break;
                case 12: moves = " It moves ^rincredibly quickly^-"; break;
            }

            switch (mPursue)
            {
                case Pursue.SlightlyErratically:
                    pursue = " and ^cslightly erratically^-.";
                    withoutSpeed = " It moves ^cslightly erratically^-.";
                    break;
                case Pursue.Erratically:
                    pursue = " and ^cerratically^-.";
                    withoutSpeed = " It moves ^cerratically^-.";
                    break;
                case Pursue.VeryErratically:
                    pursue = " and ^cvery erratically^-.";
                    withoutSpeed = " It moves ^cvery erratically^-.";
                    break;
                case Pursue.Unmoving:
                    pursue = " but ^cdoes not chase^- intruders.";
                    withoutSpeed = " It ^cdoes not chase^- intruders.";
                    break;
            }

            if (moves == String.Empty)
            {
                if (withoutSpeed != String.Empty) builder.Append(withoutSpeed);
            }
            else
            {
                if (pursue == String.Empty)
                {
                    builder.Append(moves + ".");
                }
                else
                {
                    builder.Append(moves + pursue);
                }
            }

            // group size
            switch (mGroupSize)
            {
                case GroupSize.Single: break; // default
                case GroupSize.Group:  builder.Append(" It travels in ^osmall groups^-."); break;
                case GroupSize.Pack:   builder.Append(" It travels in ^ogroups^-."); break;
                case GroupSize.Swarm:  builder.Append(" It travels in ^olarge groups^-."); break;
                case GroupSize.Horde:  builder.Append(" It travels in ^ohuge groups^-."); break;
                default: throw new UnexpectedEnumValueException(mGroupSize);
            }

            // light
            switch (mLightRadius)
            {
                case 2: builder.Append(" It ^cglows brightly^-."); break;
                case 1: builder.Append(" It ^cglows^-."); break;
                case 0: builder.Append(" It ^ccan be seen in the dark^-."); break;
            }

            if (IsUnique) builder.Append(" It is ^bunique^-.");
            if (IsBoss) builder.Append(" It is ^wyour destiny to kill it^-.");
            if (OpensDoors) builder.Append(" It can ^copen doors^-.");

            foreach (Move move in mMoves)
            {
                builder.Append(" ");
                builder.Append(move.Description);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Gets the amount of experience awarded to a level 1 <see cref="Hero"/> for
        /// killing a <see cref="Monster"/> of this <see cref="Race"/>.
        /// </summary>
        public float Experience
        {
            get
            {
                // start with a small constant such that a 1 hp monster at
                // normal speed with 1 attack for 1 damage yields 1 exp
                float exp = 1.0f / Energy.GetGain(Energy.NormalSpeed);

                // the more health it has, the longer it can hurt the hero
                exp *= mHealth.Average;

                // the more energy it gains, the more it can do per turn
                exp *= Energy.GetGain(mSpeed);

                float movesTotal = 0;
                float remainingOdds = 1.0f;
                foreach (Move move in mMoves)
                {
                    float odds = (float)move.Info.Chance / (float)move.Info.Range;

                    // get the experience for the move and reduce it by the chance of it happening
                    float moveExperience = move.GetExperience(this) * odds;

                    // also reduce by the odds of even getting here
                    moveExperience *= remainingOdds;

                    // add it to the total exp for moves
                    movesTotal += moveExperience;

                    // filter out this move's chance from what's left
                    remainingOdds *= (1.0f - odds);
                }

                // skip the attacks if the moves leave no chance to perform them
                if (remainingOdds > 0.0f)
                {
                    float damageTotal = 0;

                    foreach (Attack attack in mAttacks)
                    {
                        float damage = attack.Average;
                        //### bob: take into account strike bonus?

                        // take into account damage element
                        damage *= attack.Element.AttackExperience();

                        foreach (string flag in attack.Flags)
                        {
                            switch (flag)
                            {
                                case "drain:experience":    damage += 60.0f; damage *= 4.0f; break;
                                case "drain:strength":      damage += 50.0f; damage *= 3.0f; break;
                                case "drain:stamina":       damage += 50.0f; damage *= 3.0f; break;
                                case "drain:agility":       damage += 50.0f; damage *= 3.0f; break;
                                case "drain:will":          damage += 50.0f; damage *= 3.0f; break;
                                case "drain:intellect":     damage += 50.0f; damage *= 3.0f; break;
                                case "drain:charisma":      damage += 30.0f; damage *= 2.0f; break;
                                case "slow":                damage += 10.0f; damage *= 1.5f; break;
                                case "paralyze":            damage += 10.0f; damage *= 5.0f; break;
                                case "disease":             damage += 20.0f; damage *= 2.0f; break;
                                default: throw new Exception("Unknown attack flag \"" + flag + "\".");
                            }
                        }

                        damageTotal += damage;
                    }

                    // each attack happens only a fraction of the time
                    damageTotal /= mAttacks.Count;

                    // reduce the odds of an attack by the moves before it
                    damageTotal *= remainingOdds;

                    // combine with the moves
                    movesTotal += damageTotal;
                }

                // take into account everything the monster can do
                exp *= movesTotal;

                // pursuit
                switch (mPursue)
                {
                    case Pursue.Unmoving: exp *= 0.3f; break;
                    case Pursue.SlightlyErratically: exp *= 0.8f; break;
                    case Pursue.Erratically: exp *= 0.7f; break;
                    case Pursue.VeryErratically: exp *= 0.5f; break;
                }

                // factor in the resistances
                // the scale factor here is basically "how common is a
                // hero attack of that type". the more common, then the
                // more effective a resist of that type is for the
                // monster, so the greater the exp multiplier for
                // killing a race with it.
                exp *= GetResistExperience(Element.Air, 1.0f);
                exp *= GetResistExperience(Element.Earth, 1.0f);
                exp *= GetResistExperience(Element.Fire, 1.0f);
                exp *= GetResistExperience(Element.Water, 1.0f);

                exp *= GetResistExperience(Element.Metal, 1.5f);
                exp *= GetResistExperience(Element.Wood, 1.5f);

                exp *= GetResistExperience(Element.Acid, 0.8f);
                exp *= GetResistExperience(Element.Cold, 0.8f);
                exp *= GetResistExperience(Element.Lightning, 0.8f);
                exp *= GetResistExperience(Element.Poison, 0.6f);

                exp *= GetResistExperience(Element.Dark, 0.7f);
                exp *= GetResistExperience(Element.Light, 0.7f);

                exp *= GetResistExperience(Element.Anima, 1.2f);
                exp *= GetResistExperience(Element.Death, 0.6f);

                // group size
                switch (mGroupSize)
                {
                    case GroupSize.Single:               break; // default
                    case GroupSize.Group:   exp *= 1.3f; break;
                    case GroupSize.Pack:    exp *= 1.8f; break;
                    case GroupSize.Swarm:   exp *= 2.3f; break;
                    case GroupSize.Horde:   exp *= 3.0f; break;
                    default: throw new UnexpectedEnumValueException(mGroupSize);
                }

                return exp;
            }
        }

        public Race(Content content, string name, int depth, object appearance, int speed, Roller health)
            : base(content)
        {
            mName = name;
            mDepth = depth;
            mAppearance = appearance;
            mSpeed = speed;

            // if the health is fixed, then automatically create a range for it
            if (health.IsFixed)
            {
                int amount = health.Roll();
                int range = (int)(amount * 0.2f);

                if (range > 0)
                {
                    mHealth = Roller.Triangle(amount, range);
                }
                else
                {
                    mHealth = Roller.Fixed(amount);
                }
            }
            else
            {
                mHealth = health;
            }

            mDescription = "A nondescript beast of the dungeon.";
        }

        public override string ToString()
        {
            return mName;
        }

        public Behavior CreateBehavior(Monster monster)
        {
            IPathfinder pathfinder;

            switch (mPursue)
            {
                case Pursue.Unmoving: pathfinder = new UnmovingPathfinder(); break;
                default: pathfinder = new StraightTowardsPathfinder(mPursue); break;
            }

            return new MonsterBehavior(monster, pathfinder);
        }

        public bool IsInGroup(string group)
        {
            foreach (string thisGroup in mGroups)
            {
                if (thisGroup == group) return true;
            }

            return false;
        }

        //### bob: these methods exist only to be called by the data loading code to set up a race instead
        //         of passing in a pile of constructor parameters. this is kind of gross since it means
        //         races technically are mutable. replace these with a RaceInfo constructor parameter instead?
        #region Initializers

        public void SetDrop(IDrop<Item> drop)
        {
            mDrop = drop;
        }

        public void SetPursue(Pursue pursue)
        {
            mPursue = pursue;
        }

        public void SetDescription(string description)
        {
            mDescription = description;
        }

        public void SetResist(Element element, RaceResist resistance)
        {
            mResists[element] = resistance;
        }

        public void SetGroupSize(GroupSize size)
        {
            mGroupSize = size;
        }

        public void SetLightRadius(int radius)
        {
            mLightRadius = radius;
        }

        public void SetFlag(RaceFlags flags)
        {
            mFlags |= flags;
        }

        public void SetGroups(string[] groups)
        {
            mGroups = groups;
        }

        #endregion

        public RaceResist GetResistance(Element element)
        {
            if (mResists.ContainsKey(element))
            {
                return mResists[element];
            }

            // default to no resistance
            return RaceResist.None;
        }

        private float GetResistExperience(Element element, float scale)
        {
            switch (GetResistance(element))
            {
                case RaceResist.Immune:      return 1.0f + 0.6f * scale;
                case RaceResist.VeryStrong:  return 1.0f + 0.2f * scale;
                case RaceResist.Strong:      return 1.0f + 0.1f * scale;
                case RaceResist.None:        return 1.0f;
                case RaceResist.Weak:        return 1.0f - 0.1f * scale;
                case RaceResist.VeryWeak:    return 1.0f - 0.3f * scale;
                default: throw new UnexpectedEnumValueException(GetResistance(element));
            }
        }

        #region IComparable<Race> Members

        public int CompareTo(Race other)
        {
            return mName.CompareTo(other.mName);
        }

        #endregion

        private string mName;
        private int mDepth;
        private object mAppearance;
        private int mSpeed;
        private IDrop<Item> mDrop;
        private string mDescription;
        private readonly Roller mHealth;
        private int mLightRadius = -1;
        private readonly List<Attack> mAttacks = new List<Attack>();
        private readonly List<Move> mMoves = new List<Move>();
        private readonly Dictionary<Element, RaceResist> mResists = new Dictionary<Element, RaceResist>();
        private GroupSize mGroupSize = GroupSize.Single;
        private Pursue mPursue = Pursue.Closely;
        private RaceFlags mFlags = RaceFlags.Default;
        private string[] mGroups = new string[0];
    }
}
