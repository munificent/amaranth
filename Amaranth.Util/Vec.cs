using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Amaranth.Util
{
    /// <summary>
    /// A 2D integer vector class. Similar to Point but not dependent on System.Drawing and much more
    /// feature-rich.
    /// </summary>
    [Serializable]
    public struct Vec : IEquatable<Vec>
    {
        /// <summary>
        /// Gets the zero vector.
        /// </summary>
        public static readonly Vec Zero = new Vec(0, 0);

        /// <summary>
        /// Gets the unit vector.
        /// </summary>
        public static readonly Vec One = new Vec(1, 1);

        #region Operators

        public static bool operator ==(Vec v1, Vec v2)
        {
            return v1.Equals(v2);
        }

        public static bool operator !=(Vec v1, Vec v2)
        {
            return !v1.Equals(v2);
        }

        public static Vec operator +(Vec v1, Vec v2)
        {
            return new Vec(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vec operator +(Vec v1, int i2)
        {
            return new Vec(v1.X + i2, v1.Y + i2);
        }

        public static Vec operator +(int i1, Vec v2)
        {
            return new Vec(i1 + v2.X, i1 + v2.Y);
        }

        public static Vec operator -(Vec v1, Vec v2)
        {
            return new Vec(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static Vec operator -(Vec v1, int i2)
        {
            return new Vec(v1.X - i2, v1.Y - i2);
        }

        public static Vec operator -(int i1, Vec v2)
        {
            return new Vec(i1 - v2.X, i1 - v2.Y);
        }

        public static Vec operator *(Vec v1, int i2)
        {
            return new Vec(v1.X * i2, v1.Y * i2);
        }

        public static Vec operator *(int i1, Vec v2)
        {
            return new Vec(i1 * v2.X, i1 * v2.Y);
        }

        public static Vec operator /(Vec v1, int i2)
        {
            return new Vec(v1.X / i2, v1.Y / i2);
        }

        #endregion

        /// <summary>
        /// Gets whether the distance between the two given <see cref="Vec">Vecs</see> is within
        /// the given distance.
        /// </summary>
        /// <param name="a">First Vec.</param>
        /// <param name="b">Second Vec.</param>
        /// <param name="distance">Maximum distance between them.</param>
        /// <returns><c>true</c> if the distance between <c>a</c> and <c>b</c> is less than or equal to <c>distance</c>.</returns>
        public static bool IsDistanceWithin(Vec a, Vec b, int distance)
        {
            Vec offset = a - b;

            return offset.LengthSquared <= (distance * distance);
        }

        public int X;
        public int Y;

        /// <summary>
        /// Gets the area of a rectangle with opposite corners at (0, 0) and this Vec.
        /// </summary>
        public int Area { get { return X * Y; } }

        /// <summary>
        /// Gets the absolute magnitude of the Vec squared.
        /// </summary>
        public int LengthSquared { get { return (X * X) + (Y * Y); } }

        /// <summary>
        /// Gets the absolute magnitude of the Vec.
        /// </summary>
        public float Length { get { return (float)Math.Sqrt(LengthSquared); } }

        /// <summary>
        /// Gets the rook length of the Vec, which is the number of squares a rook on a chessboard
        /// would need to move from (0, 0) to reach the endpoint of the Vec. Also known as
        /// Manhattan or taxicab distance.
        /// </summary>
        public int RookLength { get { return Math.Abs(X) + Math.Abs(Y); } }

        /// <summary>
        /// Gets the king length of the Vec, which is the number of squares a king on a chessboard
        /// would need to move from (0, 0) to reach the endpoint of the Vec. Also known as
        /// Chebyshev distance.
        /// </summary>
        public int KingLength { get { return Math.Max(Math.Abs(X), Math.Abs(Y)); } }

        /// <summary>
        /// Initializes a new instance of Vec with the given coordinates.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        public Vec(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Converts this Vec to a human-readable string.
        /// </summary>
        /// <returns>A string representation of the Vec.</returns>
        public override string ToString()
        {
            return X.ToString() + ", " + Y.ToString();
        }

        /// <summary>
        /// Specifies whether this Vec contains the same coordinates as the specified Object. 
        /// </summary>
        /// <param name="obj">Object to compare to.</param>
        /// <returns><c>true</c> if <c>object</c> is a Vec with the same coordinates.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Vec)
            {
                return Equals((Vec)obj);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a hash code for this Vec.
        /// </summary>
        /// <returns>An integer value that specifies a hash value for this Vec.</returns>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        /// <summary>
        /// Gets whether the given vector is within a rectangle
        /// from (0,0) to this vector (half-inclusive).
        /// </summary>
        public bool Contains(Vec vec)
        {
            if (vec.X < 0) return false;
            if (vec.X >= X) return false;
            if (vec.Y < 0) return false;
            if (vec.Y >= Y) return false;

            return true;
        }

        public bool IsAdjacentTo(Vec other)
        {
            // not adjacent to the exact same position
            if (this == other) return false;

            Vec offset = this - other;

            return (Math.Abs(offset.X) <= 1) && (Math.Abs(offset.Y) <= 1);
        }

        /// <summary>
        /// Returns a new Vec whose coordinates are the coordinates of this Vec
        /// with the given values added. This Vec is not modified.
        /// </summary>
        /// <param name="x">Distance to offset the X coordinate.</param>
        /// <param name="y">Distance to offset the Y coordinate.</param>
        /// <returns>A new Vec offset by the given coordinates.</returns>
        public Vec Offset(int x, int y)
        {
            return new Vec(X + x, Y + y);
        }

        /// <summary>
        /// Returns a new Vec whose coordinates are the coordinates of this Vec
        /// with the given value added to the X coordinate. This Vec is not modified.
        /// </summary>
        /// <param name="offset">Distance to offset the X coordinate.</param>
        /// <returns>A new Vec offset by the given X coordinate.</returns>
        public Vec OffsetX(int offset)
        {
            return new Vec(X + offset, Y);
        }

        /// <summary>
        /// Returns a new Vec whose coordinates are the coordinates of this Vec
        /// with the given value added to the Y coordinate. This Vec is not modified.
        /// </summary>
        /// <param name="offset">Distance to offset the Y coordinate.</param>
        /// <returns>A new Vec offset by the given Y coordinate.</returns>
        public Vec OffsetY(int offset)
        {
            return new Vec(X, Y + offset);
        }

        /// <summary>
        /// Returns a new Vec whose coordinates are the coordinates of this Vec
        /// with the given function applied.
        /// </summary>
        public Vec Each(Func<int, int> function)
        {
            if (function == null) throw new ArgumentNullException("function");

            return new Vec(function(X), function(Y));
        }

        #region IEquatable<Vec> Members

        public bool Equals(Vec other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        #endregion
    }
}
