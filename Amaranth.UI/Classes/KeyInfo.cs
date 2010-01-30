using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Terminals;

namespace Amaranth.UI
{
    /*
     * different key use cases:
     * 
     * - one non-modifier button on keyboard
     *      Key enum
     * - the physical key press (F + Shift)
     *      KeyInfo
     * - the logical key (capital F) (needed?)
     * - the display string ("F", or a Glyph for arrows, etc.)
     *      KeyInfo.DisplayText, KeyInfo.DisplayGlyphs
     * - the character value entered into a text box
     *      KeyInfo.TextChar
     * - the game input (north)
     * 
     * 
     * 
     */
    public enum Key
    {
        Space,

        A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z,

        Digit0,
        Digit1,
        Digit2,
        Digit3,
        Digit4,
        Digit5,
        Digit6,
        Digit7,
        Digit8,
        Digit9,

        Accent,
        Dash,
        Equals,
        OpenBracket,
        CloseBracket,
        Backslash,
        Semicolon,
        Apostrophe,
        Comma,
        Period,
        Slash,

        Enter,
        Tab,
        Backspace,
        Delete,
        Escape,

        Up,
        Down,
        Left,
        Right,

        F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12, F13, F14, F15, F16, F17, F18, F19, F20, F21, F22, F23, F24
    }

    public struct KeyInfo : IEquatable<KeyInfo>
    {
        public static KeyInfo FromChar(char c)
        {
            if (sCharMap.ContainsKey(c))
            {
                return sCharMap[c];
            }

            throw new InvalidOperationException("Could not find a KeyInfo for the character '" + c + "'.");
        }

        public Key Key;
        public bool Shift;
        public bool Control;
        public bool Down;

        public Glyph[] DisplayGlyphs
        {
            get
            {
                if ((Key >= Key.A) && (Key <= Key.Z))
                {
                    if (Shift)
                    {
                        return new Glyph[] { Glyph.AUpper + (Key - Key.A) };
                    }
                    else
                    {
                        return new Glyph[] { Glyph.ALower + (Key - Key.A) };
                    }
                }
                else if ((Key >= Key.Digit0) && (Key <= Key.Digit9))
                {
                    if (Shift)
                    {
                        switch (Key)
                        {
                            case Key.Digit0: return new Glyph[] { Glyph.CloseParenthesis };
                            case Key.Digit1: return new Glyph[] { Glyph.ExclamationMark };
                            case Key.Digit2: return new Glyph[] { Glyph.At };
                            case Key.Digit3: return new Glyph[] { Glyph.Pound };
                            case Key.Digit4: return new Glyph[] { Glyph.DollarSign };
                            case Key.Digit5: return new Glyph[] { Glyph.Percent };
                            case Key.Digit6: return new Glyph[] { Glyph.Caret };
                            case Key.Digit7: return new Glyph[] { Glyph.Ampersand };
                            case Key.Digit8: return new Glyph[] { Glyph.Asterisk };
                            case Key.Digit9: return new Glyph[] { Glyph.OpenParenthesis };
                        }
                    }
                    else
                    {
                        return new Glyph[] { Glyph.Digit0 + (Key - Key.Digit0) };
                    }
                }
                else
                {
                    switch (Key)
                    {
                        case Key.Enter: return new Glyph[] { Glyph.EUpper, Glyph.NLower, Glyph.TLower, Glyph.ELower, Glyph.RLower };
                        case Key.Tab: return new Glyph[] { Glyph.TUpper, Glyph.ALower, Glyph.BLower };
                        case Key.Backspace: return new Glyph[] { Glyph.BUpper, Glyph.ALower, Glyph.CLower, Glyph.KLower };
                        case Key.Delete: return new Glyph[] { Glyph.DUpper, Glyph.ELower, Glyph.LLower };
                        case Key.Escape: return new Glyph[] { Glyph.EUpper, Glyph.SLower, Glyph.CLower };
                        case Key.Up: return new Glyph[] { Glyph.ArrowUp };
                        case Key.Down: return new Glyph[] { Glyph.ArrowDown };
                        case Key.Left: return new Glyph[] { Glyph.ArrowLeft };
                        case Key.Right: return new Glyph[] { Glyph.ArrowRight };
                        case Key.F1: return new Glyph[] { Glyph.FUpper, Glyph.Digit1 };
                        case Key.F2: return new Glyph[] { Glyph.FUpper, Glyph.Digit2 };
                        case Key.F3: return new Glyph[] { Glyph.FUpper, Glyph.Digit3 };
                        case Key.F4: return new Glyph[] { Glyph.FUpper, Glyph.Digit4 };
                        case Key.F5: return new Glyph[] { Glyph.FUpper, Glyph.Digit5 };
                        case Key.F6: return new Glyph[] { Glyph.FUpper, Glyph.Digit6 };
                        case Key.F7: return new Glyph[] { Glyph.FUpper, Glyph.Digit7 };
                        case Key.F8: return new Glyph[] { Glyph.FUpper, Glyph.Digit8 };
                        case Key.F9: return new Glyph[] { Glyph.FUpper, Glyph.Digit9 };
                        case Key.F10: return new Glyph[] { Glyph.FUpper, Glyph.Digit1, Glyph.Digit0 };
                        case Key.F11: return new Glyph[] { Glyph.FUpper, Glyph.Digit1, Glyph.Digit1 };
                        case Key.F12: return new Glyph[] { Glyph.FUpper, Glyph.Digit1, Glyph.Digit2 };
                        case Key.F13: return new Glyph[] { Glyph.FUpper, Glyph.Digit1, Glyph.Digit3 };
                        case Key.F14: return new Glyph[] { Glyph.FUpper, Glyph.Digit1, Glyph.Digit4 };
                        case Key.F15: return new Glyph[] { Glyph.FUpper, Glyph.Digit1, Glyph.Digit5 };
                        case Key.F16: return new Glyph[] { Glyph.FUpper, Glyph.Digit1, Glyph.Digit6 };
                        case Key.F17: return new Glyph[] { Glyph.FUpper, Glyph.Digit1, Glyph.Digit7 };
                        case Key.F18: return new Glyph[] { Glyph.FUpper, Glyph.Digit1, Glyph.Digit8 };
                        case Key.F19: return new Glyph[] { Glyph.FUpper, Glyph.Digit1, Glyph.Digit9 };
                        case Key.F20: return new Glyph[] { Glyph.FUpper, Glyph.Digit2, Glyph.Digit0 };
                        case Key.F21: return new Glyph[] { Glyph.FUpper, Glyph.Digit2, Glyph.Digit1 };
                        case Key.F22: return new Glyph[] { Glyph.FUpper, Glyph.Digit2, Glyph.Digit2 };
                        case Key.F23: return new Glyph[] { Glyph.FUpper, Glyph.Digit2, Glyph.Digit3 };
                        case Key.F24: return new Glyph[] { Glyph.FUpper, Glyph.Digit2, Glyph.Digit4 };
                        default:
                            if (Shift)
                            {
                                switch (Key)
                                {
                                    case Key.Space: return new Glyph[] { Glyph.SUpper, Glyph.PLower, Glyph.ALower, Glyph.CLower, Glyph.ELower };
                                    case Key.Accent: return new Glyph[] { Glyph.Tilde };
                                    case Key.Dash: return new Glyph[] { Glyph.Underscore };
                                    case Key.Equals: return new Glyph[] { Glyph.Plus };
                                    case Key.OpenBracket: return new Glyph[] { Glyph.OpenBrace };
                                    case Key.CloseBracket: return new Glyph[] { Glyph.CloseBrace };
                                    case Key.Backslash: return new Glyph[] { Glyph.Pipe };
                                    case Key.Semicolon: return new Glyph[] { Glyph.Colon };
                                    case Key.Apostrophe: return new Glyph[] { Glyph.Quote };
                                    case Key.Comma: return new Glyph[] { Glyph.LessThan };
                                    case Key.Period: return new Glyph[] { Glyph.GreaterThan };
                                    case Key.Slash: return new Glyph[] { Glyph.QuestionMark };
                                }
                            }
                            else
                            {
                                switch (Key)
                                {
                                    case Key.Space: return new Glyph[] { Glyph.SUpper, Glyph.PLower, Glyph.ALower, Glyph.CLower, Glyph.ELower };
                                    case Key.Accent: return new Glyph[] { Glyph.Accent };
                                    case Key.Dash: return new Glyph[] { Glyph.Dash };
                                    case Key.Equals: return new Glyph[] { Glyph.Equals };
                                    case Key.OpenBracket: return new Glyph[] { Glyph.OpenBracket };
                                    case Key.CloseBracket: return new Glyph[] { Glyph.CloseBracket };
                                    case Key.Backslash: return new Glyph[] { Glyph.Backslash };
                                    case Key.Semicolon: return new Glyph[] { Glyph.Semicolon };
                                    case Key.Apostrophe: return new Glyph[] { Glyph.Apostrophe };
                                    case Key.Comma: return new Glyph[] { Glyph.Comma };
                                    case Key.Period: return new Glyph[] { Glyph.Period };
                                    case Key.Slash: return new Glyph[] { Glyph.Slash };
                                }
                            }
                            throw new Exception("Unknown KeyInfo values.");
                    }
                }

                throw new Exception("Unknown KeyInfo values.");
            }
        }

        public char? Character
        {
            get
            {
                foreach (KeyValuePair<char, KeyInfo> pair in sCharMap)
                {
                    if (pair.Value.Equals(this))
                    {
                        return pair.Key;
                    }
                }

                return null;
            }
        }

        public char? TextChar
        {
            get
            {
                return Character;
            }
        }

        public KeyInfo(Key key, bool shift)
        {
            Key = key;
            Shift = shift;
            Control = false;
            Down = true;
        }

        public KeyInfo(Key key)
            : this(key, false)
        {
        }

        public override string ToString()
        {
            return Key.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is KeyInfo)
            {
                return Equals((KeyInfo)obj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode() + Shift.GetHashCode() + Control.GetHashCode();
        }

        static KeyInfo()
        {
            // create the character map
            sCharMap = new Dictionary<char, KeyInfo>();

            // letters
            for (char c = 'a'; c <= 'z'; c++)
            {
                sCharMap[c] = new KeyInfo((Key)((int)Key.A + (c - 'a')), false);
                sCharMap[Char.ToUpper(c)] = new KeyInfo((Key)((int)Key.A + (c - 'a')), true);
            }

            // digits
            for (char c = '0'; c <= '9'; c++)
            {
                sCharMap[c] = new KeyInfo((Key)((int)Key.Digit0 + (c - '0')), false);
            }

            // shift digits
            sCharMap.Add(')', new KeyInfo(Key.Digit0, true));
            sCharMap.Add('!', new KeyInfo(Key.Digit1, true));
            sCharMap.Add('@', new KeyInfo(Key.Digit2, true));
            sCharMap.Add('#', new KeyInfo(Key.Digit3, true));
            sCharMap.Add('$', new KeyInfo(Key.Digit4, true));
            sCharMap.Add('%', new KeyInfo(Key.Digit5, true));
            sCharMap.Add('^', new KeyInfo(Key.Digit6, true));
            sCharMap.Add('&', new KeyInfo(Key.Digit7, true));
            sCharMap.Add('*', new KeyInfo(Key.Digit8, true));
            sCharMap.Add('(', new KeyInfo(Key.Digit9, true));

            // other
            sCharMap.Add(' ', new KeyInfo(Key.Space, false));
            sCharMap.Add('`', new KeyInfo(Key.Accent, false));
            sCharMap.Add('~', new KeyInfo(Key.Accent, true));
            sCharMap.Add('-', new KeyInfo(Key.Dash, false));
            sCharMap.Add('_', new KeyInfo(Key.Dash, true));
            sCharMap.Add('=', new KeyInfo(Key.Equals, false));
            sCharMap.Add('+', new KeyInfo(Key.Equals, true));
            sCharMap.Add('[', new KeyInfo(Key.OpenBracket, false));
            sCharMap.Add('{', new KeyInfo(Key.OpenBracket, true));
            sCharMap.Add(']', new KeyInfo(Key.CloseBracket, false));
            sCharMap.Add('}', new KeyInfo(Key.CloseBracket, true));
            sCharMap.Add('\\', new KeyInfo(Key.Backslash, false));
            sCharMap.Add('|', new KeyInfo(Key.Backslash, true));
            sCharMap.Add(';', new KeyInfo(Key.Semicolon, false));
            sCharMap.Add(':', new KeyInfo(Key.Semicolon, true));
            sCharMap.Add('\'', new KeyInfo(Key.Apostrophe, false));
            sCharMap.Add('"', new KeyInfo(Key.Apostrophe, true));
            sCharMap.Add(',', new KeyInfo(Key.Comma, false));
            sCharMap.Add('<', new KeyInfo(Key.Comma, true));
            sCharMap.Add('.', new KeyInfo(Key.Period, false));
            sCharMap.Add('>', new KeyInfo(Key.Period, true));
            sCharMap.Add('/', new KeyInfo(Key.Slash, false));
            sCharMap.Add('?', new KeyInfo(Key.Slash, true));
        }

        #region IEquatable<KeyInfo> Members

        public bool Equals(KeyInfo other)
        {
            if (Key != other.Key) return false;
            if (Shift != other.Shift) return false;
            if (Control != other.Control) return false;

            return true;
        }

        #endregion

        private static Dictionary<char, KeyInfo> sCharMap;
    }
}
