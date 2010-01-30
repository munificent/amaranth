using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;

using Amaranth.Util;
using Amaranth.Terminals;

namespace Amaranth.UI
{
    /// <summary>
    /// A <see cref="Control"/> for choosing one from a list of options.
    /// </summary>
    public abstract class Menu : PositionControl, IFocusable
    {
        public event EventHandler ItemSelected;

        public List<MenuItem> Items { get { return mItems; } }

        public Menu(string title, string[] items)
            : base(title)
        {
            mItems = new List<MenuItem>();

            foreach (string item in items)
            {
                mItems.Add(new MenuItem(item));
            }
        }

        public Menu(string title)
            : this(title, new string[0])
        {
        }

        public Menu()
            : this(String.Empty, new string[0])
        {
        }

        private bool CheckForItemSelection(KeyInfo key)
        {
            // see if the first letter of a menu item was typed
            char? textChar = key.TextChar;
            if (textChar.HasValue)
            {
                char letter = Char.ToLower(textChar.Value);

                for (int i = 0; i < mItems.Count; i++)
                {
                    if (Char.ToLower(mItems[i].Shortcut) == letter)
                    {
                        SelectItem(i, key.Down);
                        return true;
                    }
                }
            }

            return false;
        }

        #region IFocusable Members

        public string Instruction
        {
            get
            {
                if (String.IsNullOrEmpty(Title))
                {
                    return "Choose an Option";
                }
                else
                {
                    return "Choose a " + Title;
                }
            }
        }

        public abstract IEnumerable<KeyInstruction> KeyInstructions { get; }

        public void GainFocus()
        {
            Repaint();
        }

        public void LoseFocus()
        {
            Repaint();
        }

        protected virtual void SelectItem(int item, bool down)
        {
            if (Items[item].Action != null)
            {
                Items[item].Action();
            }

            if (ItemSelected != null) ItemSelected(this, EventArgs.Empty);
        }

        #endregion

        #region IInputHandler Members

        public virtual bool KeyDown(KeyInfo key)
        {
            bool handled = true;

            if (key.Key == Key.Tab)
            {
                if (key.Shift)
                {
                    FocusPrevious();
                }
                else
                {
                    FocusNext();
                }
            }
            else
            {
                handled = CheckForItemSelection(key);
            }

            return handled;
        }

        public bool KeyUp(KeyInfo key)
        {
            return CheckForItemSelection(key);
        }

        #endregion

        private List<MenuItem> mItems;
    }
}
