using System;
using System.Collections.Generic;
using System.Text;

namespace Amaranth.Util
{
    public class ValueChangeEventArgs<ValueT> : EventArgs
    {
        public ValueT New { get { return mNew; } }
        public ValueT Old { get { return mOld; } }

        public ValueChangeEventArgs(ValueT oldValue, ValueT newValue)
        {
            mOld = oldValue;
            mNew = newValue;
        }

        private ValueT mOld;
        private ValueT mNew;
    }
}
