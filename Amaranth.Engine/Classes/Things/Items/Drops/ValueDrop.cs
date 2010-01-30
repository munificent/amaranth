using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    /// <summary>
    /// Simple drop that always drops a fixed value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ValueDrop<T> : IDrop<T>
    {
        public ValueDrop(T value)
        {
            mValue = value;
        }

        #region IDrop<T> Members

        public IEnumerable<T> Create(int level)
        {
            yield return mValue;
        }

        #endregion

        private T mValue;
    }
}
