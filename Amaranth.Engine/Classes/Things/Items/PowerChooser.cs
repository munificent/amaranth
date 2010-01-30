using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    public class PowerChooser : LevelChooser<PowerType>
    {
        public PowerType Random(int level, string category, string subcategory)
        {
            return Random(level, (power) => power.Categories.Contains(category) || power.Categories.Contains(subcategory));
        }
    }
}
