using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bramble.Core;

using Amaranth.Engine;

namespace Amaranth.Data
{
    public static class Features
    {
        public static void Load(string filePath, Content content)
        {
            foreach (PropertyBag baseProp in PropertyBag.FromFile(filePath))
            {
                var parser = new StringDropParser();
                content.SetFeatures(parser.ParseDefinition(baseProp, null));
            }

            Console.WriteLine("Loaded features");
        }
    }
}
