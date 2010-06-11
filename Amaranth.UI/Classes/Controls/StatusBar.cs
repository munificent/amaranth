using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bramble.Core;
using Malison.Core;

using Amaranth.Util;

namespace Amaranth.UI
{
    public class StatusBar : Control
    {
        public StatusBar()
            : base()
        {
        }

        protected override void OnAttach(Control parent)
        {
            base.OnAttach(parent);

            Screen.FocusChanged += Screen_FocusChanged;
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
            string instruction = Screen.UI.Title;
            IFocusable focus = Screen.FocusControl;
            if (focus != null)
            {
                instruction = focus.Instruction;
            }
            else if (!String.IsNullOrEmpty(Screen.UI.CurrentScreen.Title))
            {
                instruction = Screen.UI.CurrentScreen.Title;
            }
            terminal[0, 0][TermColor.LightGray].Write(instruction);

            Stack<KeyInstruction> instructions = new Stack<KeyInstruction>();

            // get the focused control key instructions
            IInputHandler input = Screen.FocusControl as IInputHandler;
            if (input != null)
            {
                foreach (KeyInstruction keyInstruction in input.KeyInstructions)
                {
                    instructions.Push(keyInstruction);
                }
            }
            else
            {
                foreach (Control control in Screen.Controls)
                {
                    input = control as IInputHandler;

                    if (input != null)
                    {
                        foreach (KeyInstruction keyInstruction in input.KeyInstructions)
                        {
                            instructions.Push(keyInstruction);
                        }
                    }
                }
            }

            // get the screen control keys
            IInputHandler screen = Screen as IInputHandler;
            if (screen != null)
            {
                foreach (KeyInstruction keyInstruction in screen.KeyInstructions)
                {
                    instructions.Push(keyInstruction);
                }
            }

            // write the keys from right to left
            int x = terminal.Size.X;

            while (instructions.Count > 0)
            {
                KeyInstruction keyInstruction = instructions.Pop();

                // write the text
                x -= keyInstruction.Instruction.Length;
                terminal[x, 0].Write(keyInstruction.Instruction);

                // write the glyphs
                x--;
                for (int j = keyInstruction.Keys.Length - 1; j >= 0; j--)
                {
                    Glyph[] glyphs = keyInstruction.Keys[j].DisplayGlyphs;

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

        private void Screen_FocusChanged(object sender, EventArgs e)
        {
            Repaint();
        }
    }
}
