using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Amaranth.Util;
using Amaranth.Engine;

namespace Amaranth.Data
{
    public static class Macros
    {
        /// <summary>
        /// Loads the collection of <see cref="FlagMacro"/> macros from the
        /// given file.
        /// </summary>
        /// <param name="filePath">Path to a <see cref="PropertyFile"/>
        /// containing flag macros.</param>
        /// <returns>The collection of macros.</returns>
        public static FlagMacroCollection LoadFlags(string filePath)
        {
            FlagMacroCollection collection = new FlagMacroCollection();

            PropSet root = PropSet.FromFile(filePath);

            foreach (PropSet macroProp in root)
            {
                FlagMacro macro = new FlagMacro(macroProp.Name);
                macro.Flags.AddRange(macroProp.Value.Split(' '));

                collection.Add(macro);
            }

            Console.WriteLine("Loaded " + collection.Count + " flag macros");

            return collection;
        }

        public static DropMacroCollection<Item> LoadItemDrops(string dirPath, Content content)
        {
            var parser = new ItemDropParser(content);
            return LoadDrops<Item>(dirPath, parser);
        }

        /// <summary>
        /// Loads the collection of <see cref="IDrop"/> macros from the
        /// given file.
        /// </summary>
        /// <param name="dirPath">Path to a directory with <see cref="PropertyFile">
        /// PropertyFiles</see> containing IDrop macros.</param>
        /// <returns>The collection of macros.</returns>
        public static DropMacroCollection<T> LoadDrops<T>(string dirPath, DropParser<T> parser)
        {
            DropMacroCollection<T> collection = new DropMacroCollection<T>();

            // load them
            foreach (string filePath in Directory.GetFiles(dirPath, "*.txt"))
            {
                foreach (PropSet macroProp in PropSet.FromFile(filePath))
                {
                    IDrop<T> drop = parser.ParseMacro(macroProp, collection);
                    collection.Add(macroProp.Name, drop);
                }
            }

            Console.WriteLine("Loaded " + collection.Count + " drop macros");

            return collection;
        }
    }
}
