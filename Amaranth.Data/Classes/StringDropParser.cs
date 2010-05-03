using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Engine;

namespace Amaranth.Data
{
    public class StringDropParser : DropParser<string>
    {
        protected override IDrop<string> ParseDrop(string text)
        {
            return new ValueDrop<string>(text);
        }
    }
}
