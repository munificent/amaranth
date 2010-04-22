using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    public class StoreType : ContentBase
    {
        public override string Name { get { return mName; } }

        public int Depth { get { return mDepth; } }
        public IDrop<Item> Drop { get { return mDrop; } }

        public StoreType(Content content, string name, int depth, IDrop<Item> drop)
            : base(content)
        {
            mName = name;
            mDepth = depth;
            mDrop = drop;
        }

        private string mName;
        private int mDepth;
        private IDrop<Item> mDrop;
    }
}
