using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bramble.Core;
using Malison.Core;

using Amaranth.Util;

namespace Amaranth.UI
{
    public abstract class PromptBar<TValue> : Control
    {
        public PromptBar(string title)
            : base(title)
        {
        }

        public TValue Read(TValue defaultValue, TValue cancelValue)
        {
            mValue = defaultValue;
            Repaint();

            while (true)
            {
                KeyInfo key = Screen.UI.ReadKey();

                if (key.Down)
                {
                    switch (key.Key)
                    {
                        case Key.Escape: return cancelValue;

                        case Key.Enter: return mValue;

                        default:
                            if (OnKeyDown(key, ref mValue))
                            {
                                return mValue;
                            }
                            break;
                    }
                }

                Repaint();
            }
        }

        protected abstract IEnumerable<KeyInstruction> KeyInstructions { get; }

        protected abstract bool OnKeyDown(KeyInfo key, ref TValue value);

        protected virtual string GetValueString(TValue value)
        {
            // default to just the string representation
            return value.ToString();
        }

        protected override Rect GetBounds()
        {
            return new Rect(0, Parent.Bounds.Height - 1, Parent.Bounds.Width, 1);
        }

        protected override void OnPaint(ITerminal terminal)
        {
            terminal = terminal[TermColor.White, TermColor.DarkGray].CreateWindow();

            terminal.Clear();

            // write the instruction
            terminal[0, 0][TermColor.LightGray].Write(Title);
            terminal[Title.Length + 1, 0][TermColor.Yellow].Write(GetValueString(mValue));

            Stack<KeyInstruction> instructions = new Stack<KeyInstruction>();

            foreach (KeyInstruction instruction in KeyInstructions)
            {
                instructions.Push(instruction);
            }
            instructions.Push(new KeyInstruction("Cancel", new KeyInfo(Key.Escape)));
            instructions.Push(new KeyInstruction("Accept", new KeyInfo(Key.Enter)));

            // write the keys from right to left
            int x = terminal.Size.X;

            while (instructions.Count > 0)
            {
                KeyInstruction instruction = instructions.Pop();

                // write the text
                x -= instruction.Instruction.Length;
                terminal[x, 0].Write(instruction.Instruction);

                // write the glyphs
                x--;
                for (int j = instruction.Keys.Length - 1; j >= 0; j--)
                {
                    Glyph[] glyphs = instruction.Keys[j].DisplayGlyphs;

                    for (int i = glyphs.Length - 1; i >= 0; i--)
                    {
                        x--;
                        terminal[x, 0][TermColor.Yellow].Write(glyphs[i]);
                    }
                }

                // put some space between each instruction
                x -= 2;
            }
        }

        private TValue mValue;
    }
}
