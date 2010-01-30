using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    [Serializable]
    public class PowerTypeRef : ContentReference<PowerType>
    {
        public static implicit operator PowerTypeRef(PowerType powerType)
        {
            return new PowerTypeRef(powerType.Name, powerType.Content);
        }

        public PowerTypeRef(string name, Content content)
            : base(name, content.Powers)
        {
        }

        protected override IEnumerable<PowerType> GetCollection(Content content)
        {
            return content.Powers;
        }
    }
}
