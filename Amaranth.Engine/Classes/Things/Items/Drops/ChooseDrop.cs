using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    /// <summary>
    /// <see cref="IDrop"/> that drops the results of one of a collection of drops, given weighted
    /// odds for each.
    /// </summary>
    public class ChooseDrop<T> : CollectionDropBase<T>, IDrop<T>
    {
        /// <summary>
        /// Gives default odds to choices that don't have them.
        /// </summary>
        public void FixOdds()
        {
            foreach (DropChoice choice in Choices)
            {
                // give it even odds out of the total
                if (choice.Odds == 0) choice.Odds = TotalOdds / Choices.Count;
            }
        }

        #region IDrop Members

        public IEnumerable<T> Create(int level)
        {
            // use the actual total in case the data adds up to more than 100
            float totalOdds = Math.Max(TotalOdds, Choices.Sum((choice) => choice.Odds));
            float choiceValue = Rng.Float(TotalOdds);

            float original = choiceValue;

            IDrop<T> dropped = null;

            // count through to find the chosen one
            foreach (DropChoice choice in Choices)
            {
                choiceValue -= choice.Odds;
                if (choiceValue < 0)
                {
                    dropped = choice.Drop;
                    break;
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
            else
            {
                if (typeof(T).Equals(typeof(CreateFeature)))
                {
                    Console.WriteLine(original.ToString());
                    System.Diagnostics.Debugger.Break();
                }
            }
        }

        #endregion
    }
}
