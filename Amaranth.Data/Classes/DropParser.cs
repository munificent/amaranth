using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;
using Amaranth.Engine;

namespace Amaranth.Data
{
    public abstract class DropParser<T>
    {
        public IDrop<T> ParseMacro(PropSet property, DropMacroCollection<T> macros)
        {
            float dummy;
            return Parse(property, macros, true, out dummy);
        }

        public IDrop<T> ParseDefinition(PropSet property, DropMacroCollection<T> macros)
        {
            float dummy;
            return Parse(property, macros, false, out dummy);
        }

        protected abstract IDrop<T> ParseDrop(string text);

        private IDrop<T> Parse(PropSet property, DropMacroCollection<T> macros)
        {
            float dummy;
            return Parse(property, macros, false, out dummy);
        }

        private IDrop<T> Parse(PropSet property, DropMacroCollection<T> macros,
            out float odds)
        {
            return Parse(property, macros, false, out odds);
        }

        private IDrop<T> Parse(PropSet property, DropMacroCollection<T> macros,
            bool defining, out float odds)
        {
            // default
            odds = 0;

            // check for a repeat
            string[] parts = property.Name.Split(' ');

            Roller repeat = null;
            bool hasOdds = false;
            if (parts.Length > 1)
            {
                // repeat (may return null)
                repeat = Roller.Parse(parts[0]);

                // odds, formatted like (12%)
                string oddsString = parts[parts.Length - 1];
                if ((oddsString.Length > 3) &&
                    (oddsString[0] == '(') &&
                    (oddsString[oddsString.Length - 2] == '%') &&
                    (oddsString[oddsString.Length - 1] == ')'))
                {
                    hasOdds = Single.TryParse(oddsString.Substring(1, oddsString.Length - 3), out odds);
                }

                // bob: hack. also allow no percent sign for "choose by level" odds, formatted like (12)
                if (!hasOdds &&
                    (oddsString.Length > 2) &&
                    (oddsString[0] == '(') &&
                    (oddsString[oddsString.Length - 1] == ')'))
                {
                    hasOdds = Single.TryParse(oddsString.Substring(1, oddsString.Length - 2), out odds);
                }
            }

            int start = (repeat != null) ? 1 : 0;
            int count = parts.Length - start - (hasOdds ? 1 : 0);

            // put the remaining item type back together
            string text = String.Join(" ", parts, start, count);

            // see what the underlying type is
            IDrop<T> drop = null;

            if (defining || (text == "any of") || (text == "drops"))
            {
                var allDrop = new AllDrop<T>();

                // recurse into the children
                foreach (PropSet child in property)
                {
                    float childOdds = 0;
                    IDrop<T> childDrop = Parse(child, macros, out childOdds);

                    allDrop.Add(childDrop, childOdds);
                }

                drop = allDrop;
            }
            else if ((text == "one of") || (text == "drops one"))
            {
                var chooseDrop = new ChooseDrop<T>();

                // recurse into the children
                foreach (PropSet child in property)
                {
                    float childOdds = 0;
                    IDrop<T> childDrop = Parse(child, macros, out childOdds);

                    chooseDrop.Add(childDrop, childOdds);
                }

                chooseDrop.FixOdds();

                drop = chooseDrop;
            }
            else if (text == "one from level")
            {
                var chooseDrop = new ChooseByLevelDrop<T>();

                // recurse into the children
                foreach (PropSet child in property)
                {
                    float childOdds = 0;
                    IDrop<T> childDrop = Parse(child, macros, out childOdds);

                    chooseDrop.Add(childDrop, childOdds);
                }

                drop = chooseDrop;
            }
            else if (text == "one up to level")
            {
                var chooseDrop = new ChooseUpToLevelDrop<T>();

                // recurse into the children
                foreach (PropSet child in property)
                {
                    float childOdds = 0;
                    IDrop<T> childDrop = Parse(child, macros, out childOdds);

                    chooseDrop.Add(childDrop, childOdds);
                }

                drop = chooseDrop;
            }
            else if (text == "one near level")
            {
                var chooseDrop = new ChooseNearLevelDrop<T>();

                // recurse into the children
                foreach (PropSet child in property)
                {
                    float childOdds = 0;
                    IDrop<T> childDrop = Parse(child, macros, out childOdds);

                    chooseDrop.Add(childDrop, childOdds);
                }

                drop = chooseDrop;
            }
            else if ((macros != null) && (macros.ContainsKey(text)))
            {
                // use the macro's drop (note: not cloned because drops are immutable)
                drop = macros[text];
            }
            else
            {
                drop = ParseDrop(text);

                if (drop == null)
                {
                    Console.WriteLine("Could not parse drop \"" + text + "\".");
                }
            }

            // wrap it in a repeater
            if (repeat != null)
            {
                drop = new RepeatDrop<T>(repeat, drop);
            }

            return drop;
        }
    }
}
