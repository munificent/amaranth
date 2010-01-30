using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.UI
{
    public class MenuItem
    {
        public string Text;
        public char Shortcut;
        public string Instruction;
        public Action Action;

        public MenuItem(string text, char shortcut, Action action, string instruction)
        {
            if (String.IsNullOrEmpty(text)) throw new ArgumentException("The menu text must not be null or empty.", "text");

            Text = text;
            Shortcut = shortcut;
            Instruction = instruction;
            Action = action;
        }

        public MenuItem(string text, Action action, string instruction)
            : this(text, text[0], action, instruction)
        {
        }

        public MenuItem(string text, char shortcut)
            : this(text, shortcut, null, text)
        {
        }

        public MenuItem(string text, Action action)
            : this(text, text[0], action, text)
        {
        }

        public MenuItem(string text)
            : this(text, text[0], null, text)
        {
        }
    }
}
