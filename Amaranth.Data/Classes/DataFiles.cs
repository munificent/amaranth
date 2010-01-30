using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Engine;

namespace Amaranth.Data
{
    public static class DataFiles
    {
        public static Content Load()
        {
            // only load once
            if (sContent == null)
            {
                sContent = new Content();

                // load items and powers first so that monsters can drop them
                Items.Load(@"Data\Items.txt", sContent);
                Powers.Load(@"Data\Prefix Powers.txt", sContent, true);
                Powers.Load(@"Data\Suffix Powers.txt", sContent, false);

                DropMacroCollection<Item> dropMacros = Macros.LoadItemDrops(@"Data\Drops", sContent);

                Races.Load(@"Data\Monsters.txt", dropMacros, sContent);
                Stores.Load(@"Data\Town\Stores.txt", dropMacros, sContent);

                HeroRaces.Load(@"Data\Hero\Races.txt", sContent);

                Features.Load(@"Data\Dungeon\Rooms.txt", sContent);
            }

            return sContent;
        }

        private static Content sContent;
    }
}