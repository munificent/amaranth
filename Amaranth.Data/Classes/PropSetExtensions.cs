using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Amaranth.Util;

namespace Amaranth.Data
{
    public static class PropSetExtensions
    {
        public static void ToRange(this PropSet property, out int min, out int max)
        {
            Match match = sRangeRegex.Match(property.Value);

            min = Int32.Parse(match.Groups["from"].Value);
            max = Int32.Parse(match.Groups["to"].Value);
        }

        private static Regex sRangeRegex = new Regex(@"^(?<from>\d+)\s*to\s*(?<to>\d+)?$");
        //                                                      1      2          3
        // 1 match a sequence of digits
        // 2 with " to " in between
        // 3 followed by another sequence of digits
    }
}
