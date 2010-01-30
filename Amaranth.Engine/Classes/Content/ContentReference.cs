using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    [Serializable]
    public abstract class ContentReference<T> where T : class, INamed
    {
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

        protected ContentReference(string name, IEnumerable<T> collection)
        {
            mName = name;
            mCollection = collection;
        }

        protected abstract IEnumerable<T> GetCollection(Content content);

        [OnDeserialized]
        private void AfterDeserialize(StreamingContext context)
        {
            // during deserialization, we have access to the ambient content
            // object associated with this game, so store it now
            mCollection = GetCollection(Content.CurrentlyDeserializing);
        }

        private string mName;

        [NonSerialized]
        private IEnumerable<T> mCollection;

        [NonSerialized]
        private T mCache;
    }
}
