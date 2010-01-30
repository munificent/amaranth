using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    public class RaceChooser : LevelChooser<Race>
    {
        public IList<string> Groups
        {
            get
            {
                List<string> groups = new List<string>();

                foreach (Race race in this)
                {
                    foreach (string group in race.Groups)
                    {
                        if (!groups.Contains(group)) groups.Add(group);
                    }
                }

                return groups;
            }
        }

        public IList<Race> AllInGroup(string group)
        {
            return new List<Race>(this.Where(race => race.IsInGroup(group)));
        }

    }
}
