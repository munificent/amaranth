using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bramble.Core;

using Amaranth.Util;

namespace Amaranth.Engine
{
    public static class Lighting
    {
        /// <summary>
        /// Updates the lighting for light provided by Things.
        /// </summary>
        public static void Refresh(Vec position, Game game)
        {
            // figure out which ones need to be looked at
            Rect bounds = new Rect(position - Fov.MaxDistance, new Vec(Fov.MaxDistance * 2 + 1, Fov.MaxDistance * 2 + 1));

            // pad out to the max light radius so that things out of bounds will
            // updated if their light bleeds in
            bounds = bounds.Inflate(MaxLightRadius);

            // stay in bounds
            bounds = bounds.Intersect(game.Dungeon.Bounds);

            // cache for speed
            var topLeft = bounds.TopLeft;

            // figure out which tiles are lit
            Array2D<bool> lighting = new Array2D<bool>(bounds.Size);

            //### bob: should take into account occlusion. right now, light will penetrate walls

            IEnumerable<Thing> things = Enumerable.Concat(game.Dungeon.Items.Cast<Thing>(), game.Dungeon.Entities.Cast<Thing>());

            // go through everything in the dungeon
            foreach (Thing thing in things)
            {
                if (thing.LightRadius > -1)
                {
                    Circle circle = new Circle(thing.Position, thing.LightRadius);

                    foreach (Vec pos in circle)
                    {
                        Vec localPos = pos - topLeft;

                        if (lighting.Bounds.Contains(localPos))
                        {
                            lighting[localPos] = true;
                        }
                    }
                }
            }

            // update the lighting
            foreach (Vec pos in lighting.Bounds)
            {
                game.Dungeon.SetTileThingLit(pos + topLeft, lighting[pos]);
            }
        }

        private const int MaxLightRadius = 5;
    }
}
