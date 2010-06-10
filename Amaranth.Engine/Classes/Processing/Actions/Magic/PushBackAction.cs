using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bramble.Core;

using Amaranth.Util;

namespace Amaranth.Engine
{
    class PushBackAction : Action
    {
        public PushBackAction(Entity entity, Hit hit)
            : base(entity)
        {
            mHit = hit;
        }

        protected override ActionResult OnProcess()
        {
            if (Entity.StandsFirm(mHit))
            {
                Log(LogType.Resist, "{subject} stand[s] firm against the waves.");

                return ActionResult.Done;
            }

            // get a weighted list of the possible destinations
            List<Vec> positions = new List<Vec>();

            Vec straight = Entity.Position + mHit.Direction;
            Vec left45 = Entity.Position + mHit.Direction.Previous;
            Vec right45 = Entity.Position + mHit.Direction.Next;
            Vec left90 = Entity.Position + mHit.Direction.RotateLeft90;
            Vec right90 = Entity.Position + mHit.Direction.RotateRight90;

            // directly away
            if (Entity.CanOccupy(straight))
            {
                // more likely than other directions
                positions.Add(straight);
                positions.Add(straight);
                positions.Add(straight);
                positions.Add(straight);
            }

            // off to one side
            if (Entity.CanOccupy(left45))
            {
                positions.Add(left45);
                positions.Add(left45);
            }

            // off to the other side
            if (Entity.CanOccupy(right45))
            {
                positions.Add(right45);
                positions.Add(right45);
            }

            // off to one side
            if (Entity.CanOccupy(left90))
            {
                positions.Add(left90);
            }

            // off to the other side
            if (Entity.CanOccupy(right90))
            {
                positions.Add(right90);
            }

            // fail if nowhere to be pushed
            if (positions.Count == 0) return ActionResult.Fail;

            // pick a random direction
            AddEffect(new Effect(Entity.Position, EffectType.Teleport, mHit.Attack.Element));

            Entity.Position = Rng.Item(positions);

            AddEffect(new Effect(Entity.Position, EffectType.Teleport, mHit.Attack.Element));

            Log(LogType.BadState, "{subject} [are|is] knocked back by the water!");

            return ActionResult.Done;
        }

        private Hit mHit;
    }
}
