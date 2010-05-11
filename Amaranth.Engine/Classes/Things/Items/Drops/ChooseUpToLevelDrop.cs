using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// <see cref="IDrop"/> that drops the results of one of a collection of drops, given the minimum required
    /// level for each item. Will drop a random item in the list whose level is less than or equal to the
    /// given level (which is randomly perturbed a bit). Note that if the random level chosen is less than the minimum
    /// level of all items, then nothing may be dropped.
    /// <para>
    /// This is similar to <see cref="ChooseByLevelDrop"/> except that this chooses any item up to the level where
    /// ChooseByLevelDrop always prefers the highest level item available.
    /// </para>
    /// </summary>
    public class ChooseUpToLevelDrop<T> : CollectionDropBase<T>, IDrop<T>
    {
        #region IDrop Members

        public IEnumerable<T> Create(int level)
        {
            // modify the level randomly
            int walkedLevel = Rng.WalkLevel(level).Clamp(1, 100);

            // choose up to it (best of two tries)
            int choiceValue = Math.Max(Rng.IntInclusive(walkedLevel),
                                       Rng.IntInclusive(walkedLevel));

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
                // if the choice was less than the max, then bump up the subsequent level a bit to compensate
                int bonus = (walkedLevel - choiceValue) / 4;

                level = Math.Max(Game.MaxDepth, level + bonus);

                foreach (var item in dropped.Create(level))
                {
                    yield return item;
                }
            }
        }

        #endregion
    }
}
