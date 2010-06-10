using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bramble.Core;

using Amaranth.Util;

namespace Amaranth.UI
{
    public abstract class PositionControl : Control
    {
        /// <summary>
        /// Gets and sets the position of this PositionControl.
        /// </summary>
        public Vec Position
        {
            get { return mPosition; }
            set
            {
                if (mPosition != value)
                {
                    mPosition = value;
                    Repaint();
                }
            }
        }

        protected PositionControl(Vec position, string title)
            : base(title)
        {
            mPosition = position;
        }

        protected PositionControl(string title)
            : this(Vec.Zero, title)
        {
        }

        protected PositionControl()
            : this(String.Empty)
        {
        }

        private Vec mPosition;
    }
}
