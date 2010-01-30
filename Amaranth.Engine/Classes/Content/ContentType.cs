using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    public abstract class ContentType : INamed
    {
        public abstract string Name { get; }
        public Content Content { get; private set; }

        protected ContentType(Content content)
        {
            Content = content;
        }
    }
}
