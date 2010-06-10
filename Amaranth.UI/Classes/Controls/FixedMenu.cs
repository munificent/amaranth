using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bramble.Core;

using Amaranth.Util;
using Amaranth.Terminals;

namespace Amaranth.UI
{
    public class FixedMenu : Menu
    {
        public MenuItem SelectedItem { get { return Items[mSelected]; } }

        public FixedMenu(string title, string[] options)
            : base(title, options)
        {
        }

        public FixedMenu(string title)
            : base(title)
        {
        }

        public FixedMenu()
            : base()
        {
        }

        protected override Rect GetBounds()
        {
            int leftOverhang = 0;

            // add in the title if there is one
            if (!String.IsNullOrEmpty(Title))
            {
                leftOverhang += Title.Length + 3;
            }

            // add in room for the shortcuts
            leftOverhang += 2;

            // as wide as the widest option
            int width = Items.Max((option) => option.Text.Length);

            // the height is the number of options
            int height = Items.Count;

            return new Rect(Position.X - leftOverhang, Position.Y, leftOverhang + width, height);
        }

        protected override void OnPaint(ITerminal terminal)
        {
            base.OnPaint(terminal);

            int x = 0;

            // draw the title
            if (!String.IsNullOrEmpty(Title))
            {
                terminal[0, 0][TitleColor].Write(Title);
                terminal[Title.Length + 1, 0][TerminalColors.Yellow].Write(Glyph.TriangleRight);

                x += Title.Length + 3;
            }

            // draw the options
            for (int i = 0; i < Items.Count; i++)
            {
                ColorPair shortcutColor = TitleColor;
                ColorPair color = TextColor;

                if (i == mSelected)
                {
                    shortcutColor = SelectionColor;
                    color = SelectionColor;
                }

                terminal[x, i][shortcutColor].Write(Items[i].Shortcut + " ");
                terminal[x + 2, i][color].Write(Items[i].Text);
            }
        }

        protected override void SelectItem(int item, bool down)
        {
            if (down)
            {
                mSelected = item;
                Repaint();
            }
            else
            {
                mSelected = -1;
                Repaint();

                base.SelectItem(item, down);
            }
        }

        #region IInputHandler Members

        public override IEnumerable<KeyInstruction> KeyInstructions
        {
            get
            {
                foreach (MenuItem item in Items)
                {
                    yield return new KeyInstruction(item.Instruction, KeyInfo.FromChar(Char.ToUpper(item.Shortcut)));
                }

                if (CanChangeFocus)
                {
                    yield return new KeyInstruction("Move focus", new KeyInfo(Key.Tab));
                }
            }
        }

        #endregion

        private int mSelected = -1;
    }
}
