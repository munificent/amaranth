using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace Amaranth.Util
{
    /// <summary>
    /// Hierarchical property bag structure. Each PropSet is a dictionary of name/value pairs where each
    /// value can either be a string or another child PropSet. In addition, each PropSet may have a
    /// base PropSet that it will inherit (and can override) values from.
    /// </summary>
    public class PropSet : IEnumerable<PropSet>
    {
        #region IEnumerable<PropSet> Members

        public IEnumerator<PropSet> GetEnumerator()
        {
            return FlattenProperties.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        public static PropSet FromFile(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath, Encoding.ASCII);

            IEnumerable<string> included = PropSetParser.ParseIncludes(Path.GetDirectoryName(filePath), lines);
            IEnumerable<string> noComments = PropSetParser.StripComments(included);
            IEnumerable<string> noWhitespace = PropSetParser.StripEmptyLines(noComments);

            IndentationTree tree = IndentationTree.Parse(noWhitespace);

            return PropSetParser.Parse(tree);
        }

        public PropSet this[string name]
        {
            get
            {
                // try this level
                if (mChildren.Contains(name)) return mChildren[name];

                // recurse up the bases (in reverse order so that later items override previous ones)
                for (int i = mBases.Count - 1; i >= 0; i--)
                {
                    PropSet baseProp = mBases[i];
                    if (baseProp.Contains(name)) return baseProp[name];
                }

                // not found
                return null;
            }
        }

        public string Name { get { return mName; } }
        public string Value { get { return mValue; } }

        public ReadOnlyCollection<PropSet> Bases { get { return new ReadOnlyCollection<PropSet>(mBases); } }

        public int Count { get { return FlattenProperties.Count; } }

        public PropSet(NotNull<string> name, NotNull<string> value, IEnumerable<PropSet> baseProps)
        {
            mName = name;
            mValue = value;

            if (baseProps != null)
            {
                mBases.AddRange(baseProps);
            }
        }

        public PropSet(NotNull<string> name, NotNull<string> value) : this(name, value, null) { }

        public PropSet(NotNull<string> name, IEnumerable<PropSet> baseProps) : this(name, String.Empty, baseProps) { }

        public PropSet(NotNull<string> name) : this(name, String.Empty) { }

        public bool Contains(string child)
        {
            return FlattenProperties.Contains(child);
        }

        public int ToInt32()
        {
            return Int32.Parse(mValue);
        }

        public void Add(PropSet prop)
        {
            mChildren.Add(prop);
        }

        public T GetOrDefault<T>(string name, Func<string, T> converter, T defaultValue)
        {
            if (Contains(name)) return converter(this[name].Value);

            return defaultValue;
        }

        public string GetOrDefault(string name, string defaultValue)
        {
            if (Contains(name)) return this[name].Value;

            return defaultValue;
        }

        public int GetOrDefault(string name, int defaultValue)
        {
            if (Contains(name)) return this[name].ToInt32();

            return defaultValue;
        }

        private PropSetCollection FlattenProperties
        {
            get
            {
                PropSetCollection properties = new PropSetCollection();

                // start with the parent properties
                foreach (PropSet baseProp in mBases)
                {
                    foreach (PropSet child in baseProp.FlattenProperties)
                    {
                        if (properties.Contains(child.Name)) properties.Remove(child.Name);
                        properties.Add(child);
                    }
                }

                // override with the child ones
                foreach (PropSet child in mChildren)
                {
                    if (properties.Contains(child.Name)) properties.Remove(child.Name);
                    properties.Add(child);
                }

                return properties;
            }
        }

        private class PropSetCollection : KeyedCollection<string, PropSet>
        {
            protected override string GetKeyForItem(PropSet item) { return item.Name; }
        }

        private readonly List<PropSet> mBases = new List<PropSet>();

        private string mName;
        private string mValue;

        private PropSetCollection mChildren = new PropSetCollection();
    }
}
