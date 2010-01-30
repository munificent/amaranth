using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// Simple wrapper for promoting a string to a third-person noun.
    /// </summary>
    public class Noun : INoun
    {
        #region INoun Members

        public string NounText { get { return mSingular; } }
        public Person Person { get { return Person.Third; } }
        public string Pronoun { get { return "it"; } }
        public string Possessive { get { return "its"; } }

        #endregion

        public string Singular { get { return mSingular; } }
        public string Plural { get { return mPlural; } }

        public Noun(string name)
        {
            //### bob: make it support "sta[ff|ves] of fire balls" syntax like Sentence.Format allows for verbs

            // see if the string is formatted like "foo|foos" or "foo[s]"
            if (name.Contains("|"))
            {
                string[] parts = name.Split('|');

                mSingular = parts[0];
                mPlural = parts[1];
            }
            else
            {
                // parse out verbs formatted like "hit[s]" where "hit" is
                // first and second person and "hits" is third.
                Match match = sRegex.Match(name);

                string before = match.Groups["before"].Value;
                mSingular = before;
                mPlural = before;

                for (int i = 0; i < match.Groups["optional"].Captures.Count; i++)
                {
                    mPlural += match.Groups["optional"].Captures[i].Value;

                    mSingular += match.Groups["after"].Captures[i].Value;
                    mPlural += match.Groups["after"].Captures[i].Value;
                }
            }
        }

        private static Regex sRegex = new Regex(@"(?<before>[^\[]*)(\[(?<optional>[^\]]*)\](?<after>[^\[]*))*", RegexOptions.Compiled);

        private string mSingular;
        private string mPlural;
    }

    public static class Verbs
    {
        public const string Hit = "hit[s]";
    }
}
