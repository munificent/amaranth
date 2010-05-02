using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    /// <summary>
    /// Base class for a hero's class. Each class gives the hero a different set of
    /// capabilities beyond the basic "run around and kill stuff". Unlike hero races,
    /// which are essentially data differences, classes actually change the game
    /// mechanics. Each class, by nature, requires special code for that class.
    /// </summary>
    [Serializable]
    public abstract class HeroClass
    {
        /// <summary>
        /// This is called when the Hero has slain a Monster.
        /// </summary>
        /// <param name="action">The Action in which the Monster was killed.</param>
        /// <param name="monster">The slain Monster.</param>
        public virtual void KilledMonster(Action action, Monster monster) { }

        /// <summary>
        /// This is called when the Hero is about to attempt to hit an Entity.
        /// </summary>
        /// <param name="defender">The entity being attacked.</param>
        /// <param name="attack">The attack.</param>
        public virtual void BeforeAttack(Entity defender, Attack attack) { }
    }
}
