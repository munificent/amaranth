using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;
using Amaranth.Engine;

namespace Amaranth.Data
{
    public class ItemDropParser : DropParser<Item>
    {
        public ItemDropParser(Content content)
        {
            mContent = content;
        }

        protected override IDrop<Item> ParseDrop(string text)
        {
            // couldn't find it
            if (!mContent.Items.Contains(text)) return null;

            ItemType itemType = mContent.Items.Find(text);
            return new ItemDrop(itemType);
        }

        private Content mContent;
    }
}
