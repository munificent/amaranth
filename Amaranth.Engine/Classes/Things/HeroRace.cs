using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// One of the races a <see cref="Hero"/> can be.
    /// </summary>
    public class HeroRace : ContentBase
    {
        public override string Name { get { return mName; } }

        public int[] StatBonuses;

        public HeroRace(Content content, string name, int[] statBonuses)
            : base(content)
        {
            mName = name;
            StatBonuses = statBonuses;
        }

        private string mName;
    }
}
