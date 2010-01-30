using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    public enum Element
    {
        Air,
        Earth,
        Fire,
        Water,

        Metal,
        Wood,

        Acid,
        Cold,
        Lightning,
        Poison,

        Dark,
        Light,

        /// <summary>
        /// The spirit of life that imbues living things and the
        /// non-corporeal undead. Ghosts are this and nothing else.
        /// </summary>
        Anima,

        /// <summary>
        /// The unliving mechanism that animates the corporeal undead.
        /// </summary>
        Death
    }

    public static class ElementExtensions
    {
        /// <summary>
        /// Gets the multiplier for experience awarded by killing a <see cref="Monster"/>
        /// that attacks with this <see cref="Element"/>.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static float AttackExperience(this Element element)
        {
            float multiplier = 1.0f;

            switch (element)
            {
                case Element.Air:       multiplier = 1.1f; break;
                case Element.Earth:     multiplier = 1.0f; break;
                case Element.Fire:      multiplier = 1.2f; break;
                case Element.Water:     multiplier = 1.1f; break;
                case Element.Metal:     multiplier = 1.0f; break;
                case Element.Wood:      multiplier = 1.0f; break;
                case Element.Acid:      multiplier = 1.2f; break;
                case Element.Cold:      multiplier = 1.2f; break;
                case Element.Lightning: multiplier = 1.0f; break;
                case Element.Poison:    multiplier = 1.5f; break;
                case Element.Dark:      multiplier = 1.1f; break;
                case Element.Light:     multiplier = 1.1f; break;
                case Element.Anima:     multiplier = 1.0f; break;
                case Element.Death:     multiplier = 1.3f; break;
            }

            return multiplier;
        }
    }
}
