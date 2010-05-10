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
    public class DescriptionControl : RectControl
    {
        public DescriptionControl(Game game, Rect bounds)
            : base(bounds)
        {
            mHero = game.Hero;

            game.ThingNoticed.Add(Game_ThingNoticed);
        }

        protected override void OnPaint(ITerminal terminal)
        {
            terminal.Clear();

            Monster monster = mThing as Monster;

            if (monster != null)
            {
                // draw the object
                terminal.Write(GameArt.Get(monster));

                // draw its name
                terminal[2, 0].Write(monster.NounText);

                // draw its health if alive
                if (monster.Health.Current > 0)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        if (monster.Health.Current * 10 / monster.Health.Max >= i)
                        {
                            terminal[-10 + i, 0][TerminalColors.DarkRed].Write(Glyph.Solid);
                        }
                        else
                        {
                            terminal[-10 + i, 0][TerminalColors.DarkGray].Write(Glyph.Gray);
                        }
                    }
                }

                // draw its description
                CharacterString text = new CharacterString(monster.GetDescription(mHero), TerminalColors.LightGray, TerminalColors.Black);

                int y = 1;
                foreach (CharacterString line in text.WordWrap(terminal.Size.X))
                {
                    // bail if we run out of room
                    if (y >= terminal.Size.Y) break;

                    terminal[0, y++].Write(line);
                }
            }
        }

        private void Game_ThingNoticed(Thing sender, EventArgs e)
        {
            // only if it changed
            if (sender != mThing)
            {
                // unregister the old event
                Monster oldMonster = mThing as Monster;
                if (oldMonster != null)
                {
                    oldMonster.Changed.Remove(Monster_Changed);
                }

                mThing = sender;

                Monster monster = mThing as Monster;
                if (monster != null)
                {
                    monster.Changed.Add(Monster_Changed);
                }

                Repaint();
            }
        }

        private void Monster_Changed(Entity sender, EventArgs args)
        {
            Repaint();
        }

        private Hero mHero;
        private Thing mThing;
    }
}
