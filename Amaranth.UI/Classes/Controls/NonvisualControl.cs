using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bramble.Core;

using Amaranth.Util;

namespace Amaranth.UI
{
    public abstract class NonvisualControl : Control
    {
        protected NonvisualControl(string title)
            : base(title)
        {
        }

        protected NonvisualControl()
            : base()
        {
        }

        protected override Rect GetBounds()
        {
            return Rect.Empty;
        }
    }
}
