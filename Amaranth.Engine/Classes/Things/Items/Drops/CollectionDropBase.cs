using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    /// <summary>
    /// Base class for <see cref="IDrop"/> classes that contain collection of
    /// drops.
    /// </summary>
    public abstract class CollectionDropBase<T>
    {
        public void Add(IDrop<T> drop, float odds)
        {
            mChoices.Add(new DropChoice(drop, odds));
        }

        protected IList<DropChoice> Choices { get { return mChoices; } }

        protected class DropChoice
        {
            public IDrop<T> Drop;
            public float Odds;

            public DropChoice(IDrop<T> drop, float odds)
            {
                Drop = drop;
                Odds = odds;
            }
        }

        /// <summary>
        /// Odds are given in percent.
        /// </summary>
        protected const float TotalOdds = 100.0f;

        private readonly List<DropChoice> mChoices = new List<DropChoice>();
    }
}
