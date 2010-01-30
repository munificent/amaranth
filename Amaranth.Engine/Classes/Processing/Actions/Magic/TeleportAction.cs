using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    public class TeleportAction : Action
    {
        /// <summary>
        /// Initializes a new TeleportAction for an unwillful teleport caused by a hit.
        /// </summary>
        /// <param name="entity">The <see cref="Entity"/> being teleported.</param>
        /// <param name="distance">The maximum distance teleported.</param>
        public TeleportAction(Entity entity, Hit hit, int distance)
            : this(entity, hit, distance, false)
        {
        }

        /// <summary>
        /// Initializes a new TeleportAction for a willful teleport.
        /// </summary>
        /// <param name="entity">The <see cref="Entity"/> being teleported.</param>
        /// <param name="distance">The maximum distance teleported.</param>
        public TeleportAction(Entity entity, int distance)
            : this(entity, null, distance, true)
        {
        }

        /// <summary>
        /// Initializes a new TeleportAction.
        /// </summary>
        /// <param name="entity">The <see cref="Entity"/> being teleported.</param>
        /// <param name="distance">The maximum distance teleported.</param>
        /// <param name="isWillful"><c>true</c> if the Entity is intentionally teleporting
        /// itself, <c>false</c> if it is being teleported against its will.</param>
        private TeleportAction(Entity entity, Hit hit, int distance, bool isWillful)
            : base(entity)
        {
            //### bob: use int to increase distance?
            mHit = hit;
            mDistance = distance;
            mIsWillful = isWillful;
        }

        protected override ActionResult OnProcess()
        {
            if (!mIsWillful && Entity.StandsFirm(mHit))
            {
                Log(LogType.Resist, "{subject} stand[s] firm.");

                return ActionResult.Done;
            }

            int halfDistanceSquared = 0;

            // if teleporting on purpose, try to move at a least a certain distance away
            if (mIsWillful)
            {
                halfDistanceSquared = (mDistance / 2) * (mDistance / 2);
            }

            Vec pos = Entity.Position;

            // try to find a destination at least half the max distance away, but settle for closer if needed
            int maxDistance = mDistance;
            int minDistance = mDistance / 2;
            while (!Dungeon.TryFindOpenTileWithin(Entity.Position, mDistance / 2, mDistance, out pos) && (maxDistance > 1))
            {
                // decrease the distance
                maxDistance = minDistance - 1;
                minDistance = maxDistance / 2;
            }

            if (Entity.Position != pos)
            {
                Element element = Element.Air;
                if (mHit != null)
                {
                    element = mHit.Attack.Element;
                }

                AddEffect(new Effect(Entity.Position, EffectType.Teleport, element));

                Entity.Position = pos;

                AddEffect(new Effect(Entity.Position, EffectType.Teleport, element));

                if (mHit == null)
                {
                    Log(LogType.Message, "{subject} [are|is] magically transported.");
                }
                else if (mHit.Attack.Element == Element.Air)
                {
                    Log(LogType.BadState, "{subject} [are|is] blown about by the wind!");
                }
            }

            return ActionResult.Done;
        }

        private Hit mHit;
        private int mDistance;
        private bool mIsWillful;
    }
}
