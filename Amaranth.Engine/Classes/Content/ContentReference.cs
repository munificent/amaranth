using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// Represents a named reference to a piece of game content. Game content is read-only data that
    /// the game loads from its data files. ContentReferences let a serialized hero file reference
    /// this content without containing a copy of it.
    /// </summary>
    /// <typeparam name="T">The type of game content object being referenced.</typeparam>
    [Serializable]
    public abstract class ContentReference<T> where T : class, INamed
    {
        /// <summary>
        /// Gets the content object being referenced. Will look up the actual object the first time
        /// this is accessed.
        /// </summary>
        public T Value
        {
            get
            {
                // use the cached version if present
                if (mCache != null)
                {
                    return mCache;
                }

                T value = mCollection.Find(mName);

                // cache it
                mCache = value;

                return value;
            }
        }

        /// <summary>
        /// Gets and sets the name of the referenced content object. Setting this points the reference
        /// to a different object.
        /// </summary>
        public string Name
        {
            get { return mName; }
            set
            {
                if (mName != value)
                {
                    mName = value;

                    // clear the cache
                    mCache = null;
                }
            }
        }

        /// <summary>
        /// Initializes a new reference with the given name, and the given collection
        /// of content objects that contains the named object.
        /// </summary>
        /// <param name="name">The name of the referenced object.</param>
        /// <param name="collection">The collection containing the named object.</param>
        protected ContentReference(string name, IEnumerable<T> collection)
        {
            mName = name;
            mCollection = collection;
        }

        /// <summary>
        /// Derived classes should override this to return the appropriate subcollection 
        /// owned by <see cref="Content"/> that contains the objects that this type can
        /// refer to.
        /// </summary>
        /// <param name="content">The top-level game content object.</param>
        /// <returns>The collection within <c>content</c> that this type can refer to.</returns>
        protected abstract IEnumerable<T> GetCollection(Content content);

        [OnDeserialized]
        private void AfterDeserialize(StreamingContext context)
        {
            // during deserialization, we have access to the ambient content
            // object associated with this game, so store it now
            Content content = (Content)context.Context;
            mCollection = GetCollection(content);
        }

        private string mName;

        [NonSerialized]
        private IEnumerable<T> mCollection;

        [NonSerialized]
        private T mCache;
    }
}
