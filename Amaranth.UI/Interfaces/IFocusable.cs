using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.UI
{
    public interface IFocusable : IInputHandler
    {
        /// <summary>
        /// Gets the main instruction text to show the user when this <see cref="Control"/> has focus.
        /// </summary>
        string Instruction { get; }

        void GainFocus();
        void LoseFocus();
    }
}
