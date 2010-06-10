using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bramble.Core;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// <see cref="IDrop"/> that drops the results of one of a collection of drops, given the minimum required
    /// level for each item. Will drop the last item in the list whose level is less than or equal to the
    /// given level (which is randomly perturbed a bit). Note that if the given level is less than the minimum
    /// level of all items, then nothing may be dropped.
    /// </summary>
    public class ChooseByLevelDrop<T> : CollectionDropBase<T>, IDrop<T>
    {
        #region IDrop Members

        public IEnumerable<T> Create(int level)
        {
            // modify the level randomly
            float choiceValue = Rng.WalkLevel(level).Clamp(1, 100);

            IDrop<T> dropped = null;

            // count through to find the last one at least the random level
            foreach (DropChoice choice in Choices)
            {
                if (choiceValue >= choice.Odds)
                {
                    dropped = choice.Drop;
                }
            }

            // drop it if we have one
            if (dropped != null)
            {
                foreach (var item in dropped.Create(level))
                {
                    yield return item;
                }
            }
        }

        #endregion
    }
}
