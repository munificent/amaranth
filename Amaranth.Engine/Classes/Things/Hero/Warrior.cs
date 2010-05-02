using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// <para>
    /// The Warrior class. The warrior play style is about straight-forward
    /// running around and killing things. Warrior players don't want to
    /// have to spend a lot of time selecting different actions, spells, etc.
    /// Instead, they want to slice their way through the dungeon and grow
    /// more powerful automatically.
    /// </para>
    /// <para>
    /// To enable that, the Warrior HeroClass features passive abilities
    /// that work automatically. The first is monster lore. Each time a
    /// warrior slays a monster, he gains a little proficiency in killing
    /// monsters of that group. That, in turn, is reflected as a bonus to
    /// hit and damage when later attacking monsters of that race and group.
    /// </para>
    /// </summary>
    [Serializable]
    public class Warrior : HeroClass
    {
        /// <summary>
        /// Gets the warrior's slay lore. For each monster group that a
        /// Warrior has slain at least one of, this will show the group
        /// name, the number of slain monsters of that group, and the
        /// lore level for the group.
        /// </summary>
        public IEnumerable<SlayLore> SlayLore
        {
            get
            {
                foreach (var pair in mSlays)
                {
                    yield return new SlayLore(pair.Key, pair.Value, GetLevel(pair.Value));
                }
            }
        }

        /// <summary>
        /// Overridden from HeroClass.
        /// </summary>
        public override void KilledMonster(Action action, Monster monster)
        {
            foreach (string group in monster.Race.Groups)
            {
                int oldCount = 0;
                int newCount = 1;
                if (mSlays.TryGetValue(group, out oldCount))
                {
                    newCount = oldCount + 1;
                }

                mSlays[group] = newCount;

                // tell the player if they levelled up
                int oldLevel = GetLevel(oldCount);
                int newLevel = GetLevel(newCount);

                if (newLevel > oldLevel)
                {
                    action.Log(LogType.PermanentGood, "You have gotten better at slaying " + group + "!");
                }
            }
        }

        /// <summary>
        /// Overridden from HeroClass.
        /// </summary>
        public override void BeforeAttack(Entity defender, Attack attack)
        {
            // only care about attacking monsters
            var monster = defender as Monster;
            if (monster == null) return;

            // add up the slay ability
            int level = 0;
            foreach (string group in monster.Race.Groups)
            {
                int slays;
                if (mSlays.TryGetValue(group, out slays))
                {
                    level += GetLevel(slays);
                }
            }

            level = Math.Min(level, MaxLevel);

            // can multiply the damage by up to 5x
            attack.DamageBonus *= Math2.Remap(0, level, MaxLevel, 1.0f, 5.0f);

            // can give up to +5 to strike
            attack.StrikeBonus += Math2.Remap(0, level, MaxLevel, 0, 5);
        }

        private int GetLevel(int count)
        {
            // levels follow a geometric progression. where each successive level
            // requires ten more kills than the previous one. so, 10 kills will
            // get the player to level 1, 30 kills (20 more) to level 2, 60 kills
            // (30 more) to level 3, etc.
            int level = 0;
            int step = 10;

            while ((count >= step) && (level <= MaxLevel))
            {
                level++;
                count -= step;
                step += 10;
            }

            return level;
        }

        private const int MaxLevel = 10;

        private readonly SortedDictionary<string, int> mSlays = new SortedDictionary<string, int>();
    }

    public class SlayLore
    {
        public string Group { get; private set; }
        public int Slain { get; private set; }
        public int Level { get; private set; }

        public SlayLore(string group, int slain, int level)
        {
            Group = group;
            Slain = slain;
            Level = level;
        }
    }
}
