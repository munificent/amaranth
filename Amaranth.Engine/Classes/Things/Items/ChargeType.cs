using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    /// <summary>
    /// Defines the different ways an <see cref="Item"/> can be used, and what its charges
    /// represent, if anything.
    /// </summary>
    public enum ChargeType
    {
        /// <summary>
        /// Item is not used. Examples: basic weapons and armor.
        /// </summary>
        None,

        /// <summary>
        /// Item can be used once and is destroyed. Examples: potions, scrolls.
        /// </summary>
        Single,

        /// <summary>
        /// Item is consumed like a light source, and can be lit and unlit. When lit, charges are
        /// positive and drain over time. When unlit, charges are negative and do not drain.
        /// Examples: torches.
        /// </summary>
        Light,

        /// <summary>
        /// Item has a fixed number of charges and can be used once for each charge, then it's dead.
        /// </summary>
        Multi
    }
}
