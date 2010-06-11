using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bramble.Core;
using Malison.Core;

using Amaranth.Util;

namespace Amaranth.UI
{
    /// <summary>
    /// A <see cref="Control"/> for entering a line of text.
    /// </summary>
    public class TextBox : Control, IFocusable
    {
        public string Text
        {
            get { return mText; }
            set
            {
                if (mText != value)
                {
                    mText = value;
                    Repaint();
                }
            }
        }

        /// <summary>
        /// Gets and sets the position of this TextBox. The registration point for a TextBox
        /// is the left corner of the edit area (not the title).
        /// </summary>
        public Vec Position
        {
            get { return mPosition; }
            set
            {
                if (mPosition != value)
                {
                    mPosition = value;
                    Repaint();
                }
            }
        }

        /// <summary>
        /// Gets and sets the width of the editable part of the TextBox.
        /// </summary>
        public int Width
        {
            get { return mWidth; }
            set
            {
                if (mWidth != value)
                {
                    mWidth = value;
                    Repaint();
                }
            }
        }

        /// <summary>
        /// Gets and sets the position of the cursor.
        /// </summary>
        public int Cursor
        {
            get { return mCursor; }
            set
            {
                value = value.Clamp(0, Math.Min(mText.Length, mWidth - 1));

                if (mCursor != value)
                {
                    mCursor = value;
                    Repaint();
                }
            }
        }

        public TextBox(string title, string text)
            : base(title)
        {
            mText = text;
            mCursor = 0;
        }

        protected override Rect GetBounds()
        {
            // the width is the width of the title and the box
            int width = Title.Length + 3 + mWidth + 1;

            return new Rect(mPosition.X - Title.Length - 2, mPosition.Y, width, 1);
        }

        protected override void OnPaint(ITerminal terminal)
        {
            base.OnPaint(terminal);

            // draw the title
            terminal[0, 0][TitleColor].Write(Title);

            // draw the separator
            terminal[Title.Length + 1, 0][TermColor.Yellow].Write(Glyph.TriangleRight);

            if (HasFocus)
            {
                // split the text at the cursor
                string paddedText = mText + " ";
                string beforeCursor = (mCursor > 0) ? mText.Substring(0, mCursor) : String.Empty;
                string underCursor = (mCursor < paddedText.Length) ? paddedText.Substring(mCursor, 1) : String.Empty;
                string afterCursor = (mCursor < mText.Length - 1) ? mText.Substring(mCursor + 1) : String.Empty;

                int x = Title.Length + 3;
                terminal[x, 0][TextColor].Write(beforeCursor);
                x += beforeCursor.Length;

                terminal[x, 0][SelectionColor].Write(underCursor);
                x += underCursor.Length;

                terminal[x, 0][TextColor].Write(afterCursor);
            }
            else
            {
                // draw the text
                terminal[Title.Length + 3, 0][SelectionColor].Write(mText);
            }
        }

        private void EnterChar(char c)
        {
            if (mCursor == 0)
            {
                Text = c.ToString();
            }
            else if (mCursor < mText.Length)
            {
                Text = mText.Substring(0, mCursor) + c;
            }
            else
            {
                Text = mText + c;
            }

            Cursor++;
        }

        private void DeleteChar(bool isForward)
        {
            if (isForward && (Cursor < Text.Length))
            {
                // delete: remove character before cursor if not at end, else remove before
                string before = (Cursor > 0) ? Text.Substring(0, Cursor) : String.Empty;
                string after = (Cursor < Text.Length - 1) ? Text.Substring(Cursor + 1) : String.Empty;
                Text = before + after;
            }
            else
            {
                // backspace: remove the character before the cursor
                if (Cursor > 0)
                {
                    Cursor--;

                    string before = (Cursor > 0) ? Text.Substring(0, Cursor) : String.Empty;
                    string after = (Cursor < Text.Length - 1) ? Text.Substring(Cursor + 1) : String.Empty;
                    Text = before + after;
                }
            }
        }

        #region IFocusable Members

        public string Instruction
        {
            get { return "Enter a " + Title; }
        }

        public void GainFocus()
        {
            Repaint();
        }

        public void LoseFocus()
        {
            Repaint();
        }

        #endregion

        #region IInputHandler Members

        public IEnumerable<KeyInstruction> KeyInstructions
        {
            get
            {
                yield return new KeyInstruction("Move cursor", new KeyInfo(Key.Left), new KeyInfo(Key.Right), new KeyInfo(Key.Up), new KeyInfo(Key.Down));

                if (CanChangeFocus)
                {
                    yield return new KeyInstruction("Move focus", new KeyInfo(Key.Tab));
                }
            }
        }

        public bool KeyDown(KeyInfo key)
        {
            bool handled = true;

            switch (key.Key)
            {
                case Key.Up:
                    Cursor = 0;
                    break;
                case Key.Down:
                    Cursor = Text.Length;
                    break;
                case Key.Left:
                    Cursor--;
                    break;
                case Key.Right:
                    Cursor++;
                    break;
                case Key.Backspace:
                    DeleteChar(false);
                    break;

                case Key.Delete:
                    DeleteChar(true);
                    break;

                case Key.Tab:
                    if (key.Shift)
                    {
                        FocusPrevious();
                    }
                    else
                    {
                        FocusNext();
                    }
                    break;

                default:
                    // try to enter it as text
                    char? c = key.TextChar;
                    if (c.HasValue)
                    {
                        EnterChar(c.Value);
                    }
                    else
                    {
                        handled = false;
                    }
                    break;
            }

            return handled;
        }

        public bool KeyUp(KeyInfo key)
        {
            return false;
        }

        #endregion

        private Vec mPosition;
        private int mWidth;
        private int mCursor;
        private string mText;
    }
}
