using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Amaranth.Util;

namespace Amaranth.Terminals
{
    public partial class TerminalControl : UserControl
    {
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ITerminal Terminal
        {
            get { return mTerminal; }
            set
            {
                if (mTerminal != value)
                {
                    if (mTerminal != null)
                    {
                        mTerminal.CharacterChanged -= Terminal_CharacterChanged;
                    }

                    mTerminal = value;

                    if (mTerminal != null)
                    {
                        mTerminal.CharacterChanged += Terminal_CharacterChanged;
                    }
                    Invalidate();
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GlyphSheet GlyphSheet
        {
            get { return mGlyphSheet; }
            set
            {
                if (mGlyphSheet != value)
                {
                    mGlyphSheet = value;
                    Invalidate();
                }
            }
        }

        public TerminalControl()
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.Opaque, true);

            mGlyphSheet = GlyphSheet.Terminal7x10;
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            return new Size(
                (mGlyphSheet.Width * mTerminal.Size.X) + (mPadding * 2),
                (mGlyphSheet.Height * mTerminal.Size.Y) + (mPadding * 2));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Black);

            if (mTerminal != null)
            {
                // only refresh characters in the clip rect
                int left = Math.Max(0, (e.ClipRectangle.Left - mPadding) / mGlyphSheet.Width);
                int top = Math.Max(0, (e.ClipRectangle.Top - mPadding) / mGlyphSheet.Height);
                int right = Math.Min(mTerminal.Size.X, (e.ClipRectangle.Right - mPadding) / mGlyphSheet.Width + 1);
                int bottom = Math.Min(mTerminal.Size.Y, (e.ClipRectangle.Bottom - mPadding) / mGlyphSheet.Height + 1);

                for (int y = top; y < bottom; y++)
                {
                    for (int x = left; x < right; x++)
                    {
                        Character character = mTerminal.Get(x, y);

                        // fill the background if needed
                        if (!character.BackColor.Equals(Color.Black))
                        {
                            int fillLeft = (x * mGlyphSheet.Width) + mPadding;
                            int fillTop = (y * mGlyphSheet.Height) + mPadding;
                            int width = mGlyphSheet.Width;
                            int height = mGlyphSheet.Height;

                            // fill past the padding on the edges
                            if (x == 0)
                            {
                                fillLeft -= mPadding;
                                width += mPadding;
                            }
                            if (x == mTerminal.Size.X - 1)
                            {
                                width += mPadding;
                            }
                            if (y == 0)
                            {
                                fillTop -= mPadding;
                                height += mPadding;
                            }
                            if (y == mTerminal.Size.Y - 1)
                            {
                                height += mPadding;
                            }

                            e.Graphics.FillRectangle(new SolidBrush(character.BackColor),
                                fillLeft, fillTop, width, height);
                        }

                        // draw the glyph
                        mGlyphSheet.Draw(e.Graphics,
                            (x * mGlyphSheet.Width) + mPadding,
                            (y * mGlyphSheet.Height) + mPadding,
                            character);
                    }
                }
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            Cursor.Hide();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            Cursor.Show();
        }

        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (e.KeyCode == Keys.Tab) e.IsInputKey = true;
            if (e.KeyCode == Keys.Up) e.IsInputKey = true;
            if (e.KeyCode == Keys.Down) e.IsInputKey = true;
            if (e.KeyCode == Keys.Left) e.IsInputKey = true;
            if (e.KeyCode == Keys.Right) e.IsInputKey = true;
        }

        private void InvalidateCharacter(Vec pos)
        {
            int width = mGlyphSheet.Width;
            int height = mGlyphSheet.Height;
            int left = (pos.X * width) + mPadding;
            int top = (pos.Y * height) + mPadding;

            // fill past the padding on the edges
            if (pos.X == 0)
            {
                left -= mPadding;
                width += mPadding;
            }
            if (pos.X == mTerminal.Size.X - 1)
            {
                width += mPadding;
            }
            if (pos.Y == 0)
            {
                top -= mPadding;
                height += mPadding;
            }
            if (pos.Y == mTerminal.Size.Y - 1)
            {
                height += mPadding;
            }

            // invalidate the rect under the character
            Invalidate(new Rectangle(left, top, width, height));
        }

        private void Terminal_CharacterChanged(object sender, CharacterEventArgs e)
        {
            InvalidateCharacter(e.Position);
        }

        private int mPadding = 2;

        private GlyphSheet mGlyphSheet;
        private ITerminal mTerminal;
    }
}
