using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// A monster is a CPU-controlled <see cref="Entity"/> in the game of a specific <see cref="Race"/>.
    /// </summary>
    [Serializable]
    public class Monster : Entity
    {
        /// <summary>
        /// Generates usually one random Monster chosen appropriately for the given depth. May
        /// generate more than one if the Monster chosen has escorts or appears in groups. Adds
        /// them to the current dungeon.
        /// </summary>
        /// <param name="level">Dungeon level to generate at.</param>
        /// <returns>The new Monsters that were added to the Dungeon.</returns>
        public static IList<Monster> AddRandom(Dungeon dungeon, int level, Vec startPos)
        {
            Race race = Race.Random(dungeon, level, true);

            List<Monster> monsters = new List<Monster>();

            // create the monster(s)
            Monster monster = new Monster(startPos, race);
            monsters.Add(monster);
            dungeon.Entities.Add(monster);

            // generate friends
            if (race.NumberInGroup > 1)
            {
                int numMonsters = Rng.TriangleInt(race.NumberInGroup, race.NumberInGroup / 3);
                int tries = 0;
                while ((monsters.Count < numMonsters) && (tries < 100))
                {
                    tries++;

                    // pick a random spot next to one of the monsters in the group
                    Vec pos;
                    if (dungeon.TryFindOpenAdjacent(Rng.Item(monsters).Position, out pos))
                    {
                        // found one, so put another there
                        Monster buddy = new Monster(pos, race);
                        monsters.Add(buddy);
                        dungeon.Entities.Add(buddy);
                    }
                }
            }

            return monsters;
        }

        public static IList<Monster> AddRandom(Dungeon dungeon, int level)
        {
            return AddRandom(dungeon, level, dungeon.RandomFloor());
        }

        #region INoun Members

        public override string NounText
        {
            get
            {
                // don't add an article to a proper noun
                if (Race.IsUnique) return Race.Name;

                return "the " + Race.Name;
            }
        }

        public override Person Person { get { return Person.Third; } }
        //### bob: handle gender and proper nouns
        public override string Pronoun { get { return "it"; } }
        public override string Possessive { get { return "its"; } }

        #endregion

        #region INamed Members

        public override string Name { get { return Race.Name; } }

        #endregion

        public Race Race { get { return mRace.Value; } }

        /// <summary>
        /// Gets a colored text description of this Monster.
        /// </summary>
        public string GetDescription(Hero hero) { return Race.GetDescription(hero); }

        public override int LightRadius { get { return Race.LightRadius; } }

        public override bool OpensDoors { get { return Race.OpensDoors; } }

        public Monster(Vec pos, Race race)
            : base(pos, race.Speed, race.Health.Roll())
        {
            mRace = race;

            SetBehavior(race.CreateBehavior(this));

            // shuffle its energy. makes sure all monsters don't step at the same time
            Energy.Randomize();
        }

        /// <summary>
        /// Gets the Monster instance-specific move data for the given type.
        /// </summary>
        /// <typeparam name="T">Type of move data.</typeparam>
        public T GetMoveInstance<T>() where T : new()
        {
            Type type = typeof(T);

            // lazy create
            if (!mMoveInstances.ContainsKey(type)) mMoveInstances[type] = new T();

            return (T)mMoveInstances[type];
        }

        /// <summary>
        /// Gets whether the tile specified by moving the Monster the given distance is occupied
        /// by a Monster that is neither this one or the target. Used to determine if the
        /// Monster is trying to walk into a friendly.
        /// </summary>
        /// <param name="move">The distance the Monster will move.</param>
        /// <param name="target">The Entity the Monster would like to hit.</param>
        /// <returns><c>true</c> if the destination square does not have another Monster in it.</returns>
        public bool IsOccupiedByOtherMonster(Vec move, Entity target)
        {
            Entity occupier = Dungeon.Entities.GetAt(Position + move);
            if ((occupier != null) && (occupier != this) && (occupier != target))
            {
                return true;
            }

            return false;
        }

        public override Attack GetAttack(Entity defender)
        {
            // pick one randomly
            return Rng.Item(Race.Attacks);
        }

        protected override float OnGetResistance(Element element)
        {
            switch (Race.GetResistance(element))
            {
                case RaceResist.Immune: return 0.0f;
                case RaceResist.VeryStrong: return 0.25f;
                case RaceResist.Strong: return 0.5f;
                case RaceResist.None: return 1.0f;
                case RaceResist.Weak: return 2.0f;
                case RaceResist.VeryWeak: return 4.0f;
                default: throw new UnexpectedEnumValueException(Race.GetResistance(element));
            }
        }

        protected override float OnGetSlays(IFlagCollection flags)
        {
            float slays = 1.0f;

            // see if the attack has any flags that affect this monster's group(s)
            foreach (string group in Race.Groups)
            {
                if (flags.Has("useless-against-" + group)) slays = 0.0f;
                if (flags.Has("weak-against-" + group)) slays *= 0.5f;
                if (flags.Has("hurts-" + group)) slays *= 2.0f;
                if (flags.Has("wounds-" + group)) slays *= 3.0f;
                if (flags.Has("slays-" + group)) slays *= 4.0f;
            }

            return slays;
        }

        protected override bool OnDie(Action action)
        {
            // drop stuff
            Race.PlaceDrop(Dungeon, Position);

            // monsters can die
            return true;
        }

        private RaceRef mRace;

        private readonly Dictionary<Type, object> mMoveInstances = new Dictionary<Type,object>();
    }
}
