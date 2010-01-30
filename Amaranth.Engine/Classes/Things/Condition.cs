using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// Base class for status conditions that need per turn processing (unlike bonuses).
    /// </summary>
    [Serializable]
    public class Condition
    {
        /// <summary>
        /// Raised when the Condition has either started or finished.
        /// </summary>
        internal event EventHandler<EventArgs> Changed;

        /// <summary>
        /// Gets whether the Condition is currently in effect.
        /// </summary>
        public bool IsActive { get { return mTurnsRemaining != 0; } }

        /// <summary>
        /// Gets the number of turns remaining on the Condition.
        /// </summary>
        public int TurnsRemaining { get { return mTurnsRemaining; } }

        public Condition(Entity entity, Func<Entity, Action> updateCallback, Func<Entity, Action> completeCallback)
        {
            mEntity = entity;
            mUpdate = updateCallback;
            mComplete = completeCallback;
        }

        public Condition(Entity entity, Func<Entity, Action> completeCallback)
            : this(entity, null, completeCallback)
        {
        }

        /// <summary>
        /// Deactivates the Condition. Does nothing if the Condition is not active.
        /// </summary>
        public Action Deactivate()
        {
            Action action = null;

            bool wasActive = IsActive;

            mTurnsRemaining = 0;

            // shut it down if active
            if (wasActive)
            {
                if (mComplete != null)
                {
                    action = mComplete(mEntity);
                }
                OnChanged();
            }

            return action;
        }

        /// <summary>
        /// Updates the Condition.
        /// </summary>
        public Action Update()
        {
            Action action = null;

            if (IsActive)
            {

                // positive count means it's timed
                if (mTurnsRemaining >= 0)
                {
                    mTurnsRemaining--;
                }

                // if the condition has run its course, complete it
                if (!IsActive)
                {
                    if (mComplete != null)
                    {
                        action = mComplete(mEntity);
                    }
                    OnChanged();
                }
                else
                {
                    // otherwise update it
                    if (mUpdate != null)
                    {
                        action = mUpdate(mEntity);
                    }
                }
            }

            return action;
        }

        /// <summary>
        /// Increments the duration of the Condition by the given number of turns. Will
        /// start the Condition if not already active. Otherwise, just increases the
        /// duration.
        /// </summary>
        /// <param name="duration">How long (or how much longer) the Condition should last.</param>
        public void AddDuration(int duration)
        {
            SetDuration(mTurnsRemaining + duration);
        }

        /// <summary>
        /// Replaces the duration of the Condition by the given number of turns. Will
        /// start the Condition if not already active. Otherwise, just increases the
        /// duration.
        /// </summary>
        /// <param name="duration">How long (or how much longer) the Condition should last.</param>
        public void SetDuration(int duration)
        {
            if (duration <= 0) throw new ArgumentOutOfRangeException("Must set the duration to a positive amount.");
            if (mTurnsRemaining < 0) throw new InvalidOperationException("Cannot increment the duration on a Condition that is set to run indefinitely.");

            bool wasActive = IsActive;

            // add to the duration
            mTurnsRemaining = duration;

            // if turning it on for the first time, note the change
            if (!wasActive)
            {
                OnChanged();
            }
        }

        private void OnChanged()
        {
            if (Changed != null) Changed(this, EventArgs.Empty);
        }

        private int mTurnsRemaining;
        private Entity mEntity;
        private Func<Entity, Action> mUpdate;
        private Func<Entity, Action> mComplete;
    }
}
