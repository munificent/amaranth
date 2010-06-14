using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using Malison.Core;

using Amaranth.Engine;
using Amaranth.UI;

namespace Amaranth.TermApp
{
    /// <summary>
    /// A screen that shows detailed stat information for a Hero.
    /// </summary>
    public class HeroInfoScreen : Screen, IInputHandler
    {
        public HeroInfoScreen(Hero hero)
            : base("Info for " + hero.Name)
        {
            mHero = hero;

            Controls.Add(new StatusBar());
        }

        protected override void OnPaint(ITerminal terminal)
        {
            terminal.Clear();

            const int StrikeX = 3;
            const int DamageX = 9;
            const int DodgeX = 16;
            const int ArmorX = 23;
            const int ElemX = 27;
            const int StatX = 32;
            const int ResistX = 51;
            const int LabelX = 80;

            // draw the table
            terminal[0, 0, 80, 28][TermColor.DarkGray].DrawBox();
            terminal[31, 0, 1, 28][TermColor.DarkGray].DrawBox(DrawBoxOptions.DoubleLines);
            terminal[50, 0, 1, 28][TermColor.DarkGray].DrawBox(DrawBoxOptions.DoubleLines);

            terminal[31, 0][TermColor.DarkGray].Write(Glyph.BarDoubleDownSingleLeftRight);
            terminal[50, 0][TermColor.DarkGray].Write(Glyph.BarDoubleDownSingleLeftRight);
            terminal[31, 27][TermColor.DarkGray].Write(Glyph.BarDoubleUpSingleLeftRight);
            terminal[50, 27][TermColor.DarkGray].Write(Glyph.BarDoubleUpSingleLeftRight);

            // write the header
            terminal[14, 0][TermColor.Gray].Write("Melee");
            terminal[39, 0][TermColor.Gray].Write("Stats");
            terminal[60, 0][TermColor.Gray].Write("Resistances");

            terminal[1, 1].Write("Strike Damage Dodge Armor Elem");
            terminal[StatX, 1].Write("StrAgiStaWilIntCha");

            // write the elements in color
            int x = ResistX;
            foreach (Element element in Enum.GetValues(typeof(Element)))
            {
                terminal[x, 1][GameArt.GetColor(element)].Write(GameArt.GetAbbreviation2(element));
                x += 2;
            }

            int y = 2;

            DrawRowLine(terminal, y++);

            // write the base values
            terminal[LabelX, y].Write("Base");

            // melee
            terminal[StrikeX, y].Write(" 0");
            terminal[DamageX, y].Write("x1.0");
            terminal[DodgeX, y].Write(Hero.DodgeBase.ToString());
            terminal[ArmorX, y].Write(" 0");
            terminal[ElemX, y][GameArt.GetColor(Element.Anima)].Write(GameArt.GetAbbreviation4(Element.Anima));

            // stats
            x = StatX;
            foreach (Stat stat in mHero.Stats)
            {
                terminal[x, y].Write(stat.Base.ToString().PadLeft(2));
                x += 3;
            }

            // resists
            terminal[ResistX, y].Write("0 0 0 0 0 0 0 0 0 0 0 0 0 0");

            y++;

            DrawRowLine(terminal, y++);

            // draw the equipment
            for (int i = 0; i < mHero.Equipment.Count; i++)
            {
                Item item = mHero.Equipment[i];

                if (item != null)
                {
                    terminal[LabelX, y].Write(item.ToString());
                }
                else
                {
                    terminal[LabelX, y][TermColor.DarkGray].Write(mHero.Equipment.GetCategory(i));
                }

                // melee stats
                if (item != null)
                {
                    // strike
                    WriteBonus(terminal, StrikeX, y, item.StrikeBonus);

                    // damage
                    float bonus = item.DamageBonus;
                    if (bonus < 1.0f)
                    {
                        terminal[DamageX, y][TermColor.Red].Write("x" + bonus.ToString("F2"));
                    }
                    else if (bonus == 1.0f)
                    {
                        terminal[DamageX, y][TermColor.DarkGray].Write("  -  ");
                    }
                    else
                    {
                        terminal[DamageX, y][TermColor.Green].Write("x" + bonus.ToString("F2"));
                    }

                    // dodge
                    terminal[DodgeX + 1, y][TermColor.DarkGray].Write(Glyph.Dash);

                    // armor
                    TermColor color = TermColor.White;
                    if (item.ArmorBonus < 0)
                    {
                        color = TermColor.Red;
                    }
                    else if (item.ArmorBonus > 0)
                    {
                        color = TermColor.Green;
                    }
                    else if (item.TotalArmor == 0)
                    {
                        color = TermColor.DarkGray;
                    }

                    if (item.TotalArmor < 0)
                    {
                        terminal[ArmorX, y][color].Write(Glyph.ArrowDown);
                        terminal[ArmorX + 1, y][color].Write(Math.Abs(item.TotalArmor).ToString());
                    }
                    else if (item.TotalArmor == 0)
                    {
                        terminal[ArmorX + 1, y][color].Write(Glyph.Dash);
                    }
                    else
                    {
                        terminal[ArmorX, y][color].Write(Glyph.ArrowUp);
                        terminal[ArmorX + 1, y][color].Write(item.TotalArmor.ToString());
                    }

                    // element
                    if (item.Attack != null)
                    {
                        Element element = item.Attack.Element;
                        terminal[ElemX, y][GameArt.GetColor(element)].Write(GameArt.GetAbbreviation4(element));
                    }
                }

                // stat bonuses
                x = StatX;
                foreach (Stat stat in mHero.Stats)
                {
                    if (item != null)
                    {
                        WriteBonus(terminal, x, y, item.GetStatBonus(stat));
                    }
                    x += 3;
                }

                // resists
                x = ResistX;
                foreach (Element element in Enum.GetValues(typeof(Element)))
                {
                    if (item != null)
                    {
                        if (item.Resists(element))
                        {
                            terminal[x, y][TermColor.Green].Write(Glyph.ArrowUp);
                        }
                        else
                        {
                            terminal[x, y][TermColor.DarkGray].Write(Glyph.Dash);
                        }
                    }
                    x += 2;
                }

                y++;
            }

            DrawRowLine(terminal, y++);

            // draw the stats
            foreach (Stat stat in mHero.Stats)
            {
                terminal[LabelX, y].Write(stat.Name);

                // melee bonuses

                // strike
                if (stat == mHero.Stats.Agility)
                {
                    WriteBonus(terminal, StrikeX, y, mHero.Stats.Agility.StrikeBonus);
                }

                // damage
                if (stat == mHero.Stats.Strength)
                {
                    float bonus = mHero.Stats.Strength.DamageBonus;
                    if (bonus < 1.0f)
                    {
                        terminal[DamageX, y][TermColor.Red].Write("x" + bonus.ToString("F1"));
                    }
                    else if (bonus == 1.0f)
                    {
                        terminal[DamageX, y][TermColor.DarkGray].Write("  -  ");
                    }
                    else
                    {
                        terminal[DamageX, y][TermColor.Green].Write("x" + bonus.ToString("F1"));
                    }
                }

                // dodge
                if (stat == mHero.Stats.Agility)
                {
                    WriteBonus(terminal, DodgeX, y, mHero.Stats.Agility.StrikeBonus);
                }

                // armor

                y++;
            }

            DrawRowLine(terminal, y++);

            terminal[LabelX, y].Write("Total");

            // melee totals
            if (mHero.MeleeStrikeBonus < 0)
            {
                terminal[StrikeX, y][TermColor.Red].Write(mHero.MeleeStrikeBonus.ToString());
            }
            else if (mHero.MeleeStrikeBonus == 0)
            {
                terminal[StrikeX + 1, y][TermColor.DarkGray].Write(Glyph.Digit0);
            }
            else
            {
                terminal[StrikeX, y][TermColor.Green].Write(Glyph.Plus);
                terminal[StrikeX + 1, y][TermColor.Green].Write(mHero.MeleeStrikeBonus.ToString());
            }

            // damage
            if (mHero.MeleeDamageBonus < 1.0f)
            {
                terminal[DamageX, y][TermColor.Red].Write("x" + mHero.MeleeDamageBonus.ToString("F1"));
            }
            else if (mHero.MeleeDamageBonus == 1.0f)
            {
                terminal[DamageX, y][TermColor.DarkGray].Write("x" + mHero.MeleeDamageBonus.ToString("F1"));
            }
            else
            {
                terminal[DamageX, y][TermColor.Green].Write("x" + mHero.MeleeDamageBonus.ToString("F1"));
            }

            // dodge
            if (mHero.GetDodge() < Hero.DodgeBase)
            {
                terminal[DodgeX, y][TermColor.Red].Write(mHero.GetDodge().ToString());
            }
            else
            {
                terminal[DodgeX, y].Write(mHero.GetDodge().ToString());
            }

            // armor
            if (mHero.Armor < 0)
            {
                terminal[ArmorX, y][TermColor.Red].Write(mHero.Armor.ToString());
            }
            else if (mHero.Armor == 0)
            {
                terminal[ArmorX + 1, y][TermColor.DarkGray].Write("0");
            }
            else
            {
                terminal[ArmorX + 1, y][TermColor.Green].Write(mHero.Armor.ToString());
            }

            // stat totals
            x = StatX;
            foreach (Stat stat in mHero.Stats)
            {
                if (stat.IsLowered)
                {
                    terminal[x, y][TermColor.Red].Write(stat.Current.ToString().PadLeft(2));
                }
                else if (stat.IsRaised)
                {
                    terminal[x, y][TermColor.Green].Write(stat.Current.ToString().PadLeft(2));
                }
                else
                {
                    terminal[x, y].Write(stat.Current.ToString().PadLeft(2));
                }
                x += 3;
            }

            // resistance totals
            x = ResistX;
            foreach (Element element in Enum.GetValues(typeof(Element)))
            {
                int resists = mHero.GetNumResists(element);

                if (resists == 0)
                {
                    terminal[x, y].Write(Glyph.Digit0);
                }
                else
                {
                    terminal[x, y][TermColor.Green].Write(resists.ToString());
                }
                x += 2;
            }

            // element
            Element attackElement = Element.Anima;
            Item weapon = mHero.Equipment.MeleeWeapon;
            if (weapon != null)
            {
                if (weapon.Attack != null)
                {
                    attackElement = weapon.Attack.Element;
                }
            }
            terminal[ElemX, y][GameArt.GetColor(attackElement)].Write(GameArt.GetAbbreviation4(attackElement));

            y += 2;
            terminal[1, y].Write("A total armor of " + mHero.Armor + " reduces damage by " + (100 - (int)(100.0f * Entity.GetArmorReduction(mHero.Armor))) + "%.");
        }

        #region IInputHandler Members

        public IEnumerable<KeyInstruction> KeyInstructions
        {
            get
            {
                yield return new KeyInstruction("Return to Game", new KeyInfo(Key.Escape));
                yield return new KeyInstruction("Show Class Info", new KeyInfo(Key.Tab));
            }
        }

        public bool KeyDown(KeyInfo key)
        {
            if (key.Key == Key.Escape)
            {
                UI.PopScreen();
                return true;
            }
            else if (key.Key == Key.Tab)
            {
                UI.SetScreen(new ClassInfoScreen(mHero));
                return true;
            }

            return false;
        }

        public bool KeyUp(KeyInfo key)
        {
            return false;
        }

        #endregion

        private void WriteBonus(ITerminal terminal, int x, int y, int bonus)
        {
            if (bonus < 0)
            {
                terminal[x, y][TermColor.Red].Write(Glyph.ArrowDown);
                terminal[x + 1, y][TermColor.Red].Write(Math.Abs(bonus).ToString());
            }
            else if (bonus == 0)
            {
                terminal[x + 1, y][TermColor.DarkGray].Write(Glyph.Dash);
            }
            else
            {
                terminal[x, y][TermColor.Green].Write(Glyph.ArrowUp);
                terminal[x + 1, y][TermColor.Green].Write(bonus.ToString());
            }
        }

        private void DrawRowLine(ITerminal terminal, int y)
        {
            terminal[1, y, 79, 1][TermColor.DarkGray].DrawBox();
            terminal[0, y][TermColor.DarkGray].Write(Glyph.BarUpDownRight);
            terminal[31, y][TermColor.DarkGray].Write(Glyph.BarDoubleUpDownSingleLeftRight);
            terminal[50, y][TermColor.DarkGray].Write(Glyph.BarDoubleUpDownSingleLeftRight);
            terminal[79, y][TermColor.DarkGray].Write(Glyph.BarUpDownLeft);
        }

        private Hero mHero;
    }
}
