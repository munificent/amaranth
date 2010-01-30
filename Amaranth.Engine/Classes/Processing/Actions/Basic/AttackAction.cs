using System;
using System.Collections.Generic;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    public class AttackAction : Action
    {
        public AttackAction(NotNull<Entity> attacker, NotNull<Entity> defender)
            : base(attacker)
        {
            mDefender = defender;
        }

        protected override ActionResult OnProcess()
        {
            // when the hero hits something, note it
            if (Entity is Hero)
            {
                Game.ThingNoticed.Raise(mDefender, EventArgs.Empty);
            }

            // when something hits the hero, note it
            if (mDefender is Hero)
            {
                Game.ThingNoticed.Raise(Entity, EventArgs.Empty);
            }

            // get the attack
            Attack attack = Entity.GetAttack(mDefender);

            // send the hit to the defender
            Direction direction = Direction.Towards(mDefender.Position - Entity.Position);
            Hit hit = new Hit(Entity, attack, true, direction);
            mDefender.Hit(this, hit);

            return ActionResult.Done;
        }

        private Entity mDefender;
    }
}
