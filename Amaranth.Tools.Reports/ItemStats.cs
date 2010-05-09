using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;

using Amaranth.Data;
using Amaranth.Engine;
using Amaranth.Terminals;
using Amaranth.Util;

namespace Amaranth.Reports
{
    public class AsyncStats
    {
        public event EventHandler Updated;

        public bool MaxPerLevel = false;

        public int MaxRaceCount = 0;
        public int MaxItemCount = 0;
        public int MaxPowerCount = 0;

        public int[] MaxRaceCounts = new int[100];
        public int[] MaxItemCounts = new int[100];
        public int[] MaxPowerCounts = new int[100];

        public IEnumerable<IStatRow> StatRows
        {
            get
            {
                foreach (RaceStat stat in mRaces.Values)
                {
                    yield return stat;
                }

                foreach (ItemStat stat in mItems.Values)
                {
                    yield return stat;
                }

                foreach (PowerStat stat in mPowers.Values)
                {
                    yield return stat;
                }

                yield return mMonsterHealth;
            }
        }

        public AsyncStats()
        {
            mContent = DataFiles.Load();

            foreach (Race race in mContent.Races)
            {
                mRaces[race] = new RaceStat(this, race);
            }

            foreach (ItemType type in mContent.Items)
            {
                mItems[type] = new ItemStat(this, type);
            }

            foreach (PowerType type in mContent.Powers)
            {
                mPowers[type] = new PowerStat(this, type);
            }

            mMonsterHealth = new AverageStat("Monster health");
        }

        public void Run()
        {
            if (mState == RunState.NotStarted)
            {
                ThreadStart start = new ThreadStart(GenerateStats);
                Thread thread = new Thread(start);
                thread.Priority = ThreadPriority.AboveNormal;

                mState = RunState.Running;
                thread.Start();
            }
        }

        public void Stop()
        {
            mState = RunState.Stopped;
        }

        private void GenerateStats()
        {
            int generated = 0;
            while (mState == RunState.Running)
            {
                for (int level = 1; level <= 100; level++)
                {
                    //### bob: slightly less accurate but faster way to generate
                    for (int i = 0; i < 5000; i++)
                    {
                        // let the level wander
                        int monsterLevel = Math2.Clamp(1, Rng.WalkLevel(level), 100);

                        // pick a race
                        Race race = mContent.Races.RandomSoft(monsterLevel);

                        int count = mRaces[race].Increment(level);
                        MaxRaceCount = Math.Max(MaxRaceCount, count);
                        MaxRaceCounts[level - 1] = Math.Max(MaxRaceCounts[level - 1], count);

                        if (race.Drop != null)
                        {
                            foreach (var item in race.Drop.Create(race.Depth))
                            {
                                int itemCount = mItems[item.Type].Increment(level);
                                MaxItemCount = Math.Max(MaxItemCount, itemCount);
                                MaxItemCounts[level - 1] = Math.Max(MaxItemCounts[level - 1], itemCount);

                                if (item.Power != null)
                                {
                                    int powerCount = mPowers[item.Power.Type].Increment(level);
                                    MaxPowerCount = Math.Max(MaxPowerCount, powerCount);
                                    MaxPowerCounts[level - 1] = Math.Max(MaxPowerCounts[level - 1], powerCount);
                                }
                            }
                        }
                    }

                    // bail if stopped
                    if (mState != RunState.Running) break;

                    if (level % 10 == 0)
                    {
                        if (Updated != null) Updated(this, EventArgs.Empty);
                    }

                    //### bob: slower, more accurate way
                    /*
                    // generate actual dungeons
                    Game game = new Game(Hero.CreateTemp(), mContent);
                    game.SetFloor(level);

                    Dictionary<Race, int> groupCounts = new Dictionary<Race, int>();

                    int numMonsters = 0;
                    int totalHealth = 0;
                    foreach (Monster monster in game.Dungeon.Entities.OfType<Monster>())
                    {
                        if (monster.Race.NumberInGroup > 1)
                        {
                            if (!groupCounts.ContainsKey(monster.Race))
                            {
                                groupCounts[monster.Race] = monster.Race.NumberInGroup;
                            }
                            else
                            {
                                groupCounts[monster.Race]--;
                                if (groupCounts[monster.Race] <= 0)
                                {
                                    groupCounts.Remove(monster.Race);
                                }
                                else
                                {
                                    // skip the monster to only count the group as one monster on average
                                    continue;
                                }
                            }
                        }

                        int count = mRaces[monster.Race].Increment(level);
                        MaxRaceCount = Math.Max(MaxRaceCount, count);
                        MaxRaceCounts[level - 1] = Math.Max(MaxRaceCounts[level - 1], count);

                        numMonsters++;
                        totalHealth += monster.Health.Current;
                    }

                    //### bob: monster health is assumed to increase logarithmically
                    mMonsterHealth.Add(level, (int)(100.0f * Math.Log10(totalHealth) / numMonsters));

                    foreach (Item item in game.Dungeon.Items)
                    {
                        int count = mItems[item.Type].Increment(level);
                        MaxItemCount = Math.Max(MaxItemCount, count);
                        MaxItemCounts[level - 1] = Math.Max(MaxItemCounts[level - 1], count);

                        if (item.Power != null)
                        {
                            int powerCount = mPowers[item.Power.Type].Increment(level);
                            MaxPowerCount = Math.Max(MaxPowerCount, powerCount);
                            MaxPowerCounts[level - 1] = Math.Max(MaxPowerCounts[level - 1], powerCount);
                        }
                    }

                    // bail if stopped
                    if (mState != RunState.Running) break;

                    if (level % 10 == 0)
                    {
                        if (Updated != null) Updated(this, EventArgs.Empty);
                    }
                    */
                }

                generated++;
                Console.WriteLine("generated " + generated);
            }
        }

        private enum RunState
        {
            NotStarted,
            Running,
            Stopped
        }

        private Content mContent;

        private Dictionary<ItemType, ItemStat> mItems = new Dictionary<ItemType, ItemStat>();
        private Dictionary<Race, RaceStat> mRaces = new Dictionary<Race, RaceStat>();
        private Dictionary<PowerType, PowerStat> mPowers = new Dictionary<PowerType, PowerStat>();
        private AverageStat mMonsterHealth;

        private RunState mState = RunState.NotStarted;
    }

    public abstract class StatBase : IStatRow
    {
        public bool IsTall { get { return false; } }

        public StatBase(AsyncStats stats)
        {
            mStats = stats;
            mCounts = new int[100];
        }

        public int Increment(int level)
        {
            return ++mCounts[level - 1];
        }

        #region IStatRow Members

        public abstract string Name { get; }

        public abstract Color Color { get; }

        public abstract int Max(int x);

        public IEnumerable<int> Values
        {
            get
            {
                return mCounts;
            }
        }

        #endregion

        protected AsyncStats mStats;
        private int[] mCounts;
    }

    public class RaceStat : StatBase
    {
        public RaceStat(AsyncStats stats, Race race)
            : base(stats)
        {
            mRace = race;
        }

        public override string Name { get { return mRace.Name; } }

        public override int Max(int x) { return mStats.MaxPerLevel ? mStats.MaxRaceCounts[x] : mStats.MaxRaceCount; }

        public override Color Color
        {
            get
            {
                Character character = (Character)mRace.Appearance;
                return character.ForeColor;
            }
        }

        private Race mRace;
    }

    public class ItemStat : StatBase
    {
        public ItemStat(AsyncStats stats, ItemType type)
            : base(stats)
        {
            mType = type;
        }

        public override string Name { get { return mType.Name; } }

        public override int Max(int x) { return mStats.MaxPerLevel ? mStats.MaxItemCounts[x] : mStats.MaxItemCount; }

        public override Color Color
        {
            get
            {
                Character character = (Character)mType.Appearance;
                return character.ForeColor;
            }
        }

        private ItemType mType;
    }

    public class PowerStat : StatBase
    {
        public PowerStat(AsyncStats stats, PowerType type)
            : base(stats)
        {
            mType = type;
        }

        public override string Name { get { return mType.Name; } }

        public override int Max(int x) { return mStats.MaxPerLevel ? mStats.MaxPowerCounts[x] : mStats.MaxPowerCount; }

        public override Color Color
        {
            get
            {
                if (mType.Appearance != null)
                {
                    return (Color)mType.Appearance;
                }

                return Color.White;
            }
        }

        private PowerType mType;
    }

    public class AverageStat : IStatRow
    {
        public bool IsTall { get { return true; } }

        public IEnumerable<int> Values { get { return mValues; } }

        public AverageStat(string name)
        {
            mName = name;
            mValues = new int[100];
        }

        public string Name { get { return mName; } }

        public int Max(int x) { return mValues.Max(); }

        public Color Color { get { return Color.White; } }

        public void Add(int level, int amount)
        {
            mValues[level - 1] += amount;
        }

        private int[] mValues;
        private string mName;
    }
}
