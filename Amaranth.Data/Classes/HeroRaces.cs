using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Engine;
using Amaranth.Util;

namespace Amaranth.Data
{
    public static class HeroRaces
    {
        public static void Load(string filePath, Content content)
        {
            foreach (PropSet raceProp in PropSet.FromFile(filePath))
            {
                string name = raceProp.Name;

                int[] stats = new int[6];

                stats[0] = raceProp["strength"].ToInt32();
                stats[1] = raceProp["agility"].ToInt32();
                stats[2] = raceProp["stamina"].ToInt32();
                stats[3] = raceProp["will"].ToInt32();
                stats[4] = raceProp["intellect"].ToInt32();
                stats[5] = raceProp["charisma"].ToInt32();

                content.HeroRaces.Add(new HeroRace(content, name, stats));
            }
        }
    }
}
