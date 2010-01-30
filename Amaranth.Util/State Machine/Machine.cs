using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Util
{
    /// <summary>
    /// A finite state machine with states of the given type.
    /// </summary>
    public class Machine<TState> : IStateToMachine where TState : State
    {
        /// <summary>
        /// Gets the current State of the Machine, or null if it has none.
        /// </summary>
        public TState State
        {
            get
            {
                // no state
                if (mStates.Count == 0) return null;

                // return the last one
                return mStates[mStates.Count - 1];
            }
        }

        public Machine(NotNull<TState> startingState)
        {
            Push(startingState);
        }

        public void ReceiveMessage<TSender, TMessage>(TSender sender, TMessage message)
        {
            if (CurrentMachineState != null)
            {
                CurrentMachineState.ReceiveMessage<TSender, TMessage>(sender, message);
            }
        }

        private void Push(NotNull<TState> state)
        {
            // stop the current state (if any)
            if (CurrentMachineState != null)
            {
                CurrentMachineState.Stop();
            }

            // add to the stack
            mStates.Add(state);

            // initialize it
            IMachineToState machineState = state.Value;
            machineState.Init(this);
            machineState.Start();
        }

        private void Pop(NotNull<TState> state)
        {
            if (!mStates.Contains(state)) throw new InvalidOperationException("Cannot pop a state that is not on the machine's stack.");

            // shut it down
            IMachineToState machineState = state.Value;
            machineState.End();

            // remove it
            mStates.Remove(state);

            // start up the new current state (if any)
            if (CurrentMachineState != null)
            {
                CurrentMachineState.Start();
            }
        }

        private IMachineToState CurrentMachineState
        {
            get
            {
                // no state
                if (mStates.Count == 0) return null;

                // return the last one
                return mStates[mStates.Count - 1];
            }
        }

        #region IStateToMachine<TState> Members

        void IStateToMachine.Push(NotNull<State> state)
        {
            //### bob: cast here is lame
            Push((TState)state);
        }

        void IStateToMachine.Pop(NotNull<State> state)
        {
            //### bob: cast here is lame
            Pop((TState)state);
        }

        #endregion

        private readonly List<TState> mStates = new List<TState>();
    }

    /// <summary>
    /// A finite state machine where the states are known only to be of State type.
    /// </summary>
    public class Machine : Machine<State>
    {
        public Machine(NotNull<State> startingState) : base(startingState) { }
    }
}
