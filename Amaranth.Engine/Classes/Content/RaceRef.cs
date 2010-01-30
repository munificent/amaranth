using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    [Serializable]
    public class RaceRef : ContentReference<Race>
    {
        public static implicit operator RaceRef(Race race)
        {
            return new RaceRef(race.Name, race.Content);
        }

        public RaceRef(string name, Content content)
            : base(name, content.Races)
        {
        }

        protected override IEnumerable<Race> GetCollection(Content content)
        {
            return content.Races;
        }
    }
}
