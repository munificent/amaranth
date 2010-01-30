using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;
using Amaranth.Terminals;
using Amaranth.UI;
using Amaranth.Engine;

namespace Amaranth.TermApp
{
    public class StatusControl : RectControl
    {
        public StatusControl(Game game, Rect bounds)
            : base(bounds)
        {
            mGame = game;

            RepaintOn(mGame.Hero.Changed);
            RepaintOn(mGame.FloorChanged);
        }

        protected override void OnPaint(ITerminal terminal)
        {
            terminal.Clear();

            terminal[0, 0, 50, 1][TerminalColors.DarkGray].DrawBox(false, true);

            // write the conditions
            int x = 1;

            // disease
            if (mGame.Hero.Health.HasBonus(BonusType.Disease))
            {
                terminal[x, 0][TerminalColors.Purple].Write("Disease");
                x += "Disease ".Length;
            }

            // poison
            if (mGame.Hero.Conditions.Poison.IsActive)
            {
                terminal[x, 0][TerminalColors.DarkGreen].Write("Poison");
                x += "Poison ".Length;
            }

            // slow
            if (mGame.Hero.Conditions.Slow.IsActive)
            {
                terminal[x, 0][TerminalColors.Brown].Write("Slow");
                x += "Slow ".Length;
            }

            // freeze
            if (mGame.Hero.Conditions.Freeze.IsActive)
            {
                terminal[x, 0][TerminalColors.LightBlue].Write("Freeze");
                x += "Freeze ".Length;
            }

            // haste
            if (mGame.Hero.Conditions.Haste.IsActive)
            {
                terminal[x, 0][TerminalColors.Orange].Write("Haste");
                x += "Haste ".Length;
            }

            // speed
            int speed = mGame.Hero.Speed - Energy.NormalSpeed;
            if (mGame.Hero.Speed != Energy.NormalSpeed)
            {
                string speedText = String.Empty;

                switch (mGame.Hero.Speed)
                {
                    case 0: speedText = "Slow 1/4x"; break;
                    case 1: speedText = "Slow 1/3x"; break;
                    case 2: speedText = "Slow 2/5x"; break;
                    case 3: speedText = "Slow 1/2x"; break;
                    case 4: speedText = "Slow 2/3x"; break;
                    case 5: speedText = "Slow 5/6x"; break;

                    case 7: speedText = "Fast 4/3x"; break;
                    case 8: speedText = "Fast 5/3"; break;
                    case 9: speedText = "Fast 2x"; break;
                    case 10: speedText = "Fast 5/2x"; break;
                    case 11: speedText = "Fast 3x"; break;
                    case 12: speedText = "Fast 4x"; break;
                }

                if (speed > Energy.NormalSpeed)
                {
                    terminal[x, 0][TerminalColors.Green].Write(speedText);
                    x += speedText.Length + 1;
                }
                else
                {
                    terminal[x, 0][TerminalColors.Orange].Write(speedText);
                    x += speedText.Length + 1;
                }
            }

            // write the floor
            terminal[41, 0][TerminalColors.Gray].Write("Depth:");
            terminal[47, 0][TerminalColors.Cyan].Write(mGame.Depth.ToString());
        }

        private readonly Game mGame;
    }
}
