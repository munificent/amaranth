using System;
using System.Collections.Generic;
using System.Text;

using Bramble.Core;

using Amaranth.Util;

namespace Amaranth.Engine
{
    [Serializable]
    public class Hero : Entity
    {
        /// <summary>
        /// Creates a new temporary Hero. Used for things like the reporting apps.
        /// </summary>
        public static Hero CreateTemp() { return new Hero("temp", "temp", new Stats(), false); }

        public static Hero CreateNew(Content content, string name, string race,
            Stats stats, bool cheatDeath)
        {
            Hero hero = new Hero(name, race, stats, cheatDeath);

            // start with set equipment
            //### bob: hard-coding item types is lame
            hero.Equipment.Equip(new Item(Vec.Zero, content.Items.Find("knife")));
            hero.Equipment.Equip(new Item(Vec.Zero, content.Items.Find("cloth shirt")));
            hero.Inventory.Add(new Item(Vec.Zero, content.Items.Find("scroll of sidestepping")));
            hero.Inventory.Add(new Item(Vec.Zero, content.Items.Find("salve of mending")));
            hero.Inventory.Add(new Item(Vec.Zero, content.Items.Find("scroll of light"), 5));
            hero.Inventory.Add(new Item(Vec.Zero, content.Items.Find("candle")));
            hero.Inventory.Add(new Item(Vec.Zero, content.Items.Find("candle")));
            hero.Inventory.Add(new Item(Vec.Zero, content.Items.Find("candle")));

            return hero;
        }

        public static Hero CreateCanonicalLevel10(Content content, string name, string race,
            Stats stats, bool cheatDeath)
        {
            Hero hero = new Hero(name, race, stats, cheatDeath);

            // level up
            hero.GainExperience(null, hero.GetExperience(10));

            // start with set equipment
            hero.Equipment.Equip(new Item(Vec.Zero, content.Items.Find("mace")));
            hero.Equipment.Equip(new Item(Vec.Zero, content.Items.Find("mail hauberk")));
            hero.Equipment.Equip(new Item(Vec.Zero, content.Items.Find("pair of leather boots")));
            hero.Equipment.Equip(new Item(Vec.Zero, content.Items.Find("cloak")));
            hero.Equipment.Equip(new Item(Vec.Zero, content.Items.Find("leather cap")));

            hero.Inventory.Add(new Item(Vec.Zero, content.Items.Find("scroll of sidestepping"), 5));
            hero.Inventory.Add(new Item(Vec.Zero, content.Items.Find("salve of mending"), 5));
            hero.Inventory.Add(new Item(Vec.Zero, content.Items.Find("balm of soothing"), 2));
            hero.Inventory.Add(new Item(Vec.Zero, content.Items.Find("scroll of light"), 10));
            hero.Inventory.Add(new Item(Vec.Zero, content.Items.Find("candle")));
            hero.Inventory.Add(new Item(Vec.Zero, content.Items.Find("candle")));
            hero.Inventory.Add(new Item(Vec.Zero, content.Items.Find("candle")));
            hero.Inventory.Add(new Item(Vec.Zero, content.Items.Find("rune of surfacing"), 3));

            hero.mDeepestDepth = 15;

            return hero;
        }

        #region INoun Members

        public override string NounText { get { return "you"; } }
        public override Person Person { get { return Person.Second; } }
        public override string Pronoun { get { return "you"; } }
        public override string Possessive { get { return "your"; } }

        #endregion

        #region INamed Members

        /// <summary>
        /// Gets the Hero's name.
        /// </summary>
        public override string Name { get { return mName; } }

        #endregion

        /// <summary>
        /// Gets the number of turns the Hero has been alive for.
        /// </summary>
        public long Turns { get { return mTurns; } }

        public string Race { get { return mRace; } }

        public HeroClass Class { get { return mClass; } }

        public int Level { get { return mLevel; } }

        /// <summary>
        /// Gets the depth of the deepest dungeon level the Hero has reached.
        /// </summary>
        public int DeepestDepth { get { return mDeepestDepth; } }

        /// <summary>
        /// Gets the Hero's experience, in cents.
        /// </summary>
        public FluidStat Experience { get { return mExperience; } }

        /// <summary>
        /// Gets the amount of experience required to reach the next level.
        /// </summary>
        public int NextExperience { get { return GetExperience(mLevel + 1); } }

        /// <summary>
        /// Gets the hero's <see cref="Stats"/>.
        /// </summary>
        public Stats Stats { get { return mStats; } }

        public Inventory Inventory { get { return mInventory; } }
        public Equipment Equipment { get { return mEquipment; } }

        public override int LightRadius
        {
            get
            {
                // start at zero so the hero is always visible
                int radius = 0;

                foreach (Item item in Equipment)
                {
                    if ((item != null) && (item.LightRadius > 0))
                    {
                        radius += item.LightRadius;
                    }
                }

                return radius;
            }
        }

        /// <summary>
        /// Gets and sets the amount of "money" the Hero has. Note that currency is in its own
        /// unnamed denomination, distinct from the different coins in the game which
        /// are simply items that don't do anything, but with cash value.
        /// </summary>
        public int Currency
        {
            get { return mCurrency; }
            set
            {
                if (mCurrency != value)
                {
                    mCurrency = value;
                    Changed.Raise(this, EventArgs.Empty);
                }
            }
        }

        public float MeleeDamageBonus { get { return GetDamageBonus(true); } }
        public int MeleeStrikeBonus { get { return GetStrikeBonus(true); } }
        public float MissileDamageBonus { get { return GetDamageBonus(false); } }
        public int MissileStrikeBonus { get { return GetStrikeBonus(false); } }

        private Hero(string name, string race, Stats stats, bool cheatDeath)
            : base(Vec.Zero, Energy.NormalSpeed, 10)
        {
            mName = name;
            mRace = race;

            //### bob: temp. should be passed in
            mClass = new Warrior();

            mStats = stats;

            mCheatDeath = cheatDeath;

            mInventory = new Inventory();
            mEquipment = new Equipment();
            mEquipment.ItemEquipped.Add(Equipment_ItemEquipped);

            mLevel = 1;

            SetBehavior(new OneShotBehavior(null));

            mExperience.Changed += Experience_Changed;

            foreach (Stat stat in Stats)
            {
                stat.Changed += Stat_Changed;
            }

            // pick all of the random stat gains for levelling up. pick them now so that we
            // can unwind and rewind them as exp is drained and regained.
            mStatGains = new Stat[MaxLevel];
            for (int i = 0; i < mStatGains.Length; i++)
            {
                mStatGains[i] = Rng.Item(Stats);
            }

            RefreshMaxHealth();
            RefreshAutoHealTimer();
        }

        public void NoteDepth(int depth)
        {
            mDeepestDepth = Math.Max(mDeepestDepth, depth);
        }

        public void SetNextAction(NotNull<Action> action)
        {
            base.SetBehavior(new OneShotBehavior(action));
        }

        public new void SetBehavior(NotNull<Behavior> behavior)
        {
            base.SetBehavior(behavior);
        }

        /// <summary>
        /// Gets the amount of experience the <see cref="Hero"/> will gain for
        /// killing a <see cref="Monster"/> of the given <see cref="Race"/>.
        /// </summary>
        public float GetExperience(NotNull<Race> race)
        {
            return race.Value.Experience / mLevel;
        }

        /// <summary>
        /// Called when the <see cref="Hero"/> has killed the given <see cref="Monster"/>.
        /// </summary>
        /// <param name="action">The Action that slew the Monster.</param>
        /// <param name="monster">The slain Monster.</param>
        public void Killed(Action action, Monster monster)
        {
            GainExperience(action, GetExperience(monster.Race));

            //### bob: pretty lame
            // handle killing the end boss
            if (monster.Race.IsBoss)
            {
                action.Log(LogType.PermanentGood, "You have won the game. Congratulations, hero.");
            }

            // handle killing a unique
            if (monster.Race.IsUnique)
            {
                mSlainUniques.Add(monster.Race.Name);
            }

            // let the class know
            mClass.KilledMonster(action, monster);
        }

        public override Attack GetAttack(Entity defender)
        {
            // default to barehanded
            Roller damage = Roller.Dice(1, 3);
            Element element = Element.Anima;
            string verb = Verbs.Hit;
            EffectType effectType = EffectType.Hit;
            IFlagCollection flags = new FlagCollection();

            // find the equipped melee weapon
            Item weapon = Equipment.MeleeWeapon;
            if (weapon != null)
            {
                damage = weapon.Attack.Damage;
                element = weapon.Attack.Element;
                verb = weapon.Attack.Verb;
                flags = weapon.Flags;

                //### bob: temp. need to get from weapon data
                effectType = EffectType.Stab;
            }

            var attack = new Attack(damage, MeleeStrikeBonus, MeleeDamageBonus, element, verb, effectType, flags);

            // give the class a chance to modify it
            mClass.BeforeAttack(defender, attack);

            return attack;
        }

        public override int GetDodge()
        {
            int strike = DodgeBase;

            // agility modifier
            strike += Stats.Agility.StrikeBonus;

            return strike;
        }

        public override void AfterDamage(NotNull<Action> action, NotNull<Hit> hit)
        {
            // let the base handle elements
            base.AfterDamage(action, hit);

            //### bob: handle sustains and a chance to resist?

            // handle the flags
            TryDrain(action, hit.Value.Attack, Stats.Strength, "You feel weak.");
            TryDrain(action, hit.Value.Attack, Stats.Agility, "You feel clumsy.");
            TryDrain(action, hit.Value.Attack, Stats.Stamina, "You feel tired.");
            TryDrain(action, hit.Value.Attack, Stats.Will, "Your conviction falters.");
            TryDrain(action, hit.Value.Attack, Stats.Intellect, "You feel stupid.");
            TryDrain(action, hit.Value.Attack, Stats.Charisma, "You feel ugly.");

            // drain experience
            if (hit.Value.Attack.Flags.Has("drain:experience"))
            {
                // chance to resist
                if (Stats.Will.ResistDrain())
                {
                    action.Value.Log(LogType.Resist, this, "{possessive} will sustain[s] {pronoun}.");
                }
                else
                {
                    // lower by 1% - 5%
                    float percent = Rng.Float(0.01f, 0.05f);

                    action.Value.Log(LogType.BadState, this, "{subject} feel[s] {possessive} life draining away.");
                    LoseExperience(action.Value, Experience.Current * percent);
                }
            }
        }

        public void GainExperience(Action action, float experience)
        {
            int experienceCents = (int)(experience * 100);

            if (!mExperience.IsMax)
            {
                // experience is drained, so only raise the max a little
                mExperience.Base += experienceCents / 5;
            }
            else
            {
                // not drained, so raise the full amount
                mExperience.Base += experienceCents;
            }

            mExperience.Current += experienceCents;

            RefreshLevel(action);
        }

        public void RestoreExperience(Action action)
        {
            if (!mExperience.IsMax)
            {
                mExperience.Current = mExperience.Base;

                action.Log(LogType.TemporaryGood, this, "{subject} feel[s] {possessive} life return to {pronoun}.");

                RefreshLevel(action);
            }
        }

        public void LoseExperience(Action action, float experienceCents)
        {
            mExperience.Current -= (int)experienceCents;

            RefreshLevel(action);
        }

        public int GetNumResists(Element element)
        {
            int resists = 0;
            foreach (Item item in mEquipment)
            {
                if ((item != null) && item.Resists(element))
                {
                    // add to the number of resists
                    resists++;
                }
            }

            //### bob: need to do magical resists too

            return resists;
        }

        /// <summary>
        /// Gets whether the Hero has already slain this unique Race. Always returns
        /// <c>false</c> if the Race is not unique.
        /// </summary>
        /// <param name="race">The Race.</param>
        /// <returns></returns>
        public bool HasSlain(Race race)
        {
            // can't slay and entire race of monsters!
            if (!race.IsUnique) return false;

            return mSlainUniques.Contains(race.Name);
        }

        protected override void OnSetPosition(Vec pos)
        {
            base.OnSetPosition(pos);

            Dungeon.DirtyVisibility();
        }

        protected override float OnGetResistance(Element element)
        {
            // resistance = 1 / (1 + number of resists) so that increasing resists
            // decrease the damage but by less and less. i.e. 1 resist is half
            // damage, 2 resists is one third, etc.
            float resist = 1.0f / (1.0f + GetNumResists(element));

            return resist;
        }

        protected override bool OnStandsFirm(Hit hit)
        {
            // can either dodge it or just tough it out
            int power = Stats.Strength + Stats.Agility;

            if (Rng.Int(Stat.BaseMax * 2) < power)
            {
                // resisted
                return true;
            }

            return false;
        }

        protected override int OnGetArmor()
        {
            int armor = 0;
            foreach (Item item in mEquipment)
            {
                if (item != null)
                {
                    armor += item.TotalArmor;
                }
            }

            return armor;
        }

        protected override bool OnDie(Action action)
        {
            if (!mCheatDeath)
            {
                Dungeon.Game.Lose();
            }
            else
            {
                action.Log(LogType.Good, this, "{subject} cheat[s] death!");
                action.AddAction(new HealFullAction(this));
            }

            // hero can't actually be removed from game
            return false;
        }

        protected override void OnTakeTurn()
        {
            mTurns++;
        }

        private int GetStrikeBonus(bool melee)
        {
            // get the bonuses from equipment
            int strikeBonus = 0;
            foreach (Item item in Equipment.GetEquipped(melee, !melee))
            {
                    strikeBonus += item.StrikeBonus;
            }

            // stat bonuses
            strikeBonus += Stats.Agility.StrikeBonus;

            //### bob: need to include temporary magical bonuses

            return strikeBonus;
        }

        private float GetDamageBonus(bool melee)
        {
            // get the bonuses from equipment
            float damageBonus = 1.0f;
            foreach (Item item in Equipment.GetEquipped(melee, !melee))
            {
                damageBonus *= item.DamageBonus;
            }

            // stat bonuses
            damageBonus *= Stats.Strength.DamageBonus;

            //### bob: need to include temporary magical bonuses

            return damageBonus;
        }

        private void TryDrain(Action action, Attack attack, Stat stat, string message)
        {
            // see if the attack drains this stat
            if (attack.Flags.Has("drain:" + stat.Name.ToLower()))
            {
                // chance to resist
                if (Stats.Will.ResistDrain())
                {
                    action.Log(LogType.Resist, "Your will sustains you.");
                }
                else
                {
                    stat.AddBonus(BonusType.Drain, -1);
                    action.Log(LogType.BadState, message);
                }
            }
        }

        private void RefreshAutoHealTimer()
        {
            // get rid of the old timer
            if (mAutoHealTimer != null)
            {
                Energy.Timers.Remove(mAutoHealTimer);
            }

            // heals faster the greater the max health
            int stepsPerHeal = Math.Max(1, 400 / Health.Max);

            mAutoHealTimer = new EnergyTimer(AutoHeal, stepsPerHeal, true);
            Energy.Timers.Add(mAutoHealTimer);
        }

        private void AutoHeal()
        {
            Health.Current++;
        }

        private void RefreshMaxHealth()
        {
            int change = Stats.Stamina.MaxHealth - Health.Base;

            if (change != 0)
            {
                // set the new base
                Health.Base = Stats.Stamina.MaxHealth;

                // if the max went up, increase the current health too
                if (change > 0)
                {
                    Health.Current += change;
                }
            }

            RefreshAutoHealTimer();
        }

        private void RefreshLevel(Action action)
        {
            int level = GetLevel(mExperience.Current);

            // lose levels
            while (mLevel > level)
            {
                mLevel--;

                if (action != null) action.Log(LogType.BadState, "You have lost a level!");

                // lose a stat
                Stat stat = mStatGains[mLevel + 1]; // offset by one to get the level just lost
                stat.Base--;
                if (action != null) action.Log(LogType.BadState, "Your " + stat.Name + " decreased!");
            }

            // gain levels
            while (mLevel < level)
            {
                mLevel++;
                if (action != null) action.Log(LogType.PermanentGood, "You have gained a level!");

                // gain a stat
                Stat stat = mStatGains[mLevel];
                stat.Base++;
                if (action != null) action.Log(LogType.PermanentGood, "Your " + stat.Name + " increased!");
            }
        }

        /// <summary>
        /// Gets the level the <see cref="Hero"/> would be at with the given amount of
        /// experience (in cents).
        /// </summary>
        /// <param name="experienceCents">Amount of experience, in hundredths.</param>
        /// <returns>The level the Hero would be at with that experience.</returns>
        private int GetLevel(int experienceCents)
        {
            int level = 1;

            while (level <= MaxLevel)
            {
                int needed = GetExperience(level + 1);
                int neededCents = needed * 100;

                // stop if we don't have enough
                if (experienceCents < neededCents) break;

                level++;
            }

            return level;
        }

        /// <summary>
        /// Gets the amount of experience required to reach the given level.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private int GetExperience(int level)
        {
            // offset so that level 1 = 0 experience
            level -= 1;

            return (40 * level * level);
        }

        private void Equipment_ItemEquipped(Item item, EventArgs e)
        {
            // refresh the stat bonuses
            foreach (Stat stat in Stats)
            {
                int bonus = 0;

                foreach (Item thisItem in mEquipment)
                {
                    if (thisItem != null)
                    {
                        bonus += thisItem.GetStatBonus(stat);
                    }
                }

                stat.SetBonus(BonusType.Equipment, bonus);
            }
            
            // refresh the speed bonus
            int speedBonus = 0;
            foreach (Item thisItem in mEquipment)
            {
                if (thisItem != null)
                {
                    speedBonus += thisItem.SpeedBonus;
                }
            }

            Speed.SetBonus(BonusType.Equipment, speedBonus);

            // refresh the lighting (in case it was a light source)
            OnLightRadiusChanged();

            OnChanged();
        }

        private void Experience_Changed(object sender, EventArgs e)
        {
            OnChanged();
        }

        private void Stat_Changed(object sender, EventArgs e)
        {
            OnChanged();

            // in case stamina changed
            RefreshMaxHealth();
        }

        private const int MaxLevel = 50;

        private readonly string mName;
        private readonly string mRace;
        private readonly HeroClass mClass;

        private int mLevel = 1;
        private long mTurns = 0;
        private readonly FluidStat mExperience = new FluidStat(0);

        private EnergyTimer mAutoHealTimer;

        private readonly Stats mStats;

        private readonly Inventory mInventory;
        private readonly Equipment mEquipment;

        private int mCurrency;
        private int mDeepestDepth;
        private bool mCheatDeath;

        private Stat[] mStatGains;

        private readonly List<string> mSlainUniques = new List<string>();
    }
}
