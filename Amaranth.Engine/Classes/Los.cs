using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bramble.Core;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// Line-of-sight object for tracing a straight line from a starting point through a
    /// destination point and determining which intermediate tiles are touched.
    /// </summary>
    public class Los : IEnumerable<Vec>
    {
        /// <summary>
        /// Creates a new Los starting at the given position and moving through the given target.
        /// </summary>
        public Los(Dungeon dungeon, Vec start, Vec target)
        {
            if (start == target) throw new ArgumentException("The start and target must not be the same.");

            mDungeon = dungeon;
            mStart = start;
            mTarget = target;
        }

        /// <summary>
        /// Gets whether this Los will hit the given <see cref="Entity"/>.
        /// </summary>
        public bool HitsEntity(Entity target, bool passThroughOtherEntities)
        {
            foreach (Vec pos in this)
            {
                Entity entity = mDungeon.Entities.GetAt(pos);

                // success if we found our man
                if (entity == target) return true;

                if (!passThroughOtherEntities)
                {
                    // bail if we hit anyone else
                    if (entity != null) return false;
                }
            }

            // if we got here, we missed completely
            return false;
        }

        /// <summary>
        /// Gets whether this Los will hit the given <see cref="Entity"/> before hitting any other.
        /// </summary>
        public bool HitsEntity(Entity target)
        {
            return HitsEntity(target, false);
        }

        #region IEnumerable<Vec> Members

        public IEnumerator<Vec> GetEnumerator()
        {
            Vec delta = mTarget - mStart;

            // figure which octant the line is in and increment appropriately
            Vec primaryIncrement = new Vec(Math.Sign(delta.X), 0);
            Vec secondaryIncrement = new Vec(0, Math.Sign(delta.Y));

            // discard the signs now that they are accounted for
            delta = delta.Each((coord) => Math.Abs(coord));

            // assume moving horizontally each step
            int primary = delta.X;
            int secondary = delta.Y;

            // swap the order if the y magnitude is greater
            if (delta.Y > delta.X)
            {
                Obj.Swap(ref primary, ref secondary);
                Obj.Swap(ref primaryIncrement, ref secondaryIncrement);
            }

            Vec pos = mStart;
            int error = 0;

            // step down the line until stopped
            while (true)
            {
                Vec lastPos = pos;

                // move it first, gets it off the entity in the first step
                pos += primaryIncrement;

                // see if we need to step in the secondary direction
                error += secondary;
                if (error * 2 >= primary)
                {
                    pos += secondaryIncrement;
                    error -= primary;
                }

                // stop if out of bounds
                if (!mDungeon.Bounds.Contains(pos)) break;

                // stop if we hit a wall
                if (!mDungeon.Tiles[pos].IsTransparent) break;

                // keep going
                yield return pos;
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        private Dungeon mDungeon;
        private Vec mStart;
        private Vec mTarget;
    }
}
