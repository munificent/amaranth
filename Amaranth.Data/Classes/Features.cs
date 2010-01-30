using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;
using Amaranth.Engine;

namespace Amaranth.Data
{
    public static class Features
    {
        public static void Load(string filePath, Content content)
        {
            foreach (PropSet baseProp in PropSet.FromFile(filePath))
            {
                var parser = new FeatureDropParser();
                content.SetFeatures(parser.ParseDefinition(baseProp, null));
            }

            Console.WriteLine("Loaded features");
        }
    }
}
