using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using Bramble.Core;
using Malison.Core;

using Amaranth.Engine;

namespace Amaranth.Data
{
    public static class Items
    {
        public static void Load(string filePath, Content content)
        {
            foreach (PropertyBag itemProp in PropertyBag.FromFile(filePath))
            {
                string category = itemProp["category"].Value;

                string subcategory = String.Empty;
                if (itemProp.Contains("subcategory"))
                {
                    subcategory = itemProp["subcategory"].Value;
                }

                // name
                string name = itemProp.Name;

                // art
                Character character = new Character('*', TermColor.Purple);

                if (itemProp.Contains("art"))
                {
                    //### bob: old style color and glyph combined
                    character = Character.Parse(itemProp["art"].Value);
                }
                else
                {
                    // separate glyph and color
                    character = new Character(
                        Character.ParseGlyph(itemProp["glyph"].Value),
                        TermColors.FromName(itemProp["color"].Value));
                }

                // amount
                Roller amount = itemProp.GetOrDefault("amount", value => Roller.Parse(value), Roller.Fixed(1));

                // charges
                Roller charges = itemProp.GetOrDefault("charges", value => Roller.Parse(value), Roller.Fixed(0));

                // effect type
                EffectType effect = itemProp.GetOrDefault("effect", value => (EffectType)Enum.Parse(typeof(EffectType), value, true), EffectType.Hit);

                // attack
                Attack attack = null;
                if (itemProp.Contains("attack"))
                {
                    string attackText = itemProp["attack"].Value;

                    if (attackText.Contains(" "))
                    {
                        // element included
                        string[] parts = attackText.Split(' ');
                        Roller damage = Roller.Parse(parts[0]);
                        Element element = (Element)Enum.Parse(typeof(Element), parts[1], true);

                        attack = new Attack(damage, element, Verbs.Hit, effect);
                    }
                    else
                    {
                        // default element
                        Roller damage = Roller.Parse(attackText);
                        attack = new Attack(damage, Verbs.Hit, effect);
                    }
                }

                // armor
                int armor = itemProp.GetOrDefault("armor", 0);

                // use
                ItemScript useScript = itemProp.GetOrDefault("use", value => ItemScript.Create(value), null);

                // usage
                ChargeType chargeType = ChargeType.None;

                // default to single use if there is a use specified
                if (useScript != null) chargeType = ChargeType.Single;

                chargeType = itemProp.GetOrDefault("usage", value => (ChargeType)Enum.Parse(typeof(ChargeType), value, true), chargeType);

                // price
                int price = itemProp.GetOrDefault("price", 999999);

                ItemType itemType = new ItemType(content, name, category, subcategory, character, amount, charges,
                    attack, armor, chargeType, useScript, price);

                // flags
                foreach (PropertyBag childProp in itemProp)
                {
                    if (childProp.Name.StartsWith("+ "))
                    {
                        string flag = childProp.Name.Substring(2).Trim();
                        itemType.Flags.Add(flag);
                    }
                }

                // light
                if (itemProp.Contains("light"))
                {
                    int light = itemProp["light"].ToInt32();

                    itemType.SetLightRadius(light);
                }

                // target
                if (itemProp.Contains("target"))
                {
                    itemType.SetTarget((ItemTarget)Enum.Parse(typeof(ItemTarget), itemProp["target"].Value, true));
                }

                // ammunition
                if (itemProp.Contains("ammunition"))
                {
                    itemType.SetAmmunition(itemProp["ammunition"].Value);
                }

                // hit scripts
                foreach (Element element in Enum.GetValues(typeof(Element)))
                {
                    ItemScript hitScript = itemProp.GetOrDefault("on hit by " + element.ToString().ToLower(), value => ItemScript.Create(value), null);
                    itemType.SetHitScript(element, hitScript);
                }

                content.Items.Add(itemType);
            }
        }
    }
}
