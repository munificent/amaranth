using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// Generic <see cref="Action"/> for a projectile that moves in a straight line.
    /// </summary>
    public abstract class BoltAction : EnumerableAction
    {
        /// <summary>
        /// Creates a new BoltAction starting at the <see cref="Entity"/> and moving in the given <see cref="Direction"/>.
        /// </summary>
        public BoltAction(NotNull<Entity> entity, Vec target)
            : base(entity)
        {
            if (entity.Value.Position == target) throw new ArgumentException("The Entity and target must not have the same position.");

            mTarget = target;
        }

        /// <summary>
        /// Creates a new BoltAction starting at the <see cref="Entity"/> and moving in the given <see cref="Direction"/>.
        /// </summary>
        public BoltAction(NotNull<Entity> entity, Direction direction)
            : this(entity, entity.Value.Position + direction.Offset)
        {
            if (direction == Direction.None) throw new ArgumentException("Direction.None is not a valid direction.");
        }

        protected override IEnumerable<ActionResult> OnProcessEnumerable()
        {
            Los los = new Los(Dungeon, Entity.Position, mTarget);

            Vec lastPos = Entity.Position;
            foreach (Vec pos in los)
            {
                // see which direction it stepped
                Direction direction = Direction.Towards(pos - lastPos);
                lastPos = pos;

                // stop if the derived bolt says to
                if (OnEffect(pos, direction)) break;

                // keep going
                yield return ActionResult.NotDone;
            }
        }

        protected abstract bool OnEffect(Vec pos, Direction direction);

        private Vec mTarget;
    }
}