using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    /*
     * monster behavior styles:
     * 
     * jelly: doesn't move. attacks if adjacent. totally machinelike
     * spider:
     *      if in light adjacent to dark, runs to dark
     *      if in light not adjacent to dark, runs toward hero
     *      if in dark, runs toward hero but not into light
     * zombie:
     *      run towards hero, not smart enough to go around walls
     * archer:
     *      tries to keep a fixed distance between itself and hero with open los
     * hound:
     *      generally seeks towards hero but moves erratically with a preference
     *      for turning in one direction away from the hero, so that it attacks
     *      but also circles
     */

    /// <summary>
    /// A Behavior represents the AI (which may be player-controlled) of an Entity. A Behavior's
    /// responsibility is to emit <see cref="Action">Actions</see> when requested.
    /// </summary>
    [Serializable]
    public abstract class Behavior
    {
        /// <summary>
        /// Override this to indicate whether or not this Behavior needs user input before it
        /// can return the next Action. Defaults to false.
        /// </summary>
        public virtual bool NeedsUserInput
        {
            get { return false; }
        }

        public abstract Action NextAction();

        /// <summary>
        /// Called when something has occurred to disturb the entity, such as being hit.
        /// </summary>
        public virtual void Disturb() { }

        /// <summary>
        /// Called when the user has indicated they want the current Behavior to cancel.
        /// </summary>
        public virtual void Cancel() { }
    }
}
