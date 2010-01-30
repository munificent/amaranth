using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// The collection of <see cref="Condition">Conditions</see> that can affect a single <see cref="Entity"/>.
    /// </summary>
    [Serializable]
    public class EntityConditions
    {
        internal event EventHandler<EventArgs> ConditionChanged;

        public Condition Poison { get { return mPoison; } }
        public Condition Haste { get { return mHaste; } }
        public Condition Freeze { get { return mFreeze; } }
        public Condition Slow { get { return mSlow; } }

        public EntityConditions(NotNull<Entity> entity)
        {
            mPoison =   new Condition( entity, theEntity => new PoisonDamageAction(theEntity),
                                               theEntity => new PoisonCompleteAction(theEntity));
            Add(mPoison);

            mHaste =    new Condition( entity, theEntity => new HasteCompleteAction(theEntity));
            Add(mHaste);

            mFreeze =   new Condition( entity, theEntity => new FreezeCompleteAction(theEntity));
            Add(mFreeze);

            mSlow =     new Condition( entity, theEntity => new SlowCompleteAction(theEntity));
            Add(mSlow);
        }

        public IEnumerable<Action> Update()
        {
            foreach (Condition condition in mConditions)
            {
                Action action = condition.Update();
                if (action != null) yield return action;
            }
        }

        private void Add(Condition condition)
        {
            mConditions.Add(condition);
            condition.Changed += Condition_Changed;
        }

        private void Condition_Changed(object sender, EventArgs e)
        {
            if (ConditionChanged != null) ConditionChanged(this, EventArgs.Empty);
        }

        private Condition mPoison;
        private Condition mHaste;
        private Condition mFreeze;
        private Condition mSlow;

        private readonly List<Condition> mConditions = new List<Condition>();
    }
}
