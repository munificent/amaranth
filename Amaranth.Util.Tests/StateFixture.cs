using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Amaranth.Util;

namespace Amaranth.Util.Tests
{
    [TestFixture]
    public class StateFixture
    {
        //### bob: doesn't compile because ReceiveMessage is in explicit interface impl. can make this
        // work if i do the whole friend internal assembly thing.
        /*
        [Test]
        public void TestReceiveMessage()
        {
            bool received = false;

            ReceiverState state = new ReceiverState(() => received = true);

            Assert.IsFalse(received);

            state.ReceiveMessage("sender", 123);

            Assert.IsTrue(received);
        }

        [Test]
        public void TestReceiveMessageIgnored()
        {
            bool received = false;

            ReceiverState state = new ReceiverState(() => received = true);

            Assert.IsFalse(received);

            // does not receive a message of float type
            state.ReceiveMessage("sender", 123.0f);

            Assert.IsFalse(received);
        }
        */

        private class ReceiverState : State, IMessageReceiver<string, int>
        {
            public ReceiverState(Action receiveAction)
            {
                mReceiveAction = receiveAction;
            }

            private Action mReceiveAction;

            #region IMessageReceiver<string,int> Members

            void IMessageReceiver<string, int>.Receive(string sender, int message)
            {
                mReceiveAction();
            }

            #endregion
        }
    }
}
