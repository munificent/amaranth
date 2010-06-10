using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bramble.Core;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// Generic <see cref="Action"/> for a radiating arc.
    /// </summary>
    public abstract class ConeAction : EnumerableAction
    {
        /// <summary>
        /// Creates a new ConeAction starting at the <see cref="Entity"/> and oriented towards the given target.
        /// </summary>
        public ConeAction(NotNull<Entity> entity, Vec target, int radius)
            : base(entity)
        {
            if (entity.Value.Position == target) throw new ArgumentException("The Entity and target must not have the same position.");

            mCenter = entity.Value.Position;
            mRadius = radius;
            mTarget = target;
        }

        protected override IEnumerable<ActionResult> OnProcessEnumerable()
        {
            // assuming a 45 degree arc, figure out how many rays are needed to stay
            // within one tile apart at the max radius
            double circumference = Math.PI * (2 * mRadius);
            int numRays = (int)Math.Ceiling(circumference / 8);

            // figure out the center angle of the cone
            Vec offset = mTarget - mCenter;

            double centerTheta = Math.Atan2(offset.X, offset.Y);

            // spread the rays around the center
            double?[] thetas = new double?[numRays];
            for (int i = 0; i < thetas.Length; i++)
            {
                double range = ((double)i / (numRays - 1)) - 0.5;
                thetas[i] = centerTheta + range * (Math.PI / 4.0);
            }

            // skip the center tile
            List<Vec> hitTiles = new List<Vec>();
            hitTiles.Add(mCenter);

            for (int radius = 1; radius <= mRadius; radius++)
            {
                // show the previously hit tiles
                foreach (Vec pos in hitTiles)
                {
                    if (pos != mCenter)
                    {
                        Direction direction = Direction.Towards(pos - mCenter);
                        OnEffect(pos, direction, false);
                    }
                }

                // see which new tiles each ray hit now
                for (int i = 0; i < thetas.Length; i++)
                {
                    // skip over halted rays
                    if (!thetas[i].HasValue) continue;

                    double theta = thetas[i].Value;

                    double x = mCenter.X + (Math.Sin(theta) * radius);
                    double y = mCenter.Y + (Math.Cos(theta) * radius);

                    Vec pos = new Vec((int)Math.Round(x), (int)Math.Round(y));

                    // skip if out of bounds or blocked
                    if (Dungeon.Bounds.Contains(pos) && Dungeon.Tiles[pos].IsTransparent)
                    {
                        // don't hit the same tile twice
                        if (hitTiles.Contains(pos)) continue;
                        hitTiles.Add(pos);

                        // see which direction it stepped
                        Direction direction = Direction.Towards(pos - mCenter);

                        // let the derived class do something
                        OnEffect(pos, direction, true);
                    }
                    else
                    {
                        // stop the entire ray
                        thetas[i] = null;
                    }
                }

                yield return ActionResult.NotDone;
            }
        }

        protected abstract void OnEffect(Vec pos, Direction direction, bool leadingEdge);

        private Vec mCenter;
        private Vec mTarget;
        private int mRadius;
    }
}
