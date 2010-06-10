using System;
using System.Collections.Generic;
using System.Text;

using Bramble.Core;

using Amaranth.Util;

namespace Amaranth.Engine
{
    [Serializable]
    public abstract class Thing : IPosition, INoun
    {
        internal event EventHandler LightRadiusChanged;

        public Vec Position
        {
            get { return mPos; }
            set
            {
                if (mPos != value)
                {
                    OnSetPosition(value);
                }
            }
        }

        public abstract int LightRadius { get; }

        public abstract Dungeon Dungeon { get; }

        /// <summary>
        /// Gets whether this Thing gives off light. This is <c>true</c> when the light radius
        /// is zero of greater. Note that a light radius of zero means the Thing lights its
        /// own tile, not that it has no light.
        /// </summary>
        public bool GivesOffLight { get { return LightRadius > -1; } }

        public Thing(Vec pos)
        {
            mPos = pos;
        }

        /// <summary>
        /// Gets whether this Thing can successfully occupy the given tile.
        /// </summary>
        public bool CanOccupy(Vec pos)
        {
            if (!Dungeon.Bounds.Contains(pos))
            {
                // out of the dungeon bounds
                return false;
            }

            if (!OnCanOccupy(pos, Dungeon.Tiles[pos]))
            {
                // impassable dungeon tile
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets whether this Thing can successfully move the given direction.
        /// </summary>
        public bool CanMove(Direction direction)
        {
            return CanOccupy(Position + direction);
        }

        public abstract void Hit(Action action, Hit hit);

        /// <summary>
        /// Sets the position without raising any events or doing any other processing.
        /// Useful for resetting things (such as moving the hero when entering a new floor.
        /// </summary>
        /// <param name="pos"></param>
        public void ForcePosition(Vec pos)
        {
            mPos = pos;
        }

        protected virtual void OnSetPosition(Vec pos)
        {
            mPos = pos;
        }

        protected virtual bool OnCanOccupy(Vec pos, Tile tile)
        {
            return tile.IsPassable;
        }

        protected void OnLightRadiusChanged()
        {
            if (LightRadiusChanged != null) LightRadiusChanged(this, EventArgs.Empty);
        }

        #region INoun Members

        public abstract string NounText { get; }
        public abstract Person Person { get; }
        public abstract string Pronoun { get; }
        public abstract string Possessive { get; }

        #endregion

        #region INamed Members

        public abstract string Name { get; }

        #endregion

        private Vec mPos;
    }
}
