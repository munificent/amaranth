using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bramble.Core;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// Interface used by a feature to actually apply the feature to a dungeon. The dungeon generator
    /// is expected to give an instance of this to a feature object so that it can place itself
    /// in the dungeon.
    /// </summary>
    public interface IFeatureWriter
    {
        /// <summary>
        /// Gets the bounds of the dungeon. Features must stay within this.
        /// </summary>
        Rect Bounds { get; }

        /// <summary>
        /// Gets the game content. Used by "advanced" features that place monsters or items within themselves.
        /// </summary>
        Content Content { get; }

        bool IsOpen(Rect rect, Vec? exception);
        void SetTile(Vec pos, TileType type);
        void LightRect(Rect bounds, int depth);
        void AddRoomConnector(Vec pos, Direction dir);
        void AddHallConnector(Vec pos, Direction dir);
        void AddEntity(Entity entity);

        TileType GetTile(Vec pos);

        //### bob: temp. doesn't really belong here
        FeatureCreepGeneratorOptions Options { get; }

        void Populate(Vec pos, int monsterDensity, int itemDensity, int depth);
    }
}
