using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// Container class for the basic <see cref="Hero"/> stats: strength, agility, etc.
    /// </summary>
    /// <remarks>
    /// <para>Note that the Max value here is the hero's current max value for that stat.
    /// In other words, the Max is the value a drained stat will be restored too, not the
    /// absolutely highest value any stat can have. All stats range from 1 to 50.
    /// 15 is considered "normal" for a hero. New heroes will likely have stats ranging
    /// from 10 to 20. </para>
    /// <para>Below 10 usually implies a magical effect that is temporarily lowering
    /// it to something below normal human level. For example, a Strength of 9 would
    /// mean the hero is a weakling, able to hold a sword but barely swing it. A
    /// Strength of 1 means the hero is near paralysis.</para>
    /// <para>Likewise, 20 means the hero is at the upper end of the average human.
    /// A Strength of 20 means the hero is an athlete in peak condition. 30 means
    /// the seasoned hero is stronger than mere mortals, and close to 50 means a battle-
    /// hardened warrior augmented with legendary artifacts.</para>
    /// </remarks>
    [Serializable]
    public class Stats : IList<Stat>
    {
        public Strength Strength { get { return mStrength; } }
        public Agility Agility { get { return mAgility; } }
        public Stamina Stamina { get { return mStamina; } }
        public Will Will { get { return mWill; } }
        public Intellect Intellect { get { return mIntellect; } }
        public Charisma Charisma { get { return mCharisma; } }

        public Stats()
        {
            // reset the stats and then 
            foreach (Stat stat in this)
            {
                stat.Base = 5;
            }

            // randomly add points until a total of 15 points per stat have been distributed
            for (int i = 0; i < 10 * Count; i++)
            {
                Rng.Item(this).Base++;
            }
        }

        private readonly Strength mStrength = new Strength();
        private readonly Agility mAgility = new Agility();
        private readonly Stamina mStamina = new Stamina();
        private readonly Will mWill = new Will();
        private readonly Intellect mIntellect = new Intellect();
        private readonly Charisma mCharisma = new Charisma();

        #region IList<Stat> Members

        public int IndexOf(Stat item)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i] == item) return i;
            }

            return -1;
        }

        public void Insert(int index, Stat item) { throw new NotSupportedException(); }

        public void RemoveAt(int index) { throw new NotSupportedException(); }

        public Stat this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return mStrength;
                    case 1: return mAgility;
                    case 2: return mStamina;
                    case 3: return mWill;
                    case 4: return mIntellect;
                    case 5: return mCharisma;
                    default: throw new ArgumentOutOfRangeException("index");
                }
            }
            set { throw new NotSupportedException(); }
        }

        #endregion

        #region ICollection<Stat> Members

        public void Add(Stat item) { throw new NotSupportedException(); }

        public void Clear() { throw new NotSupportedException(); }

        public bool Contains(Stat item) { throw new NotImplementedException(); }

        public void CopyTo(Stat[] array, int arrayIndex) { throw new NotImplementedException(); }

        public int Count { get { return 6; } }

        public bool IsReadOnly { get { return true; } }

        public bool Remove(Stat item) { throw new NotSupportedException(); }

        #endregion

        #region IEnumerable<Stat> Members

        public IEnumerator<Stat> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return this[i];
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }

    [Serializable]
    public class Strength : Stat
    {
        public float DamageBonus { get { return StatTable.GetPointOneToTen(Current); } }
    }

    [Serializable]
    public class Agility : Stat
    {
        public int StrikeBonus { get { return StatTable.GetNegativeFiftyToHundred(Current); } }
    }

    [Serializable]
    public class Stamina : Stat
    {
        public float HealthBonus { get { return StatTable.GetNegativeThreeToTen(Current); } }
    }

    [Serializable]
    public class Will : Stat
    {
        public bool ResistDrain()
        {
            //### bob enemy will should affect
            return Rng.Int(100) < Current;
        }
    }

    [Serializable]
    public class Intellect : Stat
    {
    }

    [Serializable]
    public class Charisma : Stat
    {
        public float BuyPriceMultiplier { get { return 1.0f / StatTable.GetPointFiveToTwo(Current); } }
        public float SellPriceMultiplier { get { return StatTable.GetPointFiveToTwo(Current); } }
    }
}
