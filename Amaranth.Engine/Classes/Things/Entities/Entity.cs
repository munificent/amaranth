using System;
using System.Collections.Generic;
using System.Text;

using Bramble.Core;

using Amaranth.Util;

namespace Amaranth.Engine
{
    [Serializable]
    public abstract class Entity : Thing, ICollectible<EntityCollection, Entity>, ISpeed
    {
        /// <summary>
        /// The default base chance of an Entity to dodge melee attack (out of 100).
        /// </summary>
        public const int DodgeBase = 20;

        /// <summary>
        /// Given a total armor value, gets the fraction of damage that will penetrate the armor.
        /// </summary>
        /// <param name="armor">The armor value.</param>
        /// <remarks>
        /// Armor reduces damage by an inverse curve such that increasing
        /// armor has less and less effect. Damage is reduced to the following:
        /// 
        /// armor damage
        /// ------------
        /// 0     100%
        /// 50    50%
        /// 100   33%
        /// 150   25%
        /// 200   20%
        /// ...   etc.
        /// </remarks>
        public static float GetArmorReduction(int armor)
        {
            // damage is never increased
            armor = Math.Max(0, armor);

            return 1.0f / (1.0f + armor / 50.0f);
        }

        public readonly GameEvent<Entity, ValueChangeEventArgs<Vec>> Moved = new GameEvent<Entity, ValueChangeEventArgs<Vec>>();
        public readonly GameEvent<Entity, EventArgs> Changed = new GameEvent<Entity, EventArgs>();

        public Energy Energy { get { return mEnergy; } }

        public Behavior Behavior { get { return mBehavior; } }

        public Speed Speed { get { return mSpeed; } }

        public FluidStat Health { get { return mHealth; } }

        public int Armor { get { return OnGetArmor(); } }

        public EntityConditions Conditions { get { return mConditions; } }

        public bool IsAlive { get { return mHealth.Current > 0; } }

        public virtual bool OpensDoors { get { return true; } }

        /// <summary>
        /// Gets the <see cref="Dungeon"/> that contains this <see cref="Entity"/>.
        /// </summary>
        public override Dungeon Dungeon { get { return mCollection.Dungeon; } }

        public Entity(Vec pos, int speed, int health)
            : base(pos)
        {
            mEnergy = new Energy(this);

            mSpeed = new Speed(speed);
            mSpeed.Changed += new EventHandler(Speed_Changed);

            mConditions = new EntityConditions(this);
            mConditions.ConditionChanged += Conditions_ConditionChanged;

            mHealth = new FluidStat(health);
            mHealth.Changed += Health_Changed;
            mHealth.BonusChanged += Health_BonusChanged;
        }

        public virtual IEnumerable<Action> GetPassiveActions()
        {
            // do nothing
            return null;
        }

        public IEnumerable<Action> TakeTurn()
        {
            // get the next action from the ai
            Action turnAction = mBehavior.NextAction();
            turnAction.MarkAsEnergyTaking();

            OnTakeTurn();

            yield return turnAction;

            // process the timed conditions
            foreach (Action conditionAction in mConditions.Update())
            {
                yield return conditionAction;
            }
        }

        public override void Hit(Action action, Hit hit)
        {
            bool madeContact = true;

            // give the entity a chance to dodge
            if (hit.CanDodge)
            {
                // ask the defender how hard it is to hit
                int strike = GetDodge();

                // modify it by how good this entity is at hitting
                // subtract so that a positive bonus is good for the attacker
                strike -= hit.Attack.StrikeBonus;

                // keep it in bounds
                strike = strike.Clamp(5, 95);

                int strikeRoll = Rng.IntInclusive(1, 100);

                madeContact = (strikeRoll >= strike);
            }

            // see if we hit
            if (madeContact)
            {
                // damage the defender
                hit.SetDamage(ReceiveDamage(hit.Attack));

                // if damage was actually done
                if (hit.Damage > 0)
                {
                    // only need to add the effect here if it's conditional on dodging. otherwise it's already been addded
                    if (hit.CanDodge)
                    {
                        action.AddEffect(hit.CreateEffect(Position));
                    }

                    Hero actingHero = Dungeon.Game.ActingEntity as Hero;
                    bool attackerIsHero = (actingHero != null);

                    if (Health.Current <= 0)
                    {
                        action.Log(LogType.Good, hit.Attacker, "{subject} kill[s] {object}.", this);

                        // if a monster was killed by the hero, tell the hero so he can gain experience
                        Monster killed = this as Monster;
                        if ((killed != null) && (actingHero != null))
                        {
                            actingHero.Killed(action, killed);
                        }

                        Die(action);
                    }
                    else
                    {
                        action.Log(LogType.Good, hit.Attacker, "{subject} " + hit.Attack.Verb + " {object} for " + hit.Damage + " damage.", this);
                        
                        // tell the user about the resists
                        float resist = OnGetResistance(hit.Attack.Element);
                        string element = hit.Attack.Element.ToString().ToLower();

                        if (resist < 1.0f)
                        {
                            action.Log(LogType.Resist, this, "{subject} resist[s] " + element + ".");
                        }
                        else if (resist > 1.0f)
                        {
                            action.Log(LogType.Resist, this, "{subject} [are|is] weak against " + element + ".");
                        }

                        // tell the user about the slays
                        float slays = OnGetSlays(hit.Attack.Flags);
                        if (slays < 1.0f)
                        {
                            action.Log(LogType.Resist, "The attack is ineffective on {object}.", this);
                        }
                        else if (slays > 4.0f)
                        {
                            action.Log(LogType.Resist, "The attack is deadly against {object}.", this);
                        }
                        else if (slays > 2.0f)
                        {
                            action.Log(LogType.Resist, "The attack is especially effective against {object}.", this);
                        }
                        else if (slays > 1.0f)
                        {
                            action.Log(LogType.Resist, "The attack is effective against {object}.", this);
                        }

                        AfterDamage(action, hit);
                    }
                }
                else
                {
                    // tell why no damage was done
                    float resist = OnGetResistance(hit.Attack.Element);
                    string element = hit.Attack.Element.ToString().ToLower();
                    float slays = OnGetSlays(hit.Attack.Flags);

                    if (resist == 0)
                    {
                        action.Log(LogType.Message, this, "{subject} [are|is] immune to " + element + ".");
                    }
                    else if (slays == 0)
                    {
                        action.Log(LogType.Message, this, "{subject} [are|is] impervious to the attack.");
                    }
                    else
                    {
                        // armor absorbed all of the damage
                        action.Log(LogType.Message, this, "{subject} [are|is] unhurt by {object}.", hit.Attacker);
                    }
                }
            }
            else
            {
                // miss
                action.Log(LogType.Message, hit.Attacker, "{subject} miss[es] {object}.", this);
            }
        }

        public abstract Attack GetAttack(Entity defender);

        public virtual int GetDodge()
        {
            //### bob: monster should override
            return DodgeBase;
        }

        public void Die(Action action)
        {
            if (OnDie(action))
            {
                mCollection.Remove(this);
            }
        }

        public virtual int ReceiveDamage(Attack attack)
        {
            // get the amount
            float amount = attack.Roll();

            // apply the modifiers
            amount *= OnGetResistance(attack.Element);
            amount *= OnGetSlays(attack.Flags);
            amount *= GetArmorReduction(OnGetArmor());

            // round up so that 1 damage doesn't always get cancelled out by any armor
            int appliedDamage = (int)Math.Ceiling(amount);

            // apply the damage
            Health.Current -= appliedDamage;

            //### bob: put disturb in Health_Changed? if so, need to handle *not* disturbing on auto-heal
            Behavior.Disturb();

            return appliedDamage;
        }

        public virtual void AfterDamage(NotNull<Action> action, NotNull<Hit> hit)
        {
            // apply the elemental effect if not resisted
            if (OnGetResistance(hit.Value.Attack.Element) >= 1.0f)
            {
                switch (hit.Value.Attack.Element)
                {
                    case Element.Air: action.Value.AddAction(new TeleportAction(this, hit, 3)); break;
                    case Element.Earth:
                        // cuts?
                        break;
                    case Element.Fire:
                        // burn up items
                        break;
                    case Element.Water: action.Value.AddAction(new PushBackAction(this, hit)); break;
                    case Element.Metal: /* no side effect */ break;
                    case Element.Wood: /* no side effect */ break;
                    case Element.Acid:
                        // damage equipment
                        break;
                    case Element.Cold: action.Value.AddAction(new FreezeAction(this, hit.Value.Damage)); break;
                    case Element.Lightning:
                        // break glass
                        break;
                    case Element.Poison: action.Value.AddAction(new PoisonAction(this, hit.Value.Damage)); break;
                    case Element.Dark:
                        // blind
                        break;
                    case Element.Light:
                        // blind
                        break;
                    case Element.Anima: /* no side effect */ break;
                    case Element.Death: action.Value.AddAction(new DiseaseAction(this, hit.Value.Damage)); break;
                }
            }

            // other side effects
            if (hit.Value.Attack.Flags.Has("slow"))
            {
                action.Value.AddAction(new SlowAction(this, hit.Value.Damage));
            }
            else if (hit.Value.Attack.Flags.Has("disease"))
            {
                action.Value.AddAction(new DiseaseAction(this, hit.Value.Damage));
            }
            else if (hit.Value.Attack.Flags.Has("paralyze"))
            {
                action.Value.Log(LogType.Special, "Should be paralyzing, but not implemented. :(");
            }
        }

        /// <summary>
        /// Gets whether or not the Entity resists an attempt to physically move it against its will.
        /// </summary>
        /// <param name="damage">The hit that's moving the Entity.</param>
        /// <returns><c>true</c> if the Entity resists and should not be moved.</returns>
        public bool StandsFirm(Hit hit)
        {
            // if the element of the attack is resisted, always stand firm
            if (OnGetResistance(hit.Attack.Element) < 1.0f) return true;

            // give the derived one a chance to resist too
            if (OnStandsFirm(hit)) return true;

            // by default, the odds of resisting are half the fraction of the max health that
            // the damage is doing. so, if the damage is taking half the entity's health,
            // the odds of resisting are 1 in 4.
            return Rng.Int(Health.Max) < (hit.Damage / 2);
        }

        protected virtual void OnTakeTurn() { }

        protected override void OnSetPosition(Vec pos)
        {
            Vec oldPos = Position;

            base.OnSetPosition(pos);

            if (GivesOffLight)
            {
                Dungeon.DirtyLighting();
            }

            OnMoved(new ValueChangeEventArgs<Vec>(oldPos, pos));
        }

        protected void OnMoved(ValueChangeEventArgs<Vec> args)
        {
            Moved.Raise(this, args);

            if (mCollection != null)
            {
                mCollection.EntityMoved.Raise(this, args);
            }
        }

        protected void SetBehavior(NotNull<Behavior> behavior)
        {
            mBehavior = behavior;
        }

        protected abstract float OnGetResistance(Element element);

        protected virtual float OnGetSlays(IFlagCollection flags)
        {
            // no slays by default
            return 1.0f;
        }

        protected virtual bool OnStandsFirm(Hit hit)
        {
            // default to failing
            return false;
        }

        protected virtual int OnGetArmor()
        {
            // default to no armor
            return 0;
        }

        protected abstract bool OnDie(Action action);

        protected void OnChanged()
        {
            Changed.Raise(this, EventArgs.Empty);
        }

        #region Event handlers

        private void Health_Changed(object sender, EventArgs e)
        {
            OnChanged();
        }

        private void Health_BonusChanged(object sender, EventArgs e)
        {
            Behavior.Disturb();

            OnChanged();
        }

        private void Conditions_ConditionChanged(object sender, EventArgs e)
        {
            Behavior.Disturb();

            OnChanged();
        }

        private void Speed_Changed(object sender, EventArgs e)
        {
            Behavior.Disturb();

            OnChanged();
        }

        #endregion

        #region ICollectible<EntityCollection,Entity> Members

        void ICollectible<EntityCollection, Entity>.SetCollection(EntityCollection collection)
        {
            mCollection = collection;
        }

        #endregion

        #region ISpeed Members

        int ISpeed.Speed
        {
            get { return Speed.Current; }
        }

        #endregion

        private EntityCollection mCollection;
        private readonly Energy mEnergy;
        private Speed mSpeed;
        private readonly FluidStat mHealth;
        private Behavior mBehavior;
        private readonly EntityConditions mConditions;
    }
}
