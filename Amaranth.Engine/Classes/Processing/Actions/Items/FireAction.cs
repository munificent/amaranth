using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bramble.Core;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// Action for firing a missile using a missile weapon.
    /// </summary>
    public class FireAction : Action
    {
        public FireAction(Hero hero, Vec target)
            : base(hero)
        {
            mTarget = target;
        }

        public FireAction(Hero hero, Direction direction)
            : this(hero, hero.Position + direction)
        {
        }

        protected override ActionResult OnProcess()
        {
            Hero hero = (Hero)Entity;

            //### bob: hardcoded categories = hack
            Item weapon = hero.Equipment["Missile Weapon"];
            Item ammunition = hero.Equipment["Ammunition"];

            // need weapon and ammo
            if (weapon == null) return Fail("{subject} do[es] not have a missile weapon equipped.");
            if (ammunition == null) return Fail("{subject} do[es] not have any ammunition equipped.");

            // and they must match
            if (weapon.Type.Ammunition != ammunition.Type.Ammunition) return Fail(weapon, "{subject} fires " + weapon.Type.Ammunition + "s, not " + ammunition.Type.Ammunition + "s.");

            // use up the ammo
            Item fired = ammunition.SplitStack(1);

            // remove the stack if empty
            if (ammunition.Quantity == 0)
            {
                hero.Equipment.Remove(ammunition);
                Log(LogType.Message, "{subject} [are|is] out of ammunition.");
            }

            MergedFlagCollection flags = new MergedFlagCollection(weapon.Flags, fired.Flags);

            Attack attack = new Attack(fired.Attack.Damage,
                hero.MissileStrikeBonus, hero.MissileDamageBonus,
                fired.Attack.Element, fired.Attack.Verb, fired.Attack.EffectType, flags);

            AddAction(new ElementBoltAction(Entity, mTarget, fired, attack));

            return ActionResult.Done;
        }

        private Vec mTarget;
    }
}
