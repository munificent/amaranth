using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Malison.Core;

using Amaranth.Util;
using Amaranth.Engine;

namespace Amaranth.Data
{
    public static class Powers
    {
        public static void Load(string filePath, Content content, bool isPrefix)
        {
            foreach (PropSet powerProp in PropSet.FromFile(filePath))
            {
                // categories
                string[] categories = powerProp["categories"].Value.Split('|');

                // appearance
                object appearance = powerProp.GetOrDefault<object>("appearance", value => TermColors.FromName(value), null);

                // level
                int minLevel = 1;
                int maxLevel = 1;
                if (powerProp.Contains("level"))
                {
                    powerProp["level"].ToRange(out minLevel, out maxLevel);
                }

                // rarity
                int rarity = powerProp.GetOrDefault("rarity", 1);

                PowerType power = new PowerType(content, powerProp.Name, isPrefix, categories, appearance);
                content.Powers.Add(power, minLevel, maxLevel, rarity);

                // brand
                power.Element       = powerProp.GetOrDefault("element", value => (Element)Enum.Parse(typeof(Element), value, true), power.Element);

                // bonuses
                power.StrikeBonus   = powerProp.GetOrDefault("strike",  value => Roller.Parse(value), power.StrikeBonus);
                power.DamageBonus   = powerProp.GetOrDefault("damage",  value => Roller.Parse(value), power.DamageBonus);
                power.ArmorBonus    = powerProp.GetOrDefault("armor",   value => Roller.Parse(value), power.ArmorBonus);
                power.StatBonus     = powerProp.GetOrDefault("stats",   value => Roller.Parse(value), power.StatBonus);
                power.SpeedBonus    = powerProp.GetOrDefault("speed",   value => Roller.Parse(value), power.SpeedBonus);

                // flags
                foreach (PropSet childProp in powerProp)
                {
                    if (childProp.Name.StartsWith("+ "))
                    {
                        string flag = childProp.Name.Substring(2).Trim();
                        power.Flags.Add(flag);
                    }
                }
            }
        }
    }
}
