using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Engine;
using Amaranth.Util;
using Amaranth.Terminals;
using Amaranth.UI;

namespace Amaranth.TermApp
{
    public class PromptDirectionBar : PromptBar<Direction>
    {
        public PromptDirectionBar(string title)
            : base(title)
        {
        }

        protected override IEnumerable<KeyInstruction> KeyInstructions
        {
            get
            {
                yield return new KeyInstruction("Direction",
                    new KeyInfo(Key.I), new KeyInfo(Key.O), new KeyInfo(Key.P),
                    new KeyInfo(Key.Semicolon), new KeyInfo(Key.Slash), new KeyInfo(Key.Period),
                    new KeyInfo(Key.Comma), new KeyInfo(Key.K));
            }
        }

        protected override bool OnKeyDown(KeyInfo key, ref Direction value)
        {
            switch (key.Key)
            {
                case Key.I: value = Direction.NW; return true;
                case Key.O: value = Direction.N; return true;
                case Key.P: value = Direction.NE; return true;
                case Key.Semicolon: value = Direction.E; return true;
                case Key.Slash: value = Direction.SE; return true;
                case Key.Period: value = Direction.S; return true;
                case Key.Comma: value = Direction.SW; return true;
                case Key.K: value = Direction.W; return true;
                default: return false;
            }
        }
    }
}
