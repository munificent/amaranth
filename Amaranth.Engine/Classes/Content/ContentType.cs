using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// Base class for an object representing some kind of game content.
    /// </summary>
    public abstract class ContentBase : INamed
    {
        /// <summary>
        /// Gets the name of this piece of content. The name should be unique
        /// across all objects of this content type.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the top-level <see cref="Content"/> object that owns this object.
        /// </summary>
        public Content Content { get; private set; }

        protected ContentBase(Content content)
        {
            Content = content;
        }
    }
}
