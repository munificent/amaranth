using System;
using System.Collections.Generic;
using System.Text;

using Bramble.Core;

using Amaranth.Util;

namespace Amaranth.Engine
{
    public enum EffectType
    {
        Hit,
        Stab,
        Slash,
        Arrow,
        Stone,
        Bolt,
        Ball,
        BallTrail,
        Cone,
        ConeTrail,
        Teleport
    }

    [Serializable]
    public class Effect : IPosition
    {
        public Vec Position { get { return mPos; } }
        public Direction Direction { get { return mDirection; } }
        public EffectType Type { get { return mType; } }
        public Element Element { get { return mElement; } }

        public Effect(Vec pos, Direction direction, EffectType type, Element element)
        {
            mPos = pos;
            mDirection = direction;
            mType = type;
            mElement = element;
        }

        public Effect(Vec pos, EffectType type, Element element)
            : this(pos, Direction.None, type, element)
        {
        }

        private Vec mPos;
        private Direction mDirection;
        private EffectType mType;
        private Element mElement;
    }
}
