using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
