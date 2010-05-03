using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// <see cref="IDrop"/> that drops a given IDrop multiple times.
    /// </summary>
    public class RepeatDrop<T> : IDrop<T>
    {
        public RepeatDrop(NotNull<Roller> repeat, IDrop<T> drop)
        {
            if (drop == null) throw new ArgumentNullException("drop");

            mRepeat = repeat;
            mDrop = drop;
        }

        #region IDrop Members

        public IEnumerable<T> Create(int level)
        {
            foreach (int value in Enumerable.Range(0, mRepeat.Roll()))
            {
                foreach (var item in mDrop.Create(level))
                {
                    yield return item;
                }
            }
        }

        #endregion

        private Roller mRepeat;
        private IDrop<T> mDrop;
    }
}
