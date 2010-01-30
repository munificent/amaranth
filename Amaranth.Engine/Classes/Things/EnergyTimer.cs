using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    [Serializable]
    public class EnergyTimer
    {
        /// <summary>
        /// Gets the label for this timer.
        /// </summary>
        public string Label { get { return mLabel; } }

        /// <summary>
        /// Initializes a new EnergyTimer.
        /// </summary>
        /// <param name="label">An identifier for the timer.</param>
        /// <param name="elapsedAction">The <see cref="System.Action"/> to perform when the number of
        /// steps has elapsed.</param>
        /// <param name="steps">The number of steps (at normal speed) before the elapsed timer fires.</param>
        /// <param name="stepAction">The <see cref="System.Action"/> to perform during each step.</param>
        /// <param name="repeat">True if the timer should reset itself after the number of steps has
        /// elapsed.</param>
        public EnergyTimer(string label, System.Action elapsedAction, int steps, System.Action stepAction, bool repeat)
        {
            mLabel = label;
            mElapsedAction = elapsedAction;
            mEnergyCost = steps * Energy.ActionCost;
            mStepAction = stepAction;
            mElapsed = 0;
            mRepeat = repeat;
        }

        /// <summary>
        /// Initializes a new EnergyTimer.
        /// </summary>
        /// <param name="elapsedAction">The <see cref="System.Action"/> to perform when the number of
        /// steps has elapsed.</param>
        /// <param name="steps">The number of steps (at normal speed) before the elapsed timer fires.</param>
        /// <param name="repeat">True if the timer should reset itself after the number of steps has
        /// elapsed.</param>
        public EnergyTimer(System.Action elapsedAction, int steps, bool repeat)
            : this(String.Empty, elapsedAction, steps, null, repeat)
        {
        }

        /// <summary>
        /// Initializes a new EnergyTimer.
        /// </summary>
        /// <param name="steps">The number of steps (at normal speed) before the elapsed timer fires.</param>
        /// <param name="stepAction">The <see cref="System.Action"/> to perform during each step.</param>
        /// <param name="repeat">True if the timer should reset itself after the number of steps has
        /// elapsed.</param>
        public EnergyTimer(int steps, System.Action stepAction, bool repeat)
            : this(String.Empty, null, steps, stepAction, repeat)
        {
        }

        public bool Elapse(int energy)
        {
            bool done = false;

            int lastStep = mElapsed / Energy.ActionCost;

            mElapsed += energy;

            int thisStep = mElapsed / Energy.ActionCost;

            // perform the step action
            if (thisStep > lastStep)
            {
                if (mStepAction != null) mStepAction();
            }

            if (mElapsed >= mEnergyCost)
            {
                // perform the final elapsed action
                if (mElapsedAction != null) mElapsedAction();

                if (mRepeat)
                {
                    // reset
                    mElapsed -= mEnergyCost;
                }
                else
                {
                    done = true;
                }
            }

            return done;
        }

        private string mLabel;
        private System.Action mStepAction;
        private System.Action mElapsedAction;
        private int mEnergyCost;
        private int mElapsed;
        private bool mRepeat;
    }

    [Serializable]
    public class EnergyTimerCollection : List<EnergyTimer>
    {
        /// <summary>
        /// Raised when a <see cref="Timer"/> is removed from the collection.
        /// </summary>
        internal event EventHandler<EventArgs> TimerRemoved;

        /// <summary>
        /// Returns <c>true</c> if there are any timers in the collection with the given label.
        /// </summary>
        /// <param name="label">The timer label to look for.</param>
        /// <returns><c>true</c> if there is at least one <see cref="Timer"/> with the label.</returns>
        public bool Has(string label)
        {
            return this.Any((timer) => timer.Label == label);
        }

        public void Remove(string label)
        {
            int count = Count;

            this.RemoveAll((timer) => timer.Label == label);

            // notify if anything was removed
            if (count != Count) OnTimerRemoved();
        }

        public void Gain(int energy)
        {
            int count = Count;

            // increment the timers, and remove the completed ones
            this.RemoveAll((timer) => timer.Elapse(energy));

            // notify if anything was removed
            if (count != Count) OnTimerRemoved();
        }

        private void OnTimerRemoved()
        {
            if (TimerRemoved != null) TimerRemoved(this, EventArgs.Empty);
        }
    }
}
