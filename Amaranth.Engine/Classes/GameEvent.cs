using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// Event mechanism for communicating game state changes to the UI. Similar to a C# event except
    /// that it doesn't serialize to keep the serializable game separate from the UI.
    /// </summary>
    /// <typeparam name="TSender">Event sender's type.</typeparam>
    /// <typeparam name="TArgs">EventArgs type for the event.</typeparam>
    [Serializable]
    public class GameEvent<TSender, TArgs> : IEvent<TSender, TArgs> where TArgs : EventArgs
    {
        public void Add(Action<TSender, TArgs> handler)
        {
            if (mHandlers == null)
            {
                mHandlers = new List<Action<TSender, TArgs>>();
            }

            mHandlers.Add(handler);
        }

        public void Remove(Action<TSender, TArgs> handler)
        {
            if (mHandlers == null) throw new InvalidOperationException("Cannot remove a handler because it does not contain any.");

            mHandlers.Remove(handler);
        }

        internal void Raise(TSender sender, TArgs args)
        {
            if (mHandlers != null)
            {
                mHandlers.ForEach((action) => action(sender, args));
            }
        }

        [NonSerialized]
        private List<Action<TSender, TArgs>> mHandlers;
    }
}
