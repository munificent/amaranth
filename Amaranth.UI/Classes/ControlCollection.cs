using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.UI
{
    public class ControlCollection : CollectibleCollection<ControlCollection, Control>
    {
        public Control Parent { get { return mParent; } }

        public ControlCollection(NotNull<Control> parent)
        {
            mParent = parent;
        }

        private readonly Control mParent;
    }
}
