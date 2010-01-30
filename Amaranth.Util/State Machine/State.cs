using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Util
{
    /// <summary>
    /// A single state in a <see cref="Machine"/>.
    /// </summary>
    public abstract class State : IMachineToState
    {
        protected void Push(NotNull<State> state)
        {
            mMachine.Push(state);
        }

        protected void Pop()
        {
            mMachine.Pop(this);
        }

        protected void GoTo(NotNull<State> state)
        {
            mMachine.Pop(this);
            mMachine.Push(state);
        }

        /// <summary>
        /// Called when the State has been added to a Machine, before it has been started.
        /// </summary>
        protected virtual void Init() { }

        /// <summary>
        /// Called when a State has become the current state for a Machine (either for the
        /// first time, or as a result of another state no longer being current).
        /// </summary>
        protected virtual void Start() { }

        /// <summary>
        /// Called when a State is no longer the current state for a Machine (either because
        /// it is about to be removed, or because another state is about to become current).
        /// </summary>
        protected virtual void Stop() { }

        /// <summary>
        /// Called when a State is about to be removed from the Machine.
        /// </summary>
        protected virtual void End() { }

        #region IMachineToState Members

        void IMachineToState.Init(IStateToMachine machine)
        {
            mMachine = machine;

            Init();
        }

        void IMachineToState.Start()
        {
            Start();
        }

        void IMachineToState.Stop()
        {
            Stop();
        }

        void IMachineToState.End()
        {
            End();
        }

        void IMachineToState.ReceiveMessage<TSender, TMessage>(TSender sender, TMessage message)
        {
            // see if this can receive it
            IMessageReceiver<TSender, TMessage> receiver = this as IMessageReceiver<TSender, TMessage>;

            if (receiver != null)
            {
                // we do receive the message, so handle it
                receiver.Receive(sender, message);
            }
        }

        #endregion

        private IStateToMachine mMachine;
    }
}
