using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Amaranth.Util
{
    /// <summary>
    /// Given a collection of lines of text with leading whitespace used to indicate indentation,
    /// produces a tree of IndentationTree objects corresponding to the nesting of the text.
    /// </summary>
    public class IndentationTree
    {
        public static IndentationTree Parse(IEnumerable<string> lines)
        {
            if (lines == null) throw new ArgumentNullException("lines");

            Stack<IndentationTree> stack = new Stack<IndentationTree>();

            // start with a blank root node
            IndentationTree root = new IndentationTree(-1, String.Empty);
            stack.Push(root);

            foreach (string line in lines)
            {
                Match match = sIndentRegex.Match(line);

                // create the new branch
                int indent = match.Groups["indent"].Value.Length;
                string text = match.Groups["text"].Value;

                IndentationTree tree = new IndentationTree(indent, text);

                // insert it in the tree
                if (indent > stack.Peek().Indent)
                {
                    // indented more, so push a new tree
                    stack.Peek().mChildren.Add(tree);
                    stack.Push(tree);
                }
                else
                {
                    // indented less or equal, so pop until we reach the right level
                    while (stack.Peek().Indent >= indent)
                    {
                        stack.Pop();
                    }

                    stack.Peek().mChildren.Add(tree);
                    stack.Push(tree);
                }
            }

            root.NormalizeIndentation(-1);

            return root;
        }

        public int Indent { get { return mIndent; } }
        public string Text { get { return mText; } }

        public ReadOnlyCollection<IndentationTree> Children
        {
            get { return new ReadOnlyCollection<IndentationTree>(mChildren); }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            BuildString(builder, String.Empty);

            return builder.ToString();
        }

        private IndentationTree(int indent, string text)
        {
            mIndent = indent;
            mText = text;
        }

        /// <summary>
        /// Converts indentation from the raw "number of whitespace characters" value to
        /// a simpler "number of levels of indentation" value.
        /// </summary>
        private void NormalizeIndentation(int currentIndent)
        {
            mIndent = currentIndent;

            foreach (IndentationTree child in mChildren)
            {
                child.NormalizeIndentation(currentIndent + 1);
            }
        }

        private void BuildString(StringBuilder builder, string indent)
        {
            // skip the root node
            if (mIndent > -1)
            {
                builder.Append(indent);
                builder.AppendLine(mText);

                indent += "  ";
            }

            foreach (IndentationTree child in Children)
            {
                child.BuildString(builder, indent);
            }
        }

        private static Regex sIndentRegex = new Regex(
            @"^(?<indent>\s*) # leading whitespace
               (?<text>.*)$   # everything else",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);

        private int mIndent;
        private string mText;

        private List<IndentationTree> mChildren = new List<IndentationTree>();
    }
}
