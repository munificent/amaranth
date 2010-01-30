using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    [Serializable]
    public class Speed : FixedStat
    {
        public static implicit operator int(Speed speed)
        {
            return speed.Current;
        }

        public Speed(int baseSpeed)
            : base(baseSpeed)
        {
        }

        protected override int GetTotalMin()
        {
            return Energy.MinSpeed;
        }

        protected override int GetTotalMax()
        {
            return Energy.MaxSpeed;
        }

        protected override int GetBaseMin()
        {
            return Energy.MinSpeed;
        }

        protected override int GetBaseMax()
        {
            return Energy.MaxSpeed;
        }
    }
}
