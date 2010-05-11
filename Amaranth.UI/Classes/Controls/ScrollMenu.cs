using System;
using System.Collections.Generic;
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
    public class ScrollMenu : Menu
    {
        public MenuItem SelectedItem { get { return Items[mSelected]; } }

        /// <summary>
        /// Gets and sets the index of the selected item.
        /// </summary>
        public int Selected
        {
            get { return mSelected; }
            set
            {
                value = value.Clamp(0, Items.Count - 1);

                if (mSelected != value)
                {
                    mSelected = value;
                    Repaint();

                    SelectItem(mSelected, true);
                }
            }
        }

        public ScrollMenu(string title, string[] options)
            : base(title, options)
        {
            mSelected = 0;
        }

        public ScrollMenu(string title)
            : base(title)
        {
            mSelected = 0;
        }

        protected override Rect GetBounds()
        {
            // the width is the width of the title and the widest option
            int width = 0;

            if (Items.Count > 0)
            {
                width = Items.Max((option) => option.Text.Length);
            }

            width += Title.Length + 3;

            // the height is twice the number of options (so they can scroll above and below)
            int height = Items.Count * 2 - 1;

            return new Rect(Position.X, Position.Y - Items.Count + 1, width, height);
        }

        protected override void OnPaint(ITerminal terminal)
        {
            base.OnPaint(terminal);

            int titleY = Items.Count - 1;
            terminal[0, titleY][TitleColor].Write(Title);

            terminal[Title.Length + 1, titleY][TerminalColors.Yellow].Write(Glyph.TriangleRight);

            int x = Title.Length + 3;

            for (int i = 0; i < Items.Count; i++)
            {
                ColorPair color = TextColor;

                if (i == Selected)
                {
                    color = SelectionColor;
                }

                int y = Items.Count - 1 + i - Selected;
                terminal[x, y][color].Write(Items[i].Text);
            }
        }

        protected override void SelectItem(int item, bool down)
        {
            if (down)
            {
                Selected = item;

                base.SelectItem(item, down);
            }
        }

        #region IInputHandler Members

        public override IEnumerable<KeyInstruction> KeyInstructions
        {
            get
            {
                yield return new KeyInstruction("Select item", new KeyInfo(Key.Up), new KeyInfo(Key.Down));

                if (CanChangeFocus)
                {
                    yield return new KeyInstruction("Move focus", new KeyInfo(Key.Tab));
                }
            }
        }

        public override bool KeyDown(KeyInfo key)
        {
            bool handled = false;

            if (!base.KeyDown(key))
            {
                switch (key.Key)
                {
                    case Key.Up:
                        Selected--;
                        handled = true;
                        break;
                    case Key.Down:
                        Selected++;
                        handled = true;
                        break;
                }
            }

            return handled;
        }

        #endregion

        private int mSelected;
    }
}
