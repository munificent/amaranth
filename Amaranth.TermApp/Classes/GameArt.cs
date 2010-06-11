using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Bramble.Core;
using Malison.Core;

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
        public static TermColor GetColor(Element element)
        {
            switch (element)
            {
                case Element.Air:       return TermColor.Cyan;
                case Element.Earth:     return TermColor.LightBrown;
                case Element.Fire:      return TermColor.Red;
                case Element.Water:     return TermColor.Blue;
                case Element.Metal:     return TermColor.LightGray;
                case Element.Wood:      return TermColor.Brown;
                case Element.Acid:      return TermColor.Green;
                case Element.Cold:      return TermColor.White;
                case Element.Lightning: return TermColor.Purple;
                case Element.Poison:    return TermColor.DarkGreen;
                case Element.Dark:      return TermColor.DarkGray;
                case Element.Light:     return TermColor.LightYellow;
                case Element.Anima:     return TermColor.Gold;
                case Element.Death:     return TermColor.DarkRed;
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
            var glyph = Glyph.Asterisk;
            var backColor = TermColor.Black;
            var foreColor = GetColor(effect.Element);

            // for colored backgrounds
            /*
            switch (effect.Element)
            {
                case Element.Air:
                    backColor = TermColor.DarkCyan;
                    foreColor = TermColor.LightCyan; break;
                case Element.Earth:
                    backColor = TermColor.DarkBrown;
                    foreColor = TermColor.LightBrown; break;
                case Element.Fire:
                    backColor = TermColor.DarkRed;
                    foreColor = TermColor.Red; break;
                case Element.Water:
                    backColor = TermColor.DarkBlue;
                    foreColor = TermColor.LightBlue; break;
                case Element.Metal:
                    backColor = TermColor.DarkGray;
                    foreColor = TermColor.LightGray; break;
                case Element.Wood:
                    backColor = TermColor.DarkBrown;
                    foreColor = TermColor.Brown; break;
                case Element.Acid:
                    backColor = TermColor.DarkPurple;
                    foreColor = TermColor.Green; break;
                case Element.Cold:
                    backColor = TermColor.DarkCyan;
                    foreColor = TermColor.White; break;
                case Element.Lightning:
                    backColor = TermColor.Purple;
                    foreColor = TermColor.White; break;
                case Element.Poison:
                    backColor = TermColor.DarkGreen;
                    foreColor = TermColor.Green; break;
                case Element.Dark:
                    backColor = TermColor.DarkGray;
                    foreColor = TermColor.Black; break;
                case Element.Light:
                    backColor = TermColor.White;
                    foreColor = TermColor.Gray; break;
                case Element.Anima:
                    backColor = TermColor.DarkBrown;
                    foreColor = TermColor.Orange; break;
                case Element.Death:
                    backColor = TermColor.Purple;
                    foreColor = TermColor.Black; break;
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
                            return new Character(Glyph.Period, TermColor.Yellow);
                        }
                        else
                        {
                            return new Character(Glyph.Period, TermColor.Gray);
                        }
                    case TileType.Wall: return new Character(Glyph.GrayFill, TermColor.Gray);
                    case TileType.LowWall: return new Character(Glyph.LightFill, TermColor.Gray);
                    case TileType.StairsUp: return new Character(Glyph.ArrowUp, TermColor.White);
                    case TileType.StairsDown: return new Character(Glyph.ArrowDown, TermColor.White);
                    case TileType.DoorOpen:
                        // hilight "temporary" lighting
                        if (tile.IsLitByThing)
                        {
                            return new Character(Glyph.Box, TermColor.LightBrown);
                        }
                        else
                        {
                            return new Character(Glyph.Box, TermColor.Brown);
                        }
                    case TileType.DoorClosed: return new Character(Glyph.Door, TermColor.LightBrown);
                    case TileType.TownPortal: return new Character(Glyph.Caret, TermColor.Green);
                    case TileType.RoofLight: return new Character(Glyph.HorizontalBarsFill, TermColor.Brown);
                    case TileType.RoofDark: return new Character(Glyph.HorizontalBarsFill, TermColor.DarkBrown);
                    case TileType.DoorStore1: return new Character(Glyph.Digit1, TermColor.Orange);
                    case TileType.DoorStore2: return new Character(Glyph.Digit2, TermColor.Red);
                    case TileType.DoorStore3: return new Character(Glyph.Digit3, TermColor.Cyan);
                    case TileType.DoorStore4: return new Character(Glyph.Digit4, TermColor.Purple);
                    case TileType.DoorStore5: return new Character(Glyph.Digit5, TermColor.Blue);
                    case TileType.DoorStore6: return new Character(Glyph.Digit6, TermColor.Green);
                }
            }
            else
            {
                switch (tile.Type)
                {
                    case TileType.Floor: return new Character(Glyph.Period, TermColor.DarkGray);
                    case TileType.Wall: return new Character(Glyph.GrayFill, TermColor.DarkGray);
                    case TileType.LowWall: return new Character(Glyph.LightFill, TermColor.DarkGray);
                    case TileType.StairsUp: return new Character(Glyph.ArrowUp, TermColor.Gray);
                    case TileType.StairsDown: return new Character(Glyph.ArrowDown, TermColor.Gray);
                    case TileType.DoorOpen: return new Character(Glyph.Box, TermColor.DarkBrown);
                    case TileType.DoorClosed: return new Character(Glyph.Door, TermColor.DarkBrown);
                    case TileType.TownPortal: return new Character(Glyph.Caret, TermColor.DarkGreen);
                    case TileType.RoofLight: return new Character(Glyph.HorizontalBarsFill, TermColor.Brown);
                    case TileType.RoofDark: return new Character(Glyph.HorizontalBarsFill, TermColor.DarkBrown);
                    case TileType.DoorStore1: return new Character(Glyph.Digit1, TermColor.Orange);
                    case TileType.DoorStore2: return new Character(Glyph.Digit2, TermColor.Red);
                    case TileType.DoorStore3: return new Character(Glyph.Digit3, TermColor.Cyan);
                    case TileType.DoorStore4: return new Character(Glyph.Digit4, TermColor.Purple);
                    case TileType.DoorStore5: return new Character(Glyph.Digit5, TermColor.Blue);
                    case TileType.DoorStore6: return new Character(Glyph.Digit6, TermColor.Green);
                }
            }

            // unknown tile type
            return new Character(Glyph.QuestionMark, TermColor.Purple);
        }

        public static Character Get(Entity entity)
        {
            Hero hero = entity as Hero;

            if (hero != null)
            {
                TermColor color = TermColor.Flesh;

                // choose a color based on condition

                // disease
                if (hero.Health.HasBonus(BonusType.Disease))
                {
                    color = TermColor.Purple;
                }

                // poison
                if (hero.Conditions.Poison.IsActive)
                {
                    color = TermColor.DarkGreen;
                }

                // near death
                if (hero.Health.Current < (hero.Health.Max / 5))
                {
                    color = TermColor.Red;
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
                c = new Character(c.Glyph, (TermColor)item.Power.Appearance, c.BackColor);
            }

            return c;
        }
    }
}
