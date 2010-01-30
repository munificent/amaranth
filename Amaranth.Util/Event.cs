using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Amaranth.Util
{
    /*
    [Serializable]
    public class Event<SenderT, ArgsT> where ArgsT : EventArgs
    {
        public delegate void Handler(SenderT sender, ArgsT args);

        public void Add(Handler handler)
        {
            if (mHandlers == null)
            {
                mHandlers = new List<Handler>();
            }

            mHandlers.Add(handler);
        }

        public void Remove(Handler handler)
        {
            if (mHandlers == null) throw new InvalidOperationException("Cannot remove a handler because it does not contain any.");
            if (!mHandlers.Contains(handler)) throw new InvalidOperationException("The given handler is not currently contained in this Event.");

            mHandlers.Remove(handler);
        }

        public void Raise(SenderT sender, ArgsT args)
        {
            if (mHandlers != null)
            {
                mHandlers.ForEach((handler) => handler(sender, args));
            }
        }

        #region Serialization

        [OnSerializing()]
        protected void OnSerializing(StreamingContext context)
        {
            if (mHandlers != null)
            {
                // copy over the handlers to serialize
                mSerializedHandlers = new List<Handler>();

                // only serialize handlers that are themselves serializable
                foreach (Handler handler in mHandlers)
                {
                    object target = handler.Target;

                    if (target == null)
                    {
                        // always serialize delegates to static methods
                        mSerializedHandlers.Add(handler);
                    }
                    else
                    {
                        object[] attributes = target.GetType().GetCustomAttributes(typeof(SerializableAttribute), true);

                        // if it's marked [Serializable] then serialize it
                        if (attributes.Length > 0)
                        {
                            mSerializedHandlers.Add(handler);
                        }
                    }
                }
            }
        }

        [OnSerialized()]
        protected void OnSerialized(StreamingContext context)
        {
            // clear the list now that it's done
            mSerializedHandlers = null;
        }

        [OnDeserialized()]
        protected void OnDeserialized(StreamingContext context)
        {
            // copy over the serialized handlers
            if (mSerializedHandlers != null)
            {
                mHandlers = new List<Handler>();
                mHandlers.AddRange(mSerializedHandlers);

                // clear the list now that it's done
                mSerializedHandlers = null;
            }
        }

        #endregion

        [NonSerialized]
        private List<Handler> mHandlers;

        private List<Handler> mSerializedHandlers;
    }*/
}
