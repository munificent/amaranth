using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    [Serializable]
    public class StoreTypeRef : ContentReference<StoreType>
    {
        public static implicit operator StoreTypeRef(StoreType storeType)
        {
            return new StoreTypeRef(storeType.Name, storeType.Content);
        }

        public StoreTypeRef(string name, Content content)
            : base(name, content.Stores)
        {
        }

        protected override IEnumerable<StoreType> GetCollection(Content content)
        {
            return content.Stores;
        }
    }
}
