using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Terminals;

namespace Amaranth.UI
{
    public class KeyInstruction
    {
        public string Instruction;
        public KeyInfo[] Keys;

        public KeyInstruction(string instruction, params KeyInfo[] keys)
        {
            Instruction = instruction;
            Keys = keys;
        }
    }
}
