using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// <see cref="IDrop"/> that drops the results of one of a collection of drops, given the minimum required
    /// level for each item. Works similar to ChooseByLevelDrop except that this has a greater chance of
    /// choosing items above and below the chosen level.
    /// </summary>
    public class ChooseNearLevelDrop<T> : CollectionDropBase<T>, IDrop<T>
    {
        #region IDrop Members

        public IEnumerable<T> Create(int level)
        {
            // find the drops before and after the level
            int before = -1;
            int after = -1;

            for (int i = 0; i < Choices.Count; i++)
            {
                if (Choices[i].Odds <= level)
                {
                    before = i;
                }
                else if (Choices[i].Odds >= level)
                {
                    after = i;
                    break;
                }
            }

            if (before == -1) before = after;
            if (after == -1) after = before;

            // choose between the two drops, weighted by the level's distance to each one
            int index = before;
            if (before != after)
            {
                if (Rng.IntInclusive((int)Choices[before].Odds, (int)Choices[after].Odds) <= level)
                {
                    index = after;
                }
                else
                {
                    index = before;
                }
            }

            const int ChanceOfDecrement = 40; // out of 100
            const int ChanceOfIncrement = 10;

            // randomly walk through the drops
            if (Rng.Int(100) < ChanceOfDecrement)
            {
                // walk down
                while (index > 0)
                {
                    index--;

                    if (Rng.Int(100) >= ChanceOfDecrement) break;
                }
            }
            else if (Rng.Int(100) < ChanceOfIncrement)
            {
                // walk up
                while (index < Choices.Count - 1)
                {
                    index++;

                    if (Rng.Int(100) >= ChanceOfIncrement) break;
                }
            }

            // drop it
            foreach (var item in Choices[index].Drop.Create(level))
            {
                yield return item;
            }
        }

        #endregion
    }
}
