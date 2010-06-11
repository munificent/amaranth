using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using Bramble.Core;
using Malison.Core;

using Amaranth.Util;
using Amaranth.UI;
using Amaranth.Engine;

namespace Amaranth.TermApp
{
    public class StatsControl : RectControl
    {
        public StatsControl(Hero hero, Rect bounds)
            : base(bounds)
        {
            mHero = hero;

            RepaintOn(mHero.Changed);
        }

        protected override void OnPaint(ITerminal terminal)
        {
            terminal.Clear();

            terminal[0, 0][TermColor.Cyan].Write(mHero.Name);
            terminal[0, 1][TermColor.DarkCyan].Write(mHero.Race);
            terminal[0, 2][TermColor.DarkCyan].Write("Adventurer");

            WriteStat(terminal, 4, "Level", mHero.Level);
            WriteMaxStat(terminal, 5, "Exp", mHero.Experience, (max) => (max / 100).ToString());
            WriteStat(terminal, 6, "Next", mHero.NextExperience, TermColor.Gray);

            WriteStat(terminal, 8, "Health", mHero.Health);
            WriteMaxStat(terminal, 9, "Max", mHero.Health);

            WriteStat(terminal, 11, "Strength", mHero.Stats.Strength);
            WriteStat(terminal, 12, "Agility", mHero.Stats.Agility);
            WriteStat(terminal, 13, "Stamina", mHero.Stats.Stamina);
            WriteStat(terminal, 14, "Will", mHero.Stats.Will);
            WriteStat(terminal, 15, "Intellect", mHero.Stats.Intellect);
            WriteStat(terminal, 16, "Charisma", mHero.Stats.Charisma);

            WriteStat(terminal, 18, "Armor", mHero.Armor);
            terminal[0, 19][TermColor.Gray].Write("Resistances");

            Vec pos = new Vec(0, 20);
            foreach (object value in Enum.GetValues(typeof(Element)))
            {
                Element element = (Element)value;
                WriteResist(terminal, element, pos);
                pos = pos.OffsetX(1);
            }

            terminal[0, 22][TermColor.Gray].Write("Currency");
            terminal[0, 23][TermColor.Gold].Write(mHero.Currency.ToString("n0").PadLeft(13));
        }

        private void WriteStat(ITerminal terminal, int y, string name, int value, TermColor color)
        {
            terminal[0, y][color].Write(value.ToString().PadLeft(13));
            terminal[0, y][TermColor.Gray].Write(name);
        }

        private void WriteStat(ITerminal terminal, int y, string name, int value)
        {
            WriteStat(terminal, y, name, value, TermColor.White);
        }

        private void WriteMaxStat(ITerminal terminal, int y, string name, FluidStat value, Func<int, string> formatter)
        {
            TermColor color = TermColor.Gray;
            TermColor textColor = TermColor.Gray;

            // highlight stats that are below the base value
            if (value.IsLowered)
            {
                color = TermColor.Purple;
                terminal[-1, y][TermColor.DarkPurple].Write(Glyph.ArrowDown);
                textColor = TermColor.DarkPurple;
            }
            else if (value.IsRaised)
            {
                color = TermColor.Blue;
                terminal[-1, y][TermColor.DarkBlue].Write(Glyph.ArrowUp);
            }

            terminal[0, y][color].Write(formatter(value.Max).PadLeft(terminal.Size.X - 1));
            terminal[0, y][textColor].Write(name);
        }

        private void WriteMaxStat(ITerminal terminal, int y, string name, FluidStat value)
        {
            WriteMaxStat(terminal, y, name, value, (max) => max.ToString());
        }

        private void WriteStat(ITerminal terminal, int y, string name, FluidStat value)
        {
            TermColor color = TermColor.Green;
            TermColor textColor = TermColor.Gray;

            // highlight stats that are below the base value
            if (value.Current < value.Max)
            {
                color = TermColor.Red;
                terminal[-1, y][TermColor.DarkRed].Write(Glyph.ExclamationMark);
                textColor = TermColor.DarkRed;
            }

            terminal[0, y][color].Write(value.Current.ToString().PadLeft(terminal.Size.X - 1));
            terminal[0, y][textColor].Write(name);
        }

        private void WriteStat(ITerminal terminal, int y, string name, Stat stat)
        {
            // drained  red
            // normal   white
            // max      green

            TermColor color = TermColor.White;
            TermColor textColor = TermColor.Gray;

            // highlight stats that have negative bonuses
            if (stat.IsLowered)
            {
                // drained
                color = TermColor.Purple;
                terminal[-1, y][TermColor.DarkPurple].Write(Glyph.ArrowDown);
                textColor = TermColor.DarkPurple;
            }
            else if (stat.Base == Stat.BaseMax)
            {
                // maxed
                color = TermColor.Green;
                terminal[-1, y][TermColor.Green].Write(Glyph.Mountains);
            }
            else if (stat.IsRaised)
            {
                // raised
                terminal[-1, y][TermColor.DarkGreen].Write(Glyph.ArrowUp);
            }

            terminal[0, y][color].Write(stat.Current.ToString().PadLeft(terminal.Size.X - 1));
            terminal[0, y][textColor].Write(name);
        }

        private void WriteResist(ITerminal terminal, Element element, Vec pos)
        {
            int numResists = mHero.GetNumResists(element);

            TermColor color = GameArt.GetColor(element);
            string letter = element.ToString().Substring(0, 1);

            switch (numResists)
            {
                case 0: terminal[pos][TermColor.DarkGray].Write("-"); break;
                case 1: terminal[pos][color].Write(letter.ToLower()); break;
                default: terminal[pos][color].Write(letter); break;
            }
        }

        private Hero mHero;
    }
}
