using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// Maintains a collection of objects with rarity and level, and chooses randomly from them
    /// with appropriate weighted distributions.
    /// </summary>
    public class LevelChooser<T> : IEnumerable<T> where T : INamed
    {
        public int Count { get { return mObjects.Count; } }

        public LevelChooser()
        {
        }

        /// <summary>
        /// Adds the item with the given level and rarity to the collection.
        /// </summary>
        public void Add(T obj, int level, int rarity)
        {
            mObjects.Add(new RareObject(obj, level, level, rarity));
        }

        /// <summary>
        /// Adds the item with the given range of levels and rarity to the collection.
        /// </summary>
        public void Add(T obj, int minLevel, int maxLevel, int rarity)
        {
            //### bob: weight the rarity?
            mObjects.Add(new RareObject(obj, minLevel, maxLevel, rarity));
        }

        /// <summary>
        /// Chooses a random item of the given level.
        /// </summary>
        public T Random(int level)
        {
            return Random(level, (obj) => true);
        }

        /// <summary>
        /// Chooses a random item from a distribution of levels surrounding
        /// the given level.
        /// </summary>
        public T RandomSoft(int level)
        {
            // figure out the level range to choose from
            int minLevel = Math.Max(1, (int)(level * 0.9f));
            int maxLevel = Math.Min(100, (int)(level * 1.1f));
            
            // choose an item from the level range
            return Random((rareObj) => (rareObj.MinLevel <= maxLevel) && (rareObj.MaxLevel >= minLevel));
        }

        public int GetLevel(T item)
        {
            foreach (RareObject obj in mObjects)
            {
                //### bob: hack. assume minLevel = level
                if (ReferenceEquals(obj.Object, item)) return obj.MinLevel;
            }

            // not found
            throw new Exception("The item was not found in the LevelChooser.");
        }

        /// <summary>
        /// Chooses a random item of the given level from the set of items that
        /// match the given predicate.
        /// </summary>
        protected T Random(int level, Func<T, bool> predicate)
        {
            // exponential chance of reducing the chosen level
            while ((level > 1) && Rng.OneIn(3)) level--;

            // chance of increasing the chosen level
            while ((level <= Game.MaxDepth) && Rng.OneIn(5)) level++;

            // choose a matching item from the level
            return Random((rareObj) =>
                (rareObj.MinLevel <= level) &&
                (rareObj.MaxLevel >= level) &&
                predicate(rareObj.Object));
        }

        private T Random(Func<RareObject, bool> predicate)
        {
            // collect all of the possible choices
            float totalCommonness = 0;

            List<RareObject> matches = new List<RareObject>();

            foreach(RareObject obj in mObjects)
            {
                if (predicate(obj))
                {
                    matches.Add(obj);
                    totalCommonness += obj.Commonness;
                }
            }

            // bail if there are no possibilities
            if (matches.Count == 0) return default(T);

            // choose one randomly
            float choice = Rng.Float(totalCommonness);

            foreach (RareObject obj in matches)
            {
                if (choice <= obj.Commonness)
                {
                    return obj.Object;
                }

                choice -= obj.Commonness;
            }

            // should not get here
            throw new Exception("Something is broke in the object selection algo.");
        }

        private class RareObject
        {
            public readonly T Object;
            public readonly int MinLevel;
            public readonly int MaxLevel;
            public readonly float Commonness;

            public RareObject(T obj, int minLevel, int maxLevel, int rarity)
            {
                Object = obj;
                MinLevel = minLevel;
                MaxLevel = maxLevel;
                Commonness = 1.0f / rarity;
            }

            public override string ToString()
            {
                return Object.ToString() + " (" + MinLevel + "-" + MaxLevel + ")";
            }
        }

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            foreach (RareObject obj in mObjects)
            {
                yield return obj.Object;
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        private readonly List<RareObject> mObjects = new List<RareObject>();
    }
}
