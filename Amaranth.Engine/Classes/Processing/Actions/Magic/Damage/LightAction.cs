using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    public class LightAction : ElementBallAction
    {
        /// <summary>
        /// Initializes a new LightAction.
        /// </summary>
        /// <param name="entity">The <see cref="Entity"/> using the light.</param>
        /// <param name="attack">The <see cref="Attack"/> to perform on anything hit by the light.</param>
        /// <param name="noun">The <see cref="Noun"/> describing the light.</param>
        /// <param name="radius">Maximum light radius.</param>
        public LightAction(Entity entity, INoun noun, int radius, Attack attack)
            : base(entity, entity.Position, radius, noun, attack)
        {
            if (!(entity is Hero)) throw new ArgumentException("Lame. Right now LightAction uses the Hero's cached visibility data, so can't be used by other entities.");
        }

        protected override void OnEffect(Vec pos, Direction direction, bool leadingEdge)
        {
            // do the element attack, but only on the leading edge so we don't see the trails
            if (leadingEdge)
            {
                base.OnEffect(pos, direction, leadingEdge);
            }
        }

        protected override bool IsTileAllowed(Vec pos)
        {
            //### bob: hack. assumes the hero is the one performing the action
            // only light up visible tiles so that walls block the light
            return Dungeon.Tiles[pos].IsVisible;
        }
    }
}
