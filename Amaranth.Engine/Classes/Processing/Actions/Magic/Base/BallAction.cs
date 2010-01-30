using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// Generic <see cref="Action"/> for an exploding circle of effect, like a fireball.
    /// </summary>
    public abstract class BallAction : EnumerableAction
    {
        /// <summary>
        /// Creates a new BallAction with the given radius.
        /// </summary>
        /// <param name="radius">Radius of the ball, in tiles. Valid values are 0 - 6.</param>
        public BallAction(NotNull<Entity> entity, Vec center, int radius)
            : base(entity)
        {
            if (radius < 0) throw new ArgumentOutOfRangeException("radius", "The radius must not be negative.");

            mCenter = center;
            mRadius = radius;
        }

        /// <summary>
        /// Overridden from <see cref="EnumerableAction"/>.
        /// </summary>
        /// <returns>The sequence of ActionResults.</returns>
        protected override IEnumerable<ActionResult> OnProcessEnumerable()
        {
            for (int radius = 0; radius <= mRadius; radius++)
            {
                Circle circle = new Circle(mCenter, radius);

                foreach (Vec pos in circle)
                {
                    // only on open tiles
                    if (Dungeon.Bounds.Contains(pos) && IsTileAllowed(pos))
                    {
                        Direction direction = Direction.Towards(pos - mCenter);

                        OnEffect(pos, direction, circle.IsEdge(pos));
                    }
                }

                yield return ActionResult.NotDone;
            }
        }

        /// <summary>
        /// Override this to control which positions are valid targets for the ball. By default, allows
        /// any passable tile in the <see cref="Dungeon"/>.
        /// </summary>
        /// <param name="pos">The position to check.</param>
        /// <returns><c>true</c> is the position is valid to have the ball effect.</returns>
        protected virtual bool IsTileAllowed(Vec pos)
        {
            return Dungeon.Tiles[pos].IsTransparent;
        }

        /// <summary>
        /// Override this to implement the effect the ball has on a given position in the <see cref="Dungeon"/>.
        /// </summary>
        /// <param name="pos">The position affected by the ball.</param>
        /// <param name="direction">The direction of the effect.</param>
        /// <param name="leadingEdge"><c>true</c> if this position is on the outer edge of the affected circle;
        /// <c>false</c> if it is in the interior of the circle and has been covered by a previous iteration.</param>
        protected abstract void OnEffect(Vec pos, Direction direction, bool leadingEdge);

        private Vec mCenter;
        private int mRadius;
    }
}
