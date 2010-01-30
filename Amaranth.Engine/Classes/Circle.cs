using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// Utility class for handling simple rasterized circles of a relatively small radius.
    /// Used for lighting, ball spells, etc. Optimized to generate "nice" looking circles
    /// at small sizes.
    /// </summary>
    public class Circle : IEnumerable<Vec>
    {
        /// <summary>
        /// Gets the position of the center of this Circle.
        /// </summary>
        public Vec Center { get { return mCenter; } }

        /// <summary>
        /// Gets the radius of this Circle.
        /// </summary>
        public int Radius { get { return mRadius; } }

        public Circle(Vec center, int radius)
        {
            if (radius < 0) throw new ArgumentOutOfRangeException("radius", "The radius cannot be negative.");

            mCenter = center;
            mRadius = radius;
        }

        public Circle(int radius)
            : this(Vec.Zero, radius)
        {
        }

        /// <summary>
        /// Gets whether the given position is in the outermost edge of the circle.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <returns><c>true</c> if <c>pos</c> is in the within the outer edge of the
        /// circle, or is outside of it.</returns>
        public bool IsEdge(Vec pos)
        {
            bool leadingEdge = true;
            if (mRadius > 0)
            {
                leadingEdge = (pos - mCenter).LengthSquared > GetRadiusSquared(mRadius - 1);
            }

            return leadingEdge;
        }

        #region IEnumerable<Vec> Members

        public IEnumerator<Vec> GetEnumerator()
        {
            Rect bounds = new Rect(Vec.Zero - mRadius, Vec.One * (mRadius + mRadius + 1));

            foreach (Vec pos in bounds)
            {
                if (pos.LengthSquared <= GetRadiusSquared(mRadius))
                {
                    yield return pos + mCenter;
                }
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        private int GetRadiusSquared(int radius)
        {
            // if small enough, use the tuned radius to look best
            if (radius < RadiiSquared.Length) return RadiiSquared[radius];

            // otherwise just use a default
            return radius * radius;
        }

        private static readonly int[] RadiiSquared = new int[] { 0, 2, 5, 10, 18, 26, 38 };

        private Vec mCenter;
        private int mRadius;
    }
}
