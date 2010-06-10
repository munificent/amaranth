using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bramble.Core;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// Represents one of the eight directions on a compass (or no direction).
    /// </summary>
    [Serializable]
    public struct Direction : IEquatable<Direction>
    {
        /// <summary>
        /// Gets the "none" direction.
        /// </summary>
        public static Direction None { get { return new Direction(Vec.Zero); } }

        public static Direction N { get { return new Direction(new Vec(0, -1)); } }
        public static Direction NE { get { return new Direction(new Vec(1, -1)); } }
        public static Direction E { get { return new Direction(new Vec(1, 0)); } }
        public static Direction SE { get { return new Direction(new Vec(1, 1)); } }
        public static Direction S { get { return new Direction(new Vec(0, 1)); } }
        public static Direction SW { get { return new Direction(new Vec(-1, 1)); } }
        public static Direction W { get { return new Direction(new Vec(-1, 0)); } }
        public static Direction NW { get { return new Direction(new Vec(-1, -1)); } }

        /// <summary>
        /// Adds the offset of the given Direction to the Vec.
        /// </summary>
        /// <param name="v1">Vector to add the Direction to.</param>
        /// <param name="d2">Direction to offset the vector.</param>
        /// <returns>A new Vec.</returns>
        public static Vec operator +(Vec v1, Direction d2)
        {
            return v1 + d2.Offset;
        }

        /// <summary>
        /// Adds the offset of the given Direction to the Vec.
        /// </summary>
        /// <param name="d1">Direction to offset the vector.</param>
        /// <param name="v2">Vector to add the Direction to.</param>
        /// <returns>A new Vec.</returns>
        public static Vec operator +(Direction d1, Vec v2)
        {
            return d1.Offset + v2;
        }

        /// <summary>
        /// Enumerates the directions in clockwise order, starting with <see cref="N"/>.
        /// </summary>
        public static IList<Direction> Clockwise
        {
            get { return new List<Direction> { N, NE, E, SE, S, SW, W, NW }; }
        }

        /// <summary>
        /// Enumerates the directions in counterclockwise order, starting with <see cref="N"/>.
        /// </summary>
        public static IList<Direction> Counterclockwise
        {
            get { return new List<Direction> { N, NW, W, SW, S, SE, E, NE }; }
        }

        /// <summary>
        /// Enumerates the four main compass directions.
        /// </summary>
        public static IList<Direction> Nsew
        {
            get { return new List<Direction> { N, S, E, W }; }
        }

        /// <summary>
        /// Gets a Direction heading from the origin towards the given position.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static Direction Towards(Vec pos)
        {
            Vec offset = Vec.Zero;

            if (pos.X < 0) offset.X = -1;
            if (pos.X > 0) offset.X = 1;
            if (pos.Y < 0) offset.Y = -1;
            if (pos.Y > 0) offset.Y = 1;

            return new Direction(offset);
        }

        public static bool operator ==(Direction left, Direction right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Direction left, Direction right)
        {
            return !left.Equals(right);
        }

        public Vec Offset { get { return mOffset; } }

        /// <summary>
        /// Gets the <see cref="Direction"/> following this one in clockwise order.
        /// Will wrap around. If this direction is None, then returns None.
        /// </summary>
        public Direction Next
        {
            get
            {
                if (this == N) return NE;
                else if (this == NE) return E;
                else if (this == E) return SE;
                else if (this == SE) return S;
                else if (this == S) return SW;
                else if (this == SW) return W;
                else if (this == W) return NW;
                else if (this == NW) return N;
                else return None;
            }
        }

        /// <summary>
        /// Gets the <see cref="Direction"/> following this one in counterclockwise
        /// order. Will wrap around. If this direction is None, then returns None.
        /// </summary>
        public Direction Previous
        {
            get
            {
                if (this == N) return NW;
                else if (this == NE) return N;
                else if (this == E) return NE;
                else if (this == SE) return E;
                else if (this == S) return SE;
                else if (this == SW) return S;
                else if (this == W) return SW;
                else if (this == NW) return W;
                else return None;
            }
        }

        public Direction RotateLeft90 { get { return Previous.Previous; } }

        public Direction RotateRight90 { get { return Next.Next; } }

        public Direction Rotate180 { get { return new Direction(mOffset * -1); } }

        public override bool Equals(object obj)
        {
            // check the type
            if (obj == null) return false;
            if (!(obj is Direction)) return false;

            // call the typed Equals
            return Equals((Direction)obj);
        }

        public override int GetHashCode()
        {
            return Offset.GetHashCode();
        }

        public override string ToString()
        {
            if (this == N) return "N";
            else if (this == NE) return "NE";
            else if (this == E) return "E";
            else if (this == SE) return "SE";
            else if (this == S) return "S";
            else if (this == SW) return "SW";
            else if (this == W) return "W";
            else if (this == NW) return "NW";
            else if (this == None) return "None";

            return Offset.ToString();
        }

        private Direction(Vec offset)
        {
            mOffset = offset;
        }

        #region IEquatable<Direction> Members

        public bool Equals(Direction other)
        {
            return Offset.Equals(other.Offset);
        }

        #endregion

        private Vec mOffset;
    }
}
