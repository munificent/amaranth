using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// <see cref="IDrop"/> that drops one or more of a collection of drops.
    /// </summary>
    public class AllDrop<T> : CollectionDropBase<T>, IDrop<T>
    {
        #region IDrop Members

        public IEnumerable<T> Create(int level)
        {
            foreach (DropChoice choice in Choices)
            {
                // see if this choice was dropped
                if ((choice.Odds == 0) || (Rng.Float(TotalOdds) <= choice.Odds))
                {
                    foreach (var item in choice.Drop.Create(level))
                    {
                        yield return item;
                    }
                }
            }
        }

        #endregion
    }
}