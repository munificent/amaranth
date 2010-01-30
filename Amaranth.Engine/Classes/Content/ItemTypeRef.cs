using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    [Serializable]
    public class ItemTypeRef : ContentReference<ItemType>
    {
        public static implicit operator ItemTypeRef(ItemType itemType)
        {
            return new ItemTypeRef(itemType.Name, itemType.Content);
        }

        public ItemTypeRef(string name, Content content)
            : base(name, content.Items)
        {
        }

        protected override IEnumerable<ItemType> GetCollection(Content content)
        {
            return content.Items;
        }
    }
}
