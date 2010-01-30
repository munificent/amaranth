using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;
using Amaranth.Terminals;

namespace Amaranth.UI
{
    //### bob: refactor to inherit PromptBar
    public class PromptYesNoBar : Control
    {
        public PromptYesNoBar(string title)
            : base(title)
        {
        }

        public bool Read(bool defaultValue, bool cancelValue)
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
                        case Key.Escape:
                            return cancelValue;

                        case Key.Enter:
                            return mValue;

                        case Key.Y:
                            return true;

                        case Key.N:
                            return false;
                    }
                }

                Repaint();
            }
        }

        protected override Rect GetBounds()
        {
            return new Rect(0, Parent.Bounds.Height - 1, Parent.Bounds.Width, 1);
        }

        protected override void OnPaint(ITerminal terminal)
        {
            using (new DisposableTerminalState(terminal))
            {
                terminal.State.ForeColor = TerminalColors.White;
                terminal.State.BackColor = TerminalColors.DarkGray;

                terminal.Clear();

                // write the instruction
                terminal[0, 0][TerminalColors.LightGray].Write(Title);
                if (mValue)
                {
                    terminal[Title.Length + 1, 0][TerminalColors.Yellow].Write("Yes");
                }
                else
                {
                    terminal[Title.Length + 1, 0][TerminalColors.Yellow].Write("No");
                }

                Stack<KeyInstruction> instructions = new Stack<KeyInstruction>();

                instructions.Push(new KeyInstruction("Yes", new KeyInfo(Key.Y, true)));
                instructions.Push(new KeyInstruction("No", new KeyInfo(Key.N, true)));
                instructions.Push(new KeyInstruction("Cancel", new KeyInfo(Key.Escape)));
                instructions.Push(new KeyInstruction("Accept", new KeyInfo(Key.Enter)));

                // write the keys from right to left
                int x = terminal.Width;

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
                            terminal[x, 0][TerminalColors.Yellow].Write(glyphs[i]);
                        }
                    }

                    // put some space between each instruction
                    x -= 2;
                }
            }
        }

        private bool mValue;
    }
}
