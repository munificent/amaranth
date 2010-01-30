using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Amaranth.Terminals
{
    public class ColorPair
    {
        public Color Fore;
        public Color Back;

        public ColorPair(Color fore, Color back)
        {
            Fore = fore;
            Back = back;
        }
    }
}
