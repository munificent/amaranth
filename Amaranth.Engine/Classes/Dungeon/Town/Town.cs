using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bramble.Core;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// <see cref="IDungeonGenerator"/> for generating the town level.
    /// </summary>
    [Serializable]
    public class Town : IDungeonGenerator
    {
        public IList<Store> Stores
        {
            get { return new List<Store>(mBuildings.Select((building) => building.Store)); }
        }

        /// <summary>
        /// Gets the <see cref="Store"/> entered through the given door <see cref="TileType"/>.
        /// </summary>
        /// <param name="doorTile">The Tile for the door.</param>
        /// <returns></returns>
        public Store GetStore(TileType doorTile)
        {
            return mBuildings.First(building => building.DoorType == doorTile).Store;
        }

        public void Init(Content content)
        {
            // add the buildings
            List<Rect> maxSizes = new List<Rect>();

            // see if it's a horizontal or vertical layout
            if (Rng.OneIn(2))
            {
                // horizontal
                Vec maxSize = new Vec(Bounds.Width / 3, Bounds.Height / 2);

                maxSizes.Add(new Rect(Bounds.Left, Bounds.Top, maxSize).Inflate(-2));
                maxSizes.Add(new Rect(Bounds.Left + maxSize.X, Bounds.Top, maxSize).Inflate(-2));
                maxSizes.Add(new Rect(Bounds.Left + maxSize.X + maxSize.X, Bounds.Top, maxSize).Inflate(-2));
                maxSizes.Add(new Rect(Bounds.Left, Bounds.Top + maxSize.Y, maxSize).Inflate(-2));
                maxSizes.Add(new Rect(Bounds.Left + maxSize.X, Bounds.Top + maxSize.Y, maxSize).Inflate(-2));
                maxSizes.Add(new Rect(Bounds.Left + maxSize.X + maxSize.X, Bounds.Top + maxSize.Y, maxSize).Inflate(-2));
            }
            else
            {
                // vertical
                Vec maxSize = new Vec(Bounds.Width / 2, Bounds.Height / 3);

                maxSizes.Add(new Rect(Bounds.Left, Bounds.Top, maxSize).Inflate(-2));
                maxSizes.Add(new Rect(Bounds.Left + maxSize.X, Bounds.Top, maxSize).Inflate(-2));
                maxSizes.Add(new Rect(Bounds.Left, Bounds.Top + maxSize.Y, maxSize).Inflate(-2));
                maxSizes.Add(new Rect(Bounds.Left + maxSize.X, Bounds.Top + maxSize.Y, maxSize).Inflate(-2));
                maxSizes.Add(new Rect(Bounds.Left, Bounds.Top + maxSize.Y + maxSize.Y, maxSize).Inflate(-2));
                maxSizes.Add(new Rect(Bounds.Left + maxSize.X, Bounds.Top + maxSize.Y + maxSize.Y, maxSize).Inflate(-2));
            }

            InitBuildings(content, maxSizes);

            // add the down stairs
            bool foundOpen = false;
            while (!foundOpen)
            {
                mStairsPos = Rng.Vec(Bounds.Inflate(-1));

                foundOpen = true;

                foreach (Building building in mBuildings)
                {
                    // see if the stairs are overlapping a building
                    if (building.Bounds.Contains(mStairsPos))
                    {
                        foundOpen = false;
                        break;
                    }
                }
            }
        }

        private Vec Size { get { return new Vec(50, 30); } }
        private Rect Bounds { get { return new Rect(Vec.One, Size); } }

        private void InitBuildings(Content content, List<Rect> maxSizes)
        {
            if (maxSizes.Count != 6) throw new ArgumentException("Should have the max sizes for 6 buildings.");

            List<TileType> buildings = new List<TileType>()
                { TileType.DoorStore1, TileType.DoorStore2, TileType.DoorStore3,
                  TileType.DoorStore4, TileType.DoorStore5, TileType.DoorStore6 };

            // make each building
            foreach (Rect maxSize in maxSizes)
            {
                // pick an actual rect within it
                Vec size = new Vec(Rng.Int(6, maxSize.Width), Rng.Int(4, maxSize.Height));
                Vec pos = maxSize.Position + Rng.Vec(maxSize.Size - size);

                Rect bounds = new Rect(pos, size);

                // pick the type
                int index = Rng.Int(buildings.Count);
                while (buildings[index] == TileType.Floor)
                {
                    index = (index + 1) % 6;
                }

                TileType type = buildings[index];
                // don't use this store again
                buildings[index] = TileType.Floor;

                Store store = new Store(content.Stores[index]);

                Vec doorPos = new Vec(Rng.IntInclusive(bounds.Left + 1, bounds.Right - 1), bounds.Bottom - 1);
                mBuildings.Add(new Building(store, type, bounds, doorPos));
            }
        }

        #region IDungeonGenerator Members

        public void Create(Dungeon dungeon, bool isDescending, int depth, object options)
        {
            // carve a "hole" for the town
            foreach (Vec pos in Bounds.Inflate(-1))
            {
                dungeon.Tiles[pos].Type = TileType.Floor;
            }

            // light it
            foreach (Vec pos in Bounds)
            {
                dungeon.Tiles[pos].LightKnown();
            }

            // add the buildings
            foreach (Building building in mBuildings)
            {
                Rect bounds = building.Bounds;

                // fill in the roof
                int center = bounds.Left + (bounds.Width / 2);
                foreach (Vec pos in bounds)
                {
                    if (pos.X < center)
                    {
                        dungeon.Tiles[pos].Type = TileType.RoofLight;
                    }
                    else
                    {
                        dungeon.Tiles[pos].Type = TileType.RoofDark;
                    }
                }

                // add the wall
                foreach (Vec pos in Rect.Row(bounds.BottomLeft - new Vec(0, 1), bounds.Width))
                {
                    dungeon.Tiles[pos].Type = TileType.Wall;
                }

                // add a door
                dungeon.Tiles[building.Door].Type = building.DoorType;
            }

            // add the down stairs
            dungeon.Tiles[mStairsPos].Type = TileType.StairsDown;

            // update the stores based on how long the hero was gone
            for (long turn = mHeroTurnsLastVisit; turn < dungeon.Game.Hero.Turns; turn += 100)
            {
                foreach (Building building in mBuildings)
                {
                    building.Store.UpdateInventory();
                }
            }

            // note when the hero arrived
            mHeroTurnsLastVisit = dungeon.Game.Hero.Turns;
        }

        #endregion

        private readonly IList<Building> mBuildings = new List<Building>();
        private Vec mStairsPos;
        private long mHeroTurnsLastVisit;
    }
}
