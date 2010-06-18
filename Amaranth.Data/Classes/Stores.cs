using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bramble.Core;

using Amaranth.Engine;

namespace Amaranth.Data
{
    public static class Stores
    {
        public static void Load(string filePath, DropMacroCollection<Item> dropMacros, Content content)
        {
            var parser = new ItemDropParser(content);

            foreach (PropertyBag storeProperty in PropertyBag.FromFile(filePath))
            {
                int depth = storeProperty.GetOrDefault("depth", 0);

                // parse the drops
                PropertyBag dropProp = storeProperty["drops"];
                IDrop<Item> drop = parser.ParseMacro(dropProp, dropMacros);

                content.Stores.Add(new StoreType(content, storeProperty.Name, depth, drop));
            }

            Console.WriteLine("Loaded " + content.Stores.Count + " stores");
        }
    }
}
