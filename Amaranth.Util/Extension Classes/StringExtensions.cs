using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Util
{
    public static class StringExtensions
    {
        public static string FormatNames(this string format, IDictionary<string, object> properties)
        {
            string result = format;

            foreach (KeyValuePair<string, object> pair in properties)
            {
                result = result.Replace("{" + pair.Key + "}", pair.Value.ToString());
            }

            return result;
        }
    }
}
