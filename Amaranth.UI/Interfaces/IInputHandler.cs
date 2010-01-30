using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.UI
{
    public interface IInputHandler
    {
        /// <summary>
        /// Gets the collection of instructions for each valid key press when this <see cref="Control"/>
        /// has focus.
        /// </summary>
        IEnumerable<KeyInstruction> KeyInstructions { get; }

        bool KeyDown(KeyInfo key);
        bool KeyUp(KeyInfo key);
    }
}
