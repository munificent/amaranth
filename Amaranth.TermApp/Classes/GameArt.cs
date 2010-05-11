using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Amaranth.Terminals;
using Amaranth.Engine;
using Amaranth.Util;

namespace Amaranth.TermApp
{
    //### bob: reimplement as extension methods?
    /// <summary>
    /// Defines the glyphs used to render various game entities.
    /// </summary>
    public static class GameArt
    {
        public static Color GetColor(Element element)
        {
            switch (element)
            {
                case Element.Air:       return TerminalColors.Cyan;
                case Element.Earth:     return TerminalColors.LightBrown;
                case Element.Fire:      return TerminalColors.Red;
                case Element.Water:     return TerminalColors.Blue;
                case Element.Metal:     return TerminalColors.LightGray;
                case Element.Wood:      return TerminalColors.Brown;
                case Element.Acid:      return TerminalColors.Green;
                case Element.Cold:      return TerminalColors.White;
                case Element.Lightning: return TerminalColors.Purple;
                case Element.Poison:    return TerminalColors.DarkGreen;
                case Element.Dark:      return TerminalColors.DarkGray;
                case Element.Light:     return TerminalColors.LightYellow;
                case Element.Anima:     return TerminalColors.Gold;
                case Element.Death:     return TerminalColors.DarkRed;
                default: throw new UnexpectedEnumValueException(element);
            }
        }

        public static string GetAbbreviation2(Element element)
        {
            switch (element)
            {
                case Element.Air:       return "Ai";
                case Element.Earth:     return "Ea";
                case Element.Fire:      return "Fi";
                case Element.Water:     return "Wa";
                case Element.Metal:     return "Me";
                case Element.Wood:      return "Wo";
                case Element.Acid:      return "Ac";
                case Element.Cold:      return "Co";
                case Element.Lightning: return "Ln";
                case Element.Poison:    return "Po";
                case Element.Dark:      return "Da";
                case Element.Light:     return "Li";
                case Element.Anima:     return "An";
                case Element.Death:     return "De";
                default: throw new UnexpectedEnumValueException(element);
            }
        }
        
        public static string GetAbbreviation4(Element element)
        {
            switch (element)
            {
                case Element.Air:       return "Air";
                case Element.Earth:     return "Eart";
                case Element.Fire:      return "Fire";
                case Element.Water:     return "Watr";
                case Element.Metal:     return "Metl";
                case Element.Wood:      return "Wood";
                case Element.Acid:      return "Acid";
                case Element.Cold:      return "Cold";
                case Element.Lightning: return "Ltng";
                case Element.Poison:    return "Pois";
                case Element.Dark:      return "Dark";
                case Element.Light:     return "Lite";
                case Element.Anima:     return "Anim";
                case Element.Death:     return "Deth";
                default: throw new UnexpectedEnumValueException(element);
            }
        }

        public static Character Get(Effect effect)
        {
            Glyph glyph = Glyph.Asterisk;
            Color backColor = TerminalColors.Black;
            Color foreColor = GetColor(effect.Element);

            // for colored backgrounds
            /*
            switch (effect.Element)
            {
                case Element.Air:
                    backColor = TerminalColors.DarkCyan;
                    foreColor = TerminalColors.LightCyan; break;
                case Element.Earth:
                    backColor = TerminalColors.DarkBrown;
                    foreColor = TerminalColors.LightBrown; break;
                case Element.Fire:
                    backColor = TerminalColors.DarkRed;
                    foreColor = TerminalColors.Red; break;
                case Element.Water:
                    backColor = TerminalColors.DarkBlue;
                    foreColor = TerminalColors.LightBlue; break;
                case Element.Metal:
                    backColor = TerminalColors.DarkGray;
                    foreColor = TerminalColors.LightGray; break;
                case Element.Wood:
                    backColor = TerminalColors.DarkBrown;
                    foreColor = TerminalColors.Brown; break;
                case Element.Acid:
                    backColor = TerminalColors.DarkPurple;
                    foreColor = TerminalColors.Green; break;
                case Element.Cold:
                    backColor = TerminalColors.DarkCyan;
                    foreColor = TerminalColors.White; break;
                case Element.Lightning:
                    backColor = TerminalColors.Purple;
                    foreColor = TerminalColors.White; break;
                case Element.Poison:
                    backColor = TerminalColors.DarkGreen;
                    foreColor = TerminalColors.Green; break;
                case Element.Dark:
                    backColor = TerminalColors.DarkGray;
                    foreColor = TerminalColors.Black; break;
                case Element.Light:
                    backColor = TerminalColors.White;
                    foreColor = TerminalColors.Gray; break;
                case Element.Anima:
                    backColor = TerminalColors.DarkBrown;
                    foreColor = TerminalColors.Orange; break;
                case Element.Death:
                    backColor = TerminalColors.Purple;
                    foreColor = TerminalColors.Black; break;
            }
            */

            switch (effect.Type)
            {
                case EffectType.Hit:
                case EffectType.Bolt:
                case EffectType.Ball:
                case EffectType.Cone:
                    glyph = Glyph.Asterisk;
                    break;

                case EffectType.BallTrail:
                case EffectType.ConeTrail:
                case EffectType.Stone:
                    glyph = Glyph.Bullet;
                    break;

                case EffectType.Stab:
                case EffectType.Arrow:
                    {
                        int index = Direction.Clockwise.IndexOf(effect.Direction);
                        if (index >= 0)
                        {
                            switch (index % 4)
                            {
                                case 0: glyph = Glyph.Pipe; break;
                                case 1: glyph = Glyph.Slash; break;
                                case 2: glyph = Glyph.Dash; break;
                                case 3: glyph = Glyph.Backslash; break;
                            }
                        }
                    }
                    break;

                case EffectType.Slash:
                    {
                        int index = Direction.Clockwise.IndexOf(effect.Direction);
                        if (index >= 0)
                        {
                            switch (index % 4)
                            {
                                case 0: glyph = Glyph.Backslash; break;
                                case 1: glyph = Glyph.Pipe; break;
                                case 2: glyph = Glyph.Slash; break;
                                case 3: glyph = Glyph.Dash; break;
                            }
                        }
                    }
                    break;

                case EffectType.Teleport:
                    glyph = Glyph.Tilde;
                    break;
            }

            return new Character(glyph, foreColor, backColor);
        }

        public static Character Get(Tile tile)
        {
            // unexplored tiles are blank
            if (!tile.IsExplored) return new Character(Glyph.Space);

            //### bob: hack if block. should just recolor based on vis
            if (tile.IsVisible && tile.IsLit)
            {
                switch (tile.Type)
                {
                    case TileType.Floor:
                        // hilight "temporary" lighting
                        if (tile.IsLitByThing)
                        {
                            return new Character(Glyph.Period, TerminalColors.Yellow);
                        }
                        else
                        {
                            return new Character(Glyph.Period, TerminalColors.Gray);
                        }
                    case TileType.Wall: return new Character(Glyph.GrayFill, TerminalColors.Gray);
                    case TileType.LowWall: return new Character(Glyph.LightFill, TerminalColors.Gray);
                    case TileType.StairsUp: return new Character(Glyph.ArrowUp, TerminalColors.White);
                    case TileType.StairsDown: return new Character(Glyph.ArrowDown, TerminalColors.White);
                    case TileType.DoorOpen:
                        // hilight "temporary" lighting
                        if (tile.IsLitByThing)
                        {
                            return new Character(Glyph.Box, TerminalColors.LightBrown);
                        }
                        else
                        {
                            return new Character(Glyph.Box, TerminalColors.Brown);
                        }
                    case TileType.DoorClosed: return new Character(Glyph.Door, TerminalColors.LightBrown);
                    case TileType.TownPortal: return new Character(Glyph.Caret, TerminalColors.Green);
                    case TileType.RoofLight: return new Character(Glyph.HorizontalBarsFill, TerminalColors.Brown);
                    case TileType.RoofDark: return new Character(Glyph.HorizontalBarsFill, TerminalColors.DarkBrown);
                    case TileType.DoorStore1: return new Character(Glyph.Digit1, TerminalColors.Orange);
                    case TileType.DoorStore2: return new Character(Glyph.Digit2, TerminalColors.Red);
                    case TileType.DoorStore3: return new Character(Glyph.Digit3, TerminalColors.Cyan);
                    case TileType.DoorStore4: return new Character(Glyph.Digit4, TerminalColors.Purple);
                    case TileType.DoorStore5: return new Character(Glyph.Digit5, TerminalColors.Blue);
                    case TileType.DoorStore6: return new Character(Glyph.Digit6, TerminalColors.Green);
                }
            }
            else
            {
                switch (tile.Type)
                {
                    case TileType.Floor: return new Character(Glyph.Period, TerminalColors.DarkGray);
                    case TileType.Wall: return new Character(Glyph.GrayFill, TerminalColors.DarkGray);
                    case TileType.LowWall: return new Character(Glyph.LightFill, TerminalColors.DarkGray);
                    case TileType.StairsUp: return new Character(Glyph.ArrowUp, TerminalColors.Gray);
                    case TileType.StairsDown: return new Character(Glyph.ArrowDown, TerminalColors.Gray);
                    case TileType.DoorOpen: return new Character(Glyph.Box, TerminalColors.DarkBrown);
                    case TileType.DoorClosed: return new Character(Glyph.Door, TerminalColors.DarkBrown);
                    case TileType.TownPortal: return new Character(Glyph.Caret, TerminalColors.DarkGreen);
                    case TileType.RoofLight: return new Character(Glyph.HorizontalBarsFill, TerminalColors.Brown);
                    case TileType.RoofDark: return new Character(Glyph.HorizontalBarsFill, TerminalColors.DarkBrown);
                    case TileType.DoorStore1: return new Character(Glyph.Digit1, TerminalColors.Orange);
                    case TileType.DoorStore2: return new Character(Glyph.Digit2, TerminalColors.Red);
                    case TileType.DoorStore3: return new Character(Glyph.Digit3, TerminalColors.Cyan);
                    case TileType.DoorStore4: return new Character(Glyph.Digit4, TerminalColors.Purple);
                    case TileType.DoorStore5: return new Character(Glyph.Digit5, TerminalColors.Blue);
                    case TileType.DoorStore6: return new Character(Glyph.Digit6, TerminalColors.Green);
                }
            }

            // unknown tile type
            return new Character(Glyph.QuestionMark, TerminalColors.Purple);
        }

        public static Character Get(Entity entity)
        {
            Hero hero = entity as Hero;

            if (hero != null)
            {
                Color color = TerminalColors.Flesh;

                // choose a color based on condition

                // disease
                if (hero.Health.HasBonus(BonusType.Disease))
                {
                    color = TerminalColors.Purple;
                }

                // poison
                if (hero.Conditions.Poison.IsActive)
                {
                    color = TerminalColors.DarkGreen;
                }

                // near death
                if (hero.Health.Current < (hero.Health.Max / 5))
                {
                    color = TerminalColors.Red;
                }

                return new Character(Glyph.Face, color);
            }

            // monster
            Monster monster = (Monster)entity;
            return (Character)(monster.Race.Appearance);
        }

        public static Character Get(Item item)
        {
            Character c = (Character)item.Type.Appearance;

            // if the power provides a color, use it
            if ((item.Power != null) && (item.Power.Appearance != null))
            {
                c = new Character(c.Glyph, (Color)item.Power.Appearance, c.BackColor);
            }

            return c;
        }
    }
}
