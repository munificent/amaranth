using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Util
{
    [Serializable]
    public class RangedInt
    {
        public static implicit operator int(RangedInt ranged)
        {
            return ranged.Current;
        }

        public event EventHandler Changed;

        public event EventHandler<ValueChangeEventArgs<int>> CurrentChanged;
        public event EventHandler<ValueChangeEventArgs<int>> MinChanged;
        public event EventHandler<ValueChangeEventArgs<int>> MaxChanged;

        public int Current
        {
            get { return mCurrent; }
            set
            {
                value = Math.Max(mMin, value);
                value = Math.Min(mMax, value);

                if (mCurrent != value)
                {
                    int oldValue = mCurrent;
                    mCurrent = value;

                    if (CurrentChanged != null) CurrentChanged(this, new ValueChangeEventArgs<int>(oldValue, value));
                    if (Changed != null) Changed(this, EventArgs.Empty);
                }
            }
        }

        public int Min
        {
            get { return mMin; }
            set
            {
                if (mMin != value)
                {
                    // setting the min above the max pushes the max up too
                    Max = Math.Max(mMax, value);

                    int oldValue = mMin;
                    mMin = value;

                    if (MinChanged != null) MinChanged(this, new ValueChangeEventArgs<int>(oldValue, value));
                    if (Changed != null) Changed(this, EventArgs.Empty);

                    // make sure the current is still in bounds
                    Current = Current;
                }
            }
        }

        public int Max
        {
            get { return mMax; }
            set
            {
                if (mMax != value)
                {
                    // setting the max below the min pushes the min down too
                    Min = Math.Min(mMin, value);

                    int oldValue = mMax;
                    mMax = value;

                    if (MaxChanged != null) MaxChanged(this, new ValueChangeEventArgs<int>(oldValue, value));
                    if (Changed != null) Changed(this, EventArgs.Empty);

                    // make sure the current is still in bounds
                    Current = Current;
                }
            }
        }

        public bool IsMax { get { return mCurrent == mMax; } }

        public bool IsMin { get { return mCurrent == mMin; } }

        public RangedInt(int current, int min, int max)
        {
            if (max < min) throw new ArgumentException("The maximum cannot be less than the minimum.");

            mMin = min;
            mMax = max;

            mCurrent = 0;

            Current = current;
        }

        public RangedInt(int max)
            : this(max, 0, max)
        {
        }

        public override string ToString()
        {
            if (mMin != 0)
            {
                return mCurrent + " / (" + mMin + " - " + mMax + ")";
            }
            else
            {
                return mCurrent + " / " + mMax;
            }
        }

        private int mCurrent;
        private int mMin;
        private int mMax;
    }
}
