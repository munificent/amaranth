using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using Bramble.Core;

using Amaranth.Engine;
using Amaranth.Terminals;
using Amaranth.UI;
using Amaranth.Util;

namespace Amaranth.TermApp
{
    public class NewStatsControl : PositionControl, IFocusable
    {
        public HeroRace Race
        {
            get { return mRace; }
            set
            {
                if (mRace != value)
                {
                    mRace = value;
                    Repaint();
                }
            }
        }

        /// <summary>
        /// Gets the current set of stats with the race modifiers applied.
        /// </summary>
        public Stats Stats
        {
            get
            {
                Stats stats = new Stats();

                for (int i = 0; i < stats.Count; i++)
                {
                    stats[i].Base = mStats[i].Base + mRace.StatBonuses[i];
                }

                return stats;
            }
        }

        public NewStatsControl(Vec position)
            : base(position, "Stats")
        {
            RollStats();
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
            int nameWidth = mStats.Max((stat) => stat.Name.Length);

            for (int i = 0; i < mStats.Count; i++)
            {
                terminal[x, i][TextColor].Write(mStats[i].Name);

                // raw value
                int raw = mStats[i].Current;
                terminal[x + nameWidth + 1, i][TerminalColors.Gray].Write(raw.ToString().PadLeft(2));

                // race bonus
                terminal[x + nameWidth + 4, i].Write(mRace.StatBonuses[i].ToString("^g+##;^r-##;^m 0"));

                // final value
                int final = mStats[i].Current + mRace.StatBonuses[i];
                terminal[x + nameWidth + 7, i][TerminalColors.White].Write(final.ToString().PadLeft(2));

                // stat bar
                for (int j = 1; j <= Math.Max(final, raw); j++)
                {
                    Glyph glyph = Glyph.BarDoubleLeftRight;
                    Color color = TerminalColors.Gray;

                    if (j == 1)
                    {
                        glyph = Glyph.BarUpDownDoubleRight;
                    }
                    else if (j == final)
                    {
                        glyph = Glyph.Solid;
                    }
                    else if (j > final)
                    {
                        glyph = Glyph.BarLeftRight;
                    }

                    if (j <= raw)
                    {
                        if (j <= final)
                        {
                            color = TerminalColors.Gray;
                        }
                        else
                        {
                            color = TerminalColors.DarkRed;
                        }
                    }
                    else
                    {
                        color = TerminalColors.Green;
                    }

                    terminal[x + nameWidth + 9 + j, i][color].Write(glyph);
                }
            }

            // draw the average line
            terminal[x + nameWidth + 9 + 15, 6][TerminalColors.DarkGray].Write(Glyph.TriangleUp);
            terminal[x + nameWidth + 9 + 12, 7][TerminalColors.DarkGray].Write("Average");
        }

        protected override Rect GetBounds()
        {
            // add title width
            int width = Title.Length + 3;

            // and the width of the stat names
            width += mStats.Max((stat) => stat.Name.Length);

            // and the stat value
            width += 3;

            // and the race bonus
            width += 4;

            // and the final value
            width += 7;

            // and the stat bar
            width += 30;

            // the height is the number of options
            int height = mStats.Count;

            // plus the "ave" marker
            height += 2;

            return new Rect(Position.X, Position.Y, width, height);
        }

        #region IFocusable Members

        public string Instruction
        {
            get { return "Roll the stats"; }
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
                yield return new KeyInstruction("Roll new stats", new KeyInfo(Key.Space));

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

                case Key.Space:
                    RollStats();
                    Repaint();
                    break;

                default:
                    handled = false;
                    break;
            }

            return handled;
        }

        public bool KeyUp(KeyInfo key)
        {
            return false;
        }

        #endregion

        private void RollStats()
        {
            mStats = new Stats();
        }

        private Stats mStats;
        private HeroRace mRace;
    }
}
