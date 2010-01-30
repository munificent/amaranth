using System;
using System.Collections.Generic;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    public abstract class Action
    {
        /// <summary>
        /// Wraps the Action in an ActionResult. Allows returning an alternate Action
        /// from an Action's Process method.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static implicit operator ActionResult(Action action)
        {
            return new ActionResult(action);
        }

        /// <summary>
        /// Gets the <see cref="Entity"/> performing this Action. May be null.
        /// </summary>
        public Entity Entity { get { return mEntity; } }

        /// <summary>
        /// Initializes a new instance of Action.
        /// </summary>
        /// <param name="entity">The <see cref="Entity"/> performing the Action.
        public Action(NotNull<Entity> entity)
        {
            mEntity = entity;
        }

        /// <summary>
        /// Initializes a new instance of Action.
        /// </summary>
        public Action(NotNull<Game> game)
        {
            mGame = game;
        }

        /// <summary>
        /// Marks this Action as one that will consume energy if successfully completed. Called
        /// by Game when the Action is requested from the Entity.
        /// </summary>
        public void MarkAsEnergyTaking()
        {
            mConsumesEnergy = true;
        }

        public ActionResult Process(IList<Effect> effects, Queue<Action> actions)
        {
            mEffects = effects;
            mActions = actions;

            ActionResult result = OnProcess();

            mEffects = null;
            mActions = null;

            return result;
        }

        public void AddEffect(NotNull<Effect> effect)
        {
            mEffects.Add(effect);
        }

        public void AddAction(NotNull<Action> action)
        {
            mActions.Enqueue(action);
        }

        public void AfterSuccess()
        {
            if (mConsumesEnergy && (mEntity != null))
            {
                mEntity.Energy.Spend();

                // only consume once
                mConsumesEnergy = false;
            }
        }

        #region Game accessors

        public Game Game { get { return mGame ?? mEntity.Dungeon.Game; } }
        public Dungeon Dungeon { get { return Game.Dungeon; } }

        /// <summary>
        /// Writes the given message to the <see cref="Log"/> using the given subject
        /// and object.
        /// </summary>
        /// <param name="message">The text of the message.</param>
        public void Log(LogType type, INoun subject, string message, INoun obj)
        {
            bool ignore = false;

            // the log type assumes the hero is the subject. if not, some things
            // change.
            if (!(subject is Hero))
            {
                switch (type)
                {
                    case LogType.Bad:
                        // good and bad are relative to the hero
                        type = LogType.Good;
                        break;

                    case LogType.Good:
                        // good and bad are relative to the hero
                        type = LogType.Bad;
                        break;

                    case LogType.DidNotWork:
                    case LogType.Fail:
                        // don't care if a monster does something useless
                        ignore = true;
                        break;
                }
            }

            if (!ignore)
            {
                message = Sentence.Format(message, subject, obj);
                Game.Log.Write(type, message);
            }
        }

        /// <summary>
        /// Writes the given message to the <see cref="Log"/> using the given subject
        /// and object.
        /// </summary>
        /// <param name="message">The text of the message.</param>
        public void Log(LogType type, string message, INoun obj)
        {
            Log(type, mEntity, message, obj);
        }

        /// <summary>
        /// Writes the given message to the <see cref="Log"/> using the given subject.
        /// </summary>
        /// <param name="message">The text of the message.</param>
        public void Log(LogType type, INoun subject, string message)
        {
            Log(type, subject, message, null);
        }

        /// <summary>
        /// Writes the given message to the <see cref="Log"/> using the Action's <see cref="Entity"/>
        /// as the subject if needed.
        /// </summary>
        /// <param name="message">The text of the message.</param>
        public void Log(LogType type, string message)
        {
            Log(type, mEntity, message, null);
        }

        public ActionResult Fail(INoun subject, string message, INoun obj)
        {
            Log(LogType.Fail, subject, message, obj);

            return ActionResult.Fail;
        }

        public ActionResult Fail(string message, INoun obj)
        {
            return Fail(mEntity, message, obj);
        }

        public ActionResult Fail(INoun subject, string message)
        {
            return Fail(subject, message, null);
        }

        public ActionResult Fail(string message)
        {
            return Fail(mEntity, message, null);
        }

        #endregion

        protected abstract ActionResult OnProcess();

        private IList<Effect> mEffects;
        private Queue<Action> mActions;
        private Entity mEntity;
        private Game mGame;
        private bool mConsumesEnergy;
    }
}
