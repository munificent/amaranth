using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    [Serializable]
    public abstract class HeroBehavior : Behavior
    {
        public Hero Hero { get { return mHero; } }

        public HeroBehavior(NotNull<Hero> hero)
        {
            mHero = hero;
        }

        private Hero mHero;
    }
}
