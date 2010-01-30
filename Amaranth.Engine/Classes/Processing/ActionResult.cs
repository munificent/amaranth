using System;
using System.Collections.Generic;
using System.Text;

namespace Amaranth.Engine
{
    [Flags]
    public enum ActionResultFlags
    {
        Default         = 0x0000,
        Done            = 0x0001,
        NeedsPause      = 0x0002,
        Failed          = 0x0004,
        CheckForCancel  = 0x0008
    }

    public class ActionResult
    {
        /// <summary>
        /// Gets a typical ActionResult for an Action that is done and does not need a pause.
        /// </summary>
        public static ActionResult Done { get { return new ActionResult(ActionResultFlags.Done); } }

        /// <summary>
        /// Gets a typical ActionResult for an Action that is done and does need a pause.
        /// </summary>
        public static ActionResult DoneAndPause { get { return new ActionResult(ActionResultFlags.Done | ActionResultFlags.NeedsPause); } }

        /// <summary>
        /// Gets a typical ActionResult for an Action that is still going.
        /// </summary>
        public static ActionResult NotDone { get { return new ActionResult(ActionResultFlags.Default); } }

        /// <summary>
        /// Gets a typical ActionResult for an Action failed to complete.
        /// </summary>
        public static ActionResult Fail { get { return new ActionResult(ActionResultFlags.Failed | ActionResultFlags.Done); } }

        /// <summary>
        /// Gets a typical ActionResult for an Action that needs to check for a cancel.
        /// </summary>
        public static ActionResult CheckForCancel { get { return new ActionResult(ActionResultFlags.CheckForCancel | ActionResultFlags.Done); } }

        /// <summary>
        /// Gets the alternate action that should be attempted since the previous attempted action is not valid.
        /// </summary>
        /// <example>If an Entity attempts to walk (<see cref="WalkAction"/>) into a door, the walk obviously cannot be completely. Instead,
        /// it will automatically return an <see cref="OpenDoorAction"/> to open the door and perform that Action.</example>
        public Action Alternate { get { return mAlternate; } }

        public bool Success { get { return !IsFlagSet(ActionResultFlags.Failed); } }
        public bool NeedsPause { get { return IsFlagSet(ActionResultFlags.NeedsPause); } }
        public bool IsDone { get { return IsFlagSet(ActionResultFlags.Done); } }
        public bool NeedsCheckForCancel { get { return IsFlagSet(ActionResultFlags.CheckForCancel); } }

        public ActionResult(ActionResultFlags flags)
        {
            mFlags = flags;
        }

        public ActionResult(Action alternate)
        {
            mAlternate = alternate;
        }

        private bool IsFlagSet(ActionResultFlags flags)
        {
            return (mFlags & flags) == flags;
        }

        private ActionResultFlags mFlags;

        private Action mAlternate;
    }
}
