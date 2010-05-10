using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;
using Amaranth.Terminals;

namespace Amaranth.UI
{
    //### bob: refactor to inherit PromptBar
    public class PromptNumberBar : Control
    {
        public PromptNumberBar()
            : base()
        {
        }

        public int Read(int min, int start, int max)
        {
            int value = start;

            mText = value.ToString();
            Repaint();

            while (true)
            {
                KeyInfo key = Screen.UI.ReadKey();

                if (key.Down)
                {
                    switch (key.Key)
                    {
                        case Key.Escape:
                            return 0;

                        case Key.Enter:
                            return value;

                        case Key.Digit0:
                        case Key.Digit1:
                        case Key.Digit2:
                        case Key.Digit3:
                        case Key.Digit4:
                        case Key.Digit5:
                        case Key.Digit6:
                        case Key.Digit7:
                        case Key.Digit8:
                        case Key.Digit9:
                            if (mReplacing)
                            {
                                value = (key.Character.Value - '0');
                                mReplacing = false;
                            }
                            else
                            {
                                value = value * 10 + (key.Character.Value - '0');
                            }
                            break;

                        case Key.Backspace:
                        case Key.Delete:
                            if (value >= 10)
                            {
                                value /= 10;
                            }
                            else
                            {
                                // deleted entire number
                                mReplacing = true;
                                value = 0;
                            }
                            break;

                        case Key.Up:
                            value++;
                            break;

                        case Key.Down:
                            value--;
                            break;

                        case Key.Left:
                            value = min;
                            break;

                        case Key.Right:
                            value = max;
                            break;
                    }
                }

                // keep in bounds
                value = Math2.Clamp(min, value, max);

                mText = value.ToString();
                Repaint();
            }
        }

        protected override Rect GetBounds()
        {
            return new Rect(0, Parent.Bounds.Height - 1, Parent.Bounds.Width, 1);
        }

        protected override void OnPaint(ITerminal terminal)
        {
            terminal = terminal.CreateWindow(new Rect(terminal.Size));

            terminal.State.ForeColor = TerminalColors.White;
            terminal.State.BackColor = TerminalColors.DarkGray;

            terminal.Clear();

            // write the instruction
            terminal[0, 0][TerminalColors.LightGray].Write("Enter a number:");
            if (mReplacing)
            {
                terminal[16, 0][TerminalColors.Black, TerminalColors.Yellow].Write(mText);
            }
            else
            {
                terminal[16, 0][TerminalColors.Yellow].Write(mText);
                terminal[16 + mText.Length, 0][TerminalColors.Black, TerminalColors.Yellow].Write(" ");
            }

            Stack<KeyInstruction> instructions = new Stack<KeyInstruction>();

            instructions.Push(new KeyInstruction("Increment", new KeyInfo(Key.Up), new KeyInfo(Key.Down)));
            instructions.Push(new KeyInstruction("Min/Max", new KeyInfo(Key.Left), new KeyInfo(Key.Right)));
            instructions.Push(new KeyInstruction("Enter Digit", new KeyInfo(Key.Digit0), new KeyInfo(Key.Dash), new KeyInfo(Key.Digit9)));
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
                        terminal[x, 0][TerminalColors.Yellow].Write(glyphs[i]);
                    }
                }

                // put some space between each instruction
                x -= 2;
            }
        }

        private string mText = String.Empty;
        private bool mReplacing = true;
    }
}
