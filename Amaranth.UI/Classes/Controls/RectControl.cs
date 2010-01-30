using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.UI
{
    public abstract class RectControl : Control
    {
        /// <summary>
        /// Gets and sets the position of this PositionControl.
        /// </summary>
        public new Rect Bounds
        {
            get { return base.Bounds; }
            set
            {
                if (mBounds != value)
                {
                    mBounds = value;
                    Repaint();
                }
            }
        }

        protected RectControl(string title, Rect bounds)
            : base(title)
        {
            mBounds = bounds;
        }

        protected RectControl(string title)
            : this(title, Rect.Empty)
        {
        }

        protected RectControl(Rect bounds)
            : this(String.Empty, bounds)
        {
        }

        protected RectControl()
            : base()
        {
        }

        protected override Rect GetBounds()
        {
            return mBounds;
        }

        private Rect mBounds;
    }
}
