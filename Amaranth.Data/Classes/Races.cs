using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Malison.Core;

using Bramble.Core;

using Amaranth.Engine;

namespace Amaranth.Data
{
    public static class Races
    {
        public static void Load(string filePath, DropMacroCollection<Item> dropMacros, Content content)
        {
            foreach (PropertyBag raceProp in PropertyBag.FromFile(filePath))
            {
                int depth;
                int rarity;
                Race race = LoadRace(raceProp, dropMacros, content, out depth, out rarity);
                content.Races.Add(race, depth, rarity);
            }

            Console.WriteLine("Loaded " + content.Races.Count + " races");
        }

        private static Race LoadRace(PropertyBag raceProp, DropMacroCollection<Item> dropMacros, Content content,
            out int depth, out int rarity)
        {
            Character character = new Character('*', TermColor.Purple);
            
            if (raceProp.Contains("art"))
            {
                //### bob: old style color and glyph combined
                character = Character.Parse(raceProp["art"].Value);
            }
            else
            {
                // separate glyph and color
                character = new Character(
                    Character.ParseGlyph(raceProp["glyph"].Value),
                    TermColors.FromName(raceProp["color"].Value));
            }

            // depth
            depth = raceProp["depth"].ToInt32();

            // speed
            int speed = raceProp.GetOrDefault("speed", 0) + Energy.NormalSpeed;

            // health
            Roller health = Roller.Parse(raceProp["health"].Value);

            // rarity
            rarity = raceProp.GetOrDefault("rarity", 1);

            // create the race
            Race race = new Race(content, raceProp.Name, depth, character, speed, health);

            // attacks
            if (raceProp.Contains("attacks"))
            {
                foreach (PropertyBag attackProp in raceProp["attacks"])
                {
                    string[] attackParts = attackProp.Value.Split(' ');

                    // create the attack
                    Roller damage = Roller.Parse(attackParts[0]);

                    FlagCollection flags = new FlagCollection();
                    Element element = Element.Anima;

                    // add the flags or element
                    for (int i = 1; i < attackParts.Length; i++)
                    {
                        try
                        {
                            // see if the part is an element
                            element = (Element)Enum.Parse(typeof(Element), attackParts[i], true);
                        }
                        catch (ArgumentException)
                        {
                            // must be a flag
                            flags.Add(attackParts[i]);
                        }
                    }

                    //### bob: need to support different effect types
                    Attack attack = new Attack(damage, 0, 1.0f, element, attackProp.Name, EffectType.Hit, flags);

                    race.Attacks.Add(attack);
                }
            }

            // moves
            if (raceProp.Contains("moves"))
            {
                foreach (PropertyBag moveProp in raceProp["moves"])
                {
                    string moveName = moveProp.Name;

                    // if an explicit move field is provided, then the prop name is not the name of the move itself
                    if (moveProp.Contains("move"))
                    {
                        moveName = moveProp["move"].Value;
                    }

                    // parse the specific move info
                    MoveInfo info = ParseMove(moveProp);

                    Move move;

                    // construct the move
                    switch (moveName)
                    {
                        case "haste self": move = new HasteSelfMove(); break;
                        case "ball self": move = new BallSelfMove(); break;
                        case "cone": move = new ElementConeMove(); break;
                        case "breathe": move = new BreatheMove(); break;
                        case "bolt": move = new BoltMove(); break;
                        case "message": move = new MessageMove(); break;
                        case "breed": move = new BreedMove(); break;

                        default:
                            throw new Exception("Unknown move \"" + moveName + "\".");
                    }

                    move.BindInfo(info);

                    race.Moves.Add(move);
                }
            }

            // flags
            foreach (PropertyBag childProp in raceProp)
            {
                if (childProp.Name.StartsWith("+ "))
                {
                    string flag = childProp.Name.Substring(2).Trim();

                    // handle the flags
                    switch (flag)
                    {
                        case "groups":              race.SetGroupSize(GroupSize.Group); break;
                        case "packs":               race.SetGroupSize(GroupSize.Pack); break;
                        case "swarms":              race.SetGroupSize(GroupSize.Swarm); break;
                        case "hordes":              race.SetGroupSize(GroupSize.Horde); break;

                        case "very-bright":         race.SetLightRadius(2); break;
                        case "bright":              race.SetLightRadius(1); break;
                        case "glows":               race.SetLightRadius(0); break;

                        case "unmoving":            race.SetPursue(Pursue.Unmoving); break;
                        case "slightly-erratic":    race.SetPursue(Pursue.SlightlyErratically); break;
                        case "erratic":             race.SetPursue(Pursue.Erratically); break;
                        case "very-erratic":        race.SetPursue(Pursue.VeryErratically); break;

                        case "unique":              race.SetFlag(RaceFlags.Unique); break;
                        case "boss":                race.SetFlag(RaceFlags.Boss); break;
                        case "opens-doors":         race.SetFlag(RaceFlags.OpensDoors); break;

                        default: Console.WriteLine("Unknown flag \"{0}\"", flag); break;
                    }
                }
            }

            // resists
            if (raceProp.Contains("resists"))
            {
                ParseResists(raceProp["resists"].Value, race);
            }

            // drops
            if (raceProp.Contains("drops"))
            {
                var parser = new ItemDropParser(content);
                IDrop<Item> drop = parser.ParseDefinition(raceProp["drops"], dropMacros);
                race.SetDrop(drop);
            }

            // description
            if (raceProp.Contains("description"))
            {
                race.SetDescription(raceProp["description"].Value);
            }

            // groups
            if (raceProp.Contains("groups"))
            {
                race.SetGroups(raceProp["groups"].Value.Split(' '));
            }

            return race;
        }

        private static MoveInfo ParseMove(PropertyBag property)
        {
            MoveInfo info = new MoveInfo();

            // odds
            if (property.Contains("odds"))
            {
                // get the odds of performing the move (should be like "1 in 4")
                string[] oddsParts = property["odds"].Value.Split(' ');
                info.Chance = Int32.Parse(oddsParts[0]);
                info.Range = Int32.Parse(oddsParts[2]);
            }

            info.Radius     = property.GetOrDefault("radius",   info.Radius);
            info.Noun       = property.GetOrDefault("noun",     value => new Noun(value), info.Noun);
            info.Verb       = property.GetOrDefault("verb",     info.Verb);
            info.Damage     = property.GetOrDefault("damage",   value => Roller.Parse(value), info.Damage);
            info.Element    = property.GetOrDefault("element",  value => (Element)Enum.Parse(typeof(Element), value, true), info.Element);
            info.Effect     = property.GetOrDefault("effect",   value => (EffectType)Enum.Parse(typeof(EffectType), value, true), info.Effect);

            return info;
        }

        private static void ParseResists(string text, Race race)
        {
            foreach (string part in text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
            {
                RaceResist resist = RaceResist.None;
                string elementName = String.Empty;

                // note: because of the repeated characters, the order of the clauses below is important
                //       (part.StartsWith("-") will match both "-fire" and "--fire")
                if (part.StartsWith("*"))
                {
                    resist = RaceResist.Immune;
                    elementName = part.Substring(1);
                }
                else if (part.StartsWith("--"))
                {
                    resist = RaceResist.VeryWeak;
                    elementName = part.Substring(2);
                }
                else if (part.StartsWith("-"))
                {
                    resist = RaceResist.Weak;
                    elementName = part.Substring(1);
                }
                else if (part.StartsWith("++"))
                {
                    resist = RaceResist.VeryStrong;
                    elementName = part.Substring(2);
                }
                else if (part.StartsWith("+"))
                {
                    resist = RaceResist.Strong;
                    elementName = part.Substring(1);
                }
                else if (part.StartsWith("."))
                {
                    resist = RaceResist.None;
                    elementName = part.Substring(1);
                }
                else
                {
                    throw new Exception("Could not parse resist field \"" + text + "\".");
                }

                Element element = (Element)Enum.Parse(typeof(Element), elementName, true);
                race.SetResist(element, resist);
            }
        }
    }
}
