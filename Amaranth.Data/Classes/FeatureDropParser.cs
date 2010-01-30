using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;
using Amaranth.Engine;

namespace Amaranth.Data
{
    public class FeatureDropParser : DropParser<CreateFeature>
    {
        protected override IDrop<CreateFeature> ParseDrop(string text)
        {
            switch (text)
            {
                case "stair": return new ValueDrop<CreateFeature>(FeatureFactory.MakeStair);
                case "room": return new ValueDrop<CreateFeature>(FeatureFactory.MakeRoom);
                case "maze": return new ValueDrop<CreateFeature>(FeatureFactory.MakeMaze);
                case "pit": return new ValueDrop<CreateFeature>(FeatureFactory.MakePit);
                case "junction": return new ValueDrop<CreateFeature>(FeatureFactory.MakeJunction);
                default: throw new ArgumentException("Unknown feature \"" + text + "\"/");
            }
        }
    }
}
