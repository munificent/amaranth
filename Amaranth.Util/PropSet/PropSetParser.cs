using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Amaranth.Util
{
    public static class PropSetParser
    {
        public static IEnumerable<string> StripEmptyLines(IEnumerable<string> lines)
        {
            if (lines == null) throw new ArgumentNullException("lines");

            return lines.Where(line => line.Trim().Length > 0);
        }

        public static IEnumerable<string> StripComments(IEnumerable<string> lines)
        {
            if (lines == null) throw new ArgumentNullException("lines");

            return lines.Select(line => sCommentRegex.Match(line).Groups["content"].Value);
        }

        public static IEnumerable<string> ParseIncludes(IEnumerable<string> lines)
        {
            //### bob: right now includes are searched for from the current working directory.
            // should be relative to the file containing the #include

            if (lines == null) throw new ArgumentNullException("lines");

            foreach (string line in lines)
            {
                Match match = sIncludeRegex.Match(line);

                if (match.Success)
                {
                    // got an include
                    string path = match.Groups["path"].Value;

                    // see if it's a dir
                    if (Directory.Exists(path))
                    {
                        foreach (string filePath in Directory.GetFiles(path))
                        {
                            string[] includeLines = File.ReadAllLines(filePath);
                            foreach (string includeLine in ParseIncludes(includeLines))
                            {
                                yield return includeLine;
                            }
                        }
                    }
                    else if (File.Exists(path))
                    {
                        // it's a file
                        string[] includeLines = File.ReadAllLines(path);
                        foreach (string includeLine in ParseIncludes(includeLines))
                        {
                            yield return includeLine;
                        }
                    }
                    else
                    {
                        // it doesn't exist, do nothing
                    }
                }
                else
                {
                    // not an include line, so just forward it
                    yield return line;
                }
            }
        }

        public static PropSet Parse(NotNull<IndentationTree> tree)
        {
            PropSet root = new PropSet(String.Empty, String.Empty);

            Stack<PropSet> parents = new Stack<PropSet>();
            parents.Push(root);

            ParseTree(tree, parents, new Stack<PropSet>());

            return root;
        }

        private static void ParseTree(IndentationTree tree, Stack<PropSet> parents, Stack<PropSet> abstractProps)
        {
            // temporary container for abstract properties. they can be
            // inherited from, but will not themselves be directly
            // added to the property tree
            abstractProps.Push(new PropSet("abstract"));

            foreach (IndentationTree child in tree.Children)
            {
                bool isAbstract = false;
                string name = parents.Peek().Count.ToString(); // default the property
                List<string> inherits = new List<string>();
                bool hasEquals = false;
                bool hasValue = false;
                string value = String.Empty;

                // parse the line
                Match match = sLineRegex.Match(child.Text);

                isAbstract = match.Groups["abstract"].Success;

                if (match.Groups["name"].Success)
                {
                    name = match.Groups["name"].Value.Trim();
                }

                if (match.Groups["inherits"].Success)
                {
                    foreach (Capture inherit in match.Groups["inherits"].Captures)
                    {
                        string inheritName = inherit.Value.Trim();

                        // if no name is given, then look to inherit a previous prop with the same name
                        if (inheritName.Length == 0)
                        {
                            inheritName = name;
                        }

                        inherits.Add(inheritName);
                    }
                }

                if (match.Groups["equals"].Success)
                {
                    hasEquals = true;
                }

                if (match.Groups["value"].Success)
                {
                    hasValue = true;
                    value = match.Groups["value"].Value.Trim();
                }

                // create the property
                if (hasEquals && hasValue)
                {
                    // fully-specified text property
                    parents.Peek().Add(new PropSet(name, value));

                    // ignore children
                }
                else if (hasEquals)
                {
                    // beginning of multi-line text property

                    // join all of the child lines together
                    foreach (IndentationTree textChild in child.Children)
                    {
                        // separate lines with a space
                        if (value != String.Empty)
                        {
                            value += " ";
                        }

                        value += textChild.Text;
                    }

                    parents.Peek().Add(new PropSet(name, value));
                }
                else
                {
                    // collection property

                    // look up the bases from this property's prior siblings
                    List<PropSet> bases = new List<PropSet>();

                    foreach (string baseName in inherits)
                    {
                        PropSet baseProp = null;

                        // walk up the concrete parent stack
                        foreach (PropSet parent in parents)
                        {
                            if (parent.Contains(baseName))
                            {
                                baseProp = parent[baseName];
                                break;
                            }
                        }

                        // if a concrete one wasn't found, look for an abstract one
                        if (baseProp == null)
                        {
                            foreach (PropSet abstractProp in abstractProps)
                            {
                                if (abstractProp.Contains(baseName))
                                {
                                    baseProp = abstractProp[baseName];
                                    break;
                                }
                            }
                        }

                        if (baseProp != null) bases.Add(baseProp);
                    }

                    PropSet prop = new PropSet(name, bases);

                    // add it to the appropriate prop
                    if (isAbstract)
                    {
                        abstractProps.Peek().Add(prop);
                    }
                    else
                    {
                        parents.Peek().Add(prop);
                    }

                    // recurse
                    parents.Push(prop);
                    ParseTree(child, parents, abstractProps);
                }
            }

            parents.Pop();
            abstractProps.Pop();
        }

        private static Regex sCommentRegex = new Regex(
            @"^(?<content>.*?)      # everything before the comment
               (?<comment>//.*)?$   # the comment",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);

        private static Regex sLineRegex = new Regex(
            @"
            ^(?<abstract>::)?             # may be abstract
             (?<name>..*?)                # name
             ((?<equals>=)(?<value>..*?)? # '= value' (value may be omitted)
             |(::(?<inherits>.*?))*       # or base props
             |                            # or nothing
             )$
            ",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);

        private static Regex sIncludeRegex = new Regex(
            @"
            ^\s*                # allow space before the include
             \#include          # the include command
             \s*                # allow space after the include
             ""(?<path>.*)""    # the include path
             $
            ",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);
    }
}
