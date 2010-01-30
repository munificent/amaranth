using System;
using System.Collections.Generic;
using System.Text;

namespace Amaranth.Util
{
    [Serializable]
    public class Array2D<T>
    {
        public Vec Size { get { return new Vec(Width, Height); } }

        public Rect Bounds { get { return new Rect(Size); } }

        public int Width { get { return mWidth; } }
        public int Height { get { return mValues.Length / mWidth; } }

        public T this[Vec pos]
        {
            get { return this[pos.X, pos.Y]; }
            set { this[pos.X, pos.Y] = value; }
        }

        public T this[int x, int y]
        {
            get { return mValues[GetIndex(x, y)]; }
            set { mValues[GetIndex(x, y)] = value; }
        }

        public Array2D(int width, int height)
        {
            if (width < 0) throw new ArgumentException("Width must be greater than zero.");
            if (height < 0) throw new ArgumentException("Height must be greater than zero.");

            mWidth = width;
            mValues = new T[width * height];
        }

        public Array2D(Vec size)
            : this(size.X, size.Y)
        {
        }

        public void ForEach(Action<T> action)
        {
            foreach (Vec pos in new Rect(Size))
            {
                // perform the action
                action(this[pos]);
            }
        }

        public void SetAll(Func<Vec, T> function)
        {
            foreach (Vec pos in new Rect(Size))
            {
                this[pos] = function(pos);
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
