using System;
using System.Collections.Generic;
using System.Text;

namespace Amaranth.Util
{
    /// <summary>
    /// Generic fixed-size two-dimensional array class.
    /// </summary>
    /// <typeparam name="T">The array element type.</typeparam>
    [Serializable]
    public class Array2D<T>
    {
        /// <summary>
        /// Gets the size of the array.
        /// </summary>
        public Vec Size { get { return new Vec(Width, Height); } }

        /// <summary>
        /// Gets the bounds of the array. The top-level coordinate will be the origin.
        /// </summary>
        public Rect Bounds { get { return new Rect(Size); } }

        /// <summary>
        /// Gets the width of the array.
        /// </summary>
        public int Width { get { return mWidth; } }

        /// <summary>
        /// Gets the height of the array.
        /// </summary>
        public int Height { get { return mValues.Length / mWidth; } }

        /// <summary>
        /// Gets and sets the array element at the given position.
        /// </summary>
        /// <param name="pos">The position of the element. Must be within bounds.</param>
        /// <exception cref="IndexOutOfBoundsException">if the position is out of bounds.</exception>
        public T this[Vec pos]
        {
            get { return this[pos.X, pos.Y]; }
            set { this[pos.X, pos.Y] = value; }
        }

        /// <summary>
        /// Gets and sets the array element at the given position.
        /// </summary>
        /// <param name="x">The X-coordinate of the element.</param>
        /// <param name="x">The Y-coordinate of the element.</param>
        /// <exception cref="IndexOutOfBoundsException">if the position is out of bounds.</exception>
        public T this[int x, int y]
        {
            get { return mValues[GetIndex(x, y)]; }
            set { mValues[GetIndex(x, y)] = value; }
        }

        /// <summary>
        /// Initializes a new instance of Array2D with the given dimensions.
        /// </summary>
        /// <param name="width">Width of the array.</param>
        /// <param name="height">Height of the array.</param>
        public Array2D(int width, int height)
        {
            if (width < 0) throw new ArgumentException("Width must be greater than zero.");
            if (height < 0) throw new ArgumentException("Height must be greater than zero.");

            mWidth = width;
            mValues = new T[width * height];
        }

        /// <summary>
        /// Initializes a new instance of Array2D with the given size.
        /// </summary>
        /// <param name="size">Size of the array.</param>
        public Array2D(Vec size)
            : this(size.X, size.Y)
        {
        }

        /// <summary>
        /// Fills all of the elements in the array with the given value.
        /// </summary>
        /// <param name="value">The value to fill the array with.</param>
        public void Fill(T value)
        {
            foreach (Vec pos in new Rect(Size))
            {
                this[pos] = value;
            }
        }

        /// <summary>
        /// Fills all of the elements in the array with values returned by the given callback.
        /// </summary>
        /// <param name="callback">The function to call for each element in the array.</param>
        public void Fill(Func<Vec, T> callback)
        {
            foreach (Vec pos in new Rect(Size))
            {
                this[pos] = callback(pos);
            }
        }

        private void CheckBounds(Vec pos)
        {
            if (pos.X < 0) throw new ArgumentOutOfRangeException("pos.X");
            if (pos.X >= Width) throw new ArgumentOutOfRangeException("pos.X");
            if (pos.Y < 0) throw new ArgumentOutOfRangeException("pos.Y");
            if (pos.Y >= Height) throw new ArgumentOutOfRangeException("pos.Y");
        }

        private int GetIndex(int x, int y)
        {
            return (y * mWidth) + x;
        }

        private int mWidth;
        private readonly T[] mValues;
    }
}
