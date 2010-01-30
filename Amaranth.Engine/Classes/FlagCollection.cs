using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    public interface IFlagCollection : IEnumerable<string>
    {
        bool Has(string flag);
    }

    public class FlagCollection : IFlagCollection
    {
        public override string ToString()
        {
            return String.Join(" ", mFlags.ToArray());
        }

        public void Add(string flag)
        {
            // normalize the name
            flag = flag.Trim().ToLower();

            // add it if not there
            if (!Has(flag))
            {
                mFlags.Add(flag);
            }
        }

        public bool Has(string flag)
        {
            return mFlags.Contains(flag.ToLower());
        }

        #region IEnumerable<string> Members

        public IEnumerator<string> GetEnumerator()
        {
            return mFlags.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        private readonly List<string> mFlags = new List<string>();
    }

    public class MergedFlagCollection : IFlagCollection
    {
        public MergedFlagCollection(params IFlagCollection[] collections)
        {
            mCollections = collections;
        }

        public override string ToString()
        {
            return String.Join(" ", this.ToArray());
        }

        #region IFlagCollection Members

        public bool Has(string flag)
        {
            foreach (IFlagCollection collection in mCollections)
            {
                if ((collection != null) && (collection.Has(flag))) return true;
            }

            return false;
        }

        #endregion

        #region IEnumerable<string> Members

        public IEnumerator<string> GetEnumerator()
        {
            foreach (IFlagCollection collection in mCollections)
            {
                if (collection != null)
                {
                    foreach (string flag in collection)
                    {
                        yield return flag;
                    }
                }
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        private readonly IFlagCollection[] mCollections;
    }
}
