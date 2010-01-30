using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Amaranth.Data
{
    /// <summary>
    /// Represents a named macro that expands to a collection of flags.
    /// </summary>
    public class FlagMacro
    {
        public string Name;
        public readonly List<string> Flags;

        public FlagMacro(string name)
        {
            Name = name;
            Flags = new List<string>();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(Name);
            builder.Append(" = ");
            builder.Append(String.Join(" ", Flags.ToArray()));

            return builder.ToString();
        }
    }

    public class FlagMacroCollection : KeyedCollection<string, FlagMacro>
    {
        public IEnumerable<string> Expand(string macro)
        {
            List<string> flags = new List<string>();

            if (Contains(macro))
            {
                // macro, so recursively expand
                Expand(macro, flags);
            }
            else
            {
                // not a macro, so just return itself
                flags.Add(macro);
            }
            return flags;
        }

        protected override string GetKeyForItem(FlagMacro item)
        {
            return item.Name;
        }

        private void Expand(string macro, List<string> flags)
        {
            foreach (string flag in this[macro].Flags)
            {
                if (!flags.Contains(flag))
                {
                    // see if the expanded flag is itself a macro
                    if (Contains(flag))
                    {
                        // recurse into the macro
                        Expand(flag, flags);
                    }
                    else
                    {
                        // not a macro, just add it
                        flags.Add(flag);
                    }
                }
            }
        }
    }
}
