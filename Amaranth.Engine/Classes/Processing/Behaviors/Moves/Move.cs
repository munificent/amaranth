using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// A Move is something a <see cref="Monster"/> can do other than walk and melee attack.
    /// This includes, breath attacks, missiles, and spells, but excludes things a Monster can
    /// do that do not consume <see cref="Energy"/> (those are <see cref="Ability">Abilities</see>).
    /// </summary>
    public abstract class Move
    {
        public MoveInfo Info { get { return mInfo; } }

        public abstract string Description { get; }

        public void BindInfo(MoveInfo info)
        {
            Assign.OnlyOnce(ref mInfo, info, "info");
        }

        public bool ShouldAttempt()
        {
            return (Rng.Int(Info.Range) < Info.Chance);
        }

        /// <summary>
        /// Gets whether or not the <see cref="Monster"/> will try this Move right now. Override
        /// to selectively prevent the move. Otherwise defaults to always <c>true</c>
        /// </summary>
        /// <returns></returns>
        public virtual bool WillUseMove(Monster monster, Entity target)
        {
            return true;
        }

        /// <summary>
        /// Override to return the <see cref="Action"/> created by performing this Move. Will only
        /// be called if <see cref="CanMove()"/> returns <c>true</c>.
        /// </summary>
        /// <returns></returns>
        public abstract Action GetAction(Monster monster, Entity target);

        /// <summary>
        /// Override this to return the experience multiplier awarded for killing a <see cref="Monster"/>
        /// that uses this Move. Do not take into account chance.
        /// </summary>
        public abstract float GetExperience(Race race);

        private MoveInfo mInfo;
    }

    /// <summary>
    /// Derived base Move class for Moves that need to store additional persistent information for each
    /// Monster that can perform the Move. Moves are bound to a Monster's Race, not the Monster itself,
    /// so if a given Move needs to store information for each Monster, this needs to be used instead.
    /// </summary>
    /// <typeparam name="TInstanceInfo">The class of the object containing the per-monster instance move
    /// information.</typeparam>
    public abstract class Move<TInstanceInfo> : Move where TInstanceInfo : new()
    {
        public override bool WillUseMove(Monster monster, Entity target)
        {
            return WillUseMove(monster, target, monster.GetMoveInstance<TInstanceInfo>());
        }

        public override Action GetAction(Monster monster, Entity target)
        {
            return GetAction(monster, target, monster.GetMoveInstance<TInstanceInfo>());
        }

        protected abstract Action GetAction(Monster monster, Entity target, TInstanceInfo info);

        protected virtual bool WillUseMove(Monster monster, Entity target, TInstanceInfo info)
        {
            return true;
        }
    }

    public class MoveInfo
    {
        public int Chance;
        public int Range;
        public int Radius;
        public INoun Noun;
        public string Verb;
        public Roller Damage;
        public Element Element;
        public EffectType Effect;

        public MoveInfo(int chance, int range)
        {
            Chance = chance;
            Range = range;

            // default the info
            Radius = 1;
            Verb = Verbs.Hit;
            Element = Element.Anima;
            Effect = EffectType.Bolt;
        }

        public MoveInfo()
            : this(1, 1)
        {
        }

        public MoveInfo Clone()
        {
            MoveInfo info = new MoveInfo();
            info.Chance = Chance;
            info.Range = Range;
            info.Radius = Radius;
            info.Noun = Noun;
            info.Verb = Verb;
            info.Damage = Damage;
            info.Element = Element;
            info.Effect = Effect;

            return info;
        }
    }
}
