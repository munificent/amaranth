using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    [Serializable]
    public class Dungeon
    {
        public readonly GameEvent<Dungeon, TileEventArgs> TileChanged = new GameEvent<Dungeon, TileEventArgs>();

        public Array2D<Tile> Tiles { get { return mTiles; } }

        public ItemCollection Items { get { return mItems; } }
        public EntityCollection Entities { get { return mEntities; } }

        public Rect Bounds { get { return new Rect(mTiles.Size); } }

        public Game Game { get { return mGame; } }

        public Dungeon(Game game, int width, int height)
        {
            mGame = game;

            mTiles = new Array2D<Tile>(width, height);

            mItems = new ItemCollection(this);
            mItems.ItemAdded.Add(Items_ItemAdded);
            mItems.ItemRemoved.Add(Items_ItemRemoved);

            mEntities = new EntityCollection(this);
            mEntities.EntityAdded.Add(Entities_EntityAdded);
            mEntities.EntityRemoved.Add(Entities_EntityRemoved);
        }

        public Dungeon(Game game) : this(game, 100, 80) { }

        public void Generate(bool isDescending, int depth)
        {
            mItems.Clear();
            mEntities.Clear();

            // fill the dungeon with default tiles
            mTiles.SetAll((pos) => new Tile(TileType.Wall));

            // generate the dungeon
            IDungeonGenerator generator;
            object options = null;

            if (depth == 0)
            {
                // town at the top
                generator = mGame.Town;
            }
            else
            {
                generator = new FeatureCreepGenerator();
                options = new FeatureCreepGeneratorOptions();
            }

            generator.Create(this, isDescending, depth, options);

            // add the hero
            mEntities.Add(mGame.Hero);

            // place the hero on the stairs
            //### bob: need to handle placing on portals
            foreach (Vec pos in Bounds)
            {
                if ((Tiles[pos].Type == TileType.StairsDown) && !isDescending)
                {
                    // place the hero on them
                    mGame.Hero.ForcePosition(pos);
                    break;
                }
                else if ((Tiles[pos].Type == TileType.StairsUp) && isDescending)
                {
                    // place the hero on them
                    mGame.Hero.ForcePosition(pos);
                    break;
                }
            }
        }

        public void SetTileType(Vec position, TileType type)
        {
            IDungeonTile tile = (IDungeonTile)Tiles[position];

            if (tile.SetTileType(type))
            {
                TileChanged.Raise(this, new TileEventArgs((Tile)tile, position));
            }
        }

        public void SetTileVisible(Vec position, bool isVisible)
        {
            IDungeonTile tile = (IDungeonTile)Tiles[position];

            if (tile.SetIsVisible(isVisible))
            {
                TileChanged.Raise(this, new TileEventArgs((Tile)tile, position));
            }
        }

        public void SetTileExplored(Vec position)
        {
            IDungeonTile tile = (IDungeonTile)Tiles[position];

            if (tile.SetExplored())
            {
                TileChanged.Raise(this, new TileEventArgs((Tile)tile, position));
            }
        }

        public void SetTileThingLit(Vec position, bool lit)
        {
            IDungeonTile tile = (IDungeonTile)Tiles[position];

            if (tile.SetTileThingLit(lit))
            {
                TileChanged.Raise(this, new TileEventArgs((Tile)tile, position));
            }
        }

        public void SetTilePermanentLit(Vec position, bool lit)
        {
            IDungeonTile tile = (IDungeonTile)Tiles[position];

            if (tile.SetTilePermanentLit(lit))
            {
                DirtyLighting();
                TileChanged.Raise(this, new TileEventArgs((Tile)tile, position));
            }
        }

        /// <summary>
        /// Hits everything on the Tile at the given position.
        /// </summary>
        /// <param name="pos">The target position to hit.</param>
        /// <param name="action">The Action that is causing the hit. The Entity associated with
        /// this Action will not be hit.</param>
        /// <param name="hit">The hit.</param>
        /// <returns><c>true</c> if an Entity was hit.</returns>
        public bool HitAt(Vec pos, Action action, Hit hit)
        {
            // hit the dungeon itself
            switch (hit.Attack.Element)
            {
                case Element.Light:
                    SetTilePermanentLit(pos, true);
                    break;

                case Element.Dark:
                    SetTilePermanentLit(pos, false);
                    break;
            }

            // hit the items
            foreach (Item item in Items.GetAllAt(pos))
            {
                item.Hit(action, hit);
            }

            // hit the entity
            bool hitEntity = false;

            Entity entity = mEntities.GetAt(pos);
            if ((entity != null) && (entity != action.Entity))
            {
                entity.Hit(action, hit);
                hitEntity = true;
            }

            return hitEntity;
        }

        /// <summary>
        /// Tries to find a tile reachable from the starting tile that has as few items as possible.
        /// </summary>
        /// <param name="startPos"></param>
        /// <returns></returns>
        public Vec GetOpenItemPosition(Vec startPos)
        {
            Vec currentPos = startPos;

            bool changed = true;
            while (changed)
            {
                int currentCount = Items.CountAt(currentPos);
                changed = false;

                // short-circuit if we hit an empty tile
                if (currentCount == 0) break;

                int found = 0;
                Vec fromPos = currentPos;
                foreach (Direction dir in Direction.Clockwise)
                {
                    Vec pos = fromPos + dir;

                    // make sure it's a valid tile
                    if (Bounds.Contains(pos) && Tiles[pos].IsPassable)
                    {
                        int count = Items.CountAt(pos);

                        // if we found a square just as good, randomly choose between
                        // it and all of the previously found ones in this loop
                        if ((count == currentCount) && (found > 0))
                        {
                            if (Rng.OneIn(found + 1))
                            {
                                // pick this new one
                                currentCount++;
                            }
                            else
                            {
                                // still consider it found even if it was skipped
                                found++;
                            }
                        }

                        if (count < currentCount)
                        {
                            found++;
                            currentPos = pos;
                            currentCount = count;
                            changed = true;
                        }
                    }
                }
            }

            return currentPos;
        }

        /*
        /// Finds a nearby position for an <see cref="Item"/> starting at the given
        /// position. Tries to spread items out (minimize floor stacking) but still
        /// make sure that all spawn items are reachable from the starting location.
        /// </summary>
        /// <param name="startPos"></param>
        public IList<Vec> FindItemPositions(Vec startPos, int numNeeded)
        {
            // get the possible positions
            IDictionary<Vec, int> reachable = GetReachablePositions(startPos, numNeeded + 2);

            // weight by the number of items and jitter a little
            IDictionary<Vec, int> weighted = new Dictionary<Vec, int>();

            foreach (KeyValuePair<Vec, int> pair in reachable)
            {
                weighted[pair.Key] = ((pair.Value + Items.GetAllAt(pair.Key).Count) * 5) + Rng.Int(7);
            }

            // sort by weight
            List<Vec> positions = new List<Vec>(weighted.Keys);
            positions.Sort((a, b) => weighted[a].CompareTo(weighted[b]));

            // add duplicates if we don't have enough
            while (positions.Count < numNeeded)
            {
                positions.Add(Rng.Item(positions));
            }

            return positions;
        }

        private IDictionary<Vec, int> GetReachablePositions(Vec startPos, int maxSteps)
        {
            // convert between flood coordinates and dungeon coordinates
            Vec toDungeon = startPos - new Vec(maxSteps, maxSteps);
            Vec toFlood = Vec.Zero - toDungeon;

            Array2D<int> flood = new Array2D<int>(maxSteps * 2 + 1, maxSteps * 2 + 1);

            // mark the invalid areas
            foreach (Vec pos in flood.Bounds)
            {
                Vec dungeon = pos + toDungeon;

                if (!Bounds.Contains(dungeon) || !Tiles[dungeon].IsPassable)
                {
                    flood[pos] = -1;
                }
            }

            // start the flood
            flood[startPos + toFlood] = 1;

            // flood
            bool changed = true;

            while (changed)
            {
                changed = false;

                foreach (Vec pos in flood.Bounds)
                {
                    // if this tile has been reached
                    if (flood[pos] > 0)
                    {
                        // reach its neighbors
                        foreach (Direction direction in Direction.Clockwise)
                        {
                            Vec neighbor = pos + direction;

                            // flood it
                            if (flood.Bounds.Contains(neighbor) && (flood[neighbor] == 0))
                            {
                                flood[neighbor] = flood[pos] + 1;
                                changed = true;
                            }
                        }
                    }
                }
            }

            // collect the reached tiles
            IDictionary<Vec, int> positions = new Dictionary<Vec, int>();

            foreach (Vec pos in flood.Bounds)
            {
                if (flood[pos] > 0)
                {
                    positions[pos + toDungeon] = flood[pos];
                }
            }

            return positions;
        }
        */

        /// <summary>
        /// Attempts to find a tile adjacent to the given starting position
        /// that does not contain any Monsters. If there are multiple available
        /// adjacent tiles, one will be chosen randomly.
        /// </summary>
        /// <param name="startPos">The position to search around.</param>
        /// <param name="pos">The chosen open tile.</param>
        /// <returns><c>true</c> if one was found.</returns>
        public bool TryFindOpenAdjacent(Vec startPos, out Vec pos)
        {
            // find all possible ones
            List<Vec> positions = new List<Vec>();

            foreach (Direction direction in Direction.Clockwise)
            {
                Vec tryPos = startPos + direction;

                // skip if out of bounds
                if (!Bounds.Contains(tryPos)) continue;

                // skip if not open
                if (!Tiles[tryPos].IsPassable) continue;

                // skip if already an entity there
                if (mEntities.GetAt(tryPos) != null) continue;

                // found a possible one
                positions.Add(tryPos);
            }

            // bail if there aren't any
            pos = startPos;
            if (positions.Count == 0) return false;

            // choose one randomly
            pos = Rng.Item(positions);
            return true;
        }

        public bool TryFindOpenTileNearest(Vec startPos, out Vec pos)
        {
            //### bob: total hack. should use flood fill
            for (int radius = 1; radius < 10; radius++)
            {
                for (int tries = 0; tries < 3 * radius * radius; tries++)
                {
                    int diameter = radius * 2 + 1;
                    Rect range = new Rect(startPos - radius, new Vec(diameter, diameter));
                    Vec tryPos = Rng.VecInclusive(range);

                    // skip if out of bounds
                    if (!Bounds.Contains(tryPos)) continue;

                    // skip if not open
                    if (!Tiles[tryPos].IsPassable) continue;

                    // skip if already an entity there
                    if (mEntities.GetAt(tryPos) != null) continue;

                    // if we got here, we found one
                    pos = tryPos;
                    return true;
                }
            }

            // if we got here, we didn't find one
            pos = Vec.Zero;
            return false;
        }

        public bool TryFindOpenTileWithin(Vec startPos, int minRadius, int maxRadius, out Vec pos)
        {
            // find all possible tiles
            List<Vec> positions = new List<Vec>();
            Rect bounds = new Rect(startPos - (Vec.One * maxRadius), Vec.One * (maxRadius + maxRadius + 1));

            foreach (Vec tryPos in bounds)
            {
                // skip if out of bounds
                if (!Bounds.Contains(tryPos)) continue;

                // skip if outside the valid radii
                int distanceSquared = (tryPos - startPos).LengthSquared;
                if ((distanceSquared < minRadius) || (distanceSquared > maxRadius)) continue;

                // skip if not open
                if (!Tiles[tryPos].IsPassable) continue;

                // skip if already an entity there
                if (mEntities.GetAt(tryPos) != null) continue;

                // if we got here, we found one
                positions.Add(tryPos);
            }

            // bail if there are none
            pos = startPos;
            if (positions.Count == 0) return false;

            // choose one randomly
            pos = Rng.Item(positions);
            return true;
        }

        public Vec RandomFloor()
        {
            Vec pos = Rng.Vec(Bounds);

            bool found = false;
            while (!found)
            {
                pos = Rng.Vec(Bounds);

                found = true;

                if (mTiles[pos].Type != TileType.Floor)
                {
                    found = false;
                }

                if (mEntities.GetAt(pos) != null)
                {
                    found = false;
                }

                if (mItems.GetAt(pos) != null)
                {
                    found = false;
                }
            }

            return pos;
        }

        /// <summary>
        /// Refreshes all of the visibility, lighting, and explored information based on the
        /// <see cref="Hero"/>'s position.
        /// </summary>
        public void RefreshView(Game game)
        {
            if (mVisibilityDirty)
            {
                // refresh visibility for the hero
                Fov.RefreshVisibility(game.Hero.Position, this);
            }

            if (mLightingDirty)
            {
                // now refresh lighting (which needs up-to-date visibility to know where the
                // hero's light is occluded)
                Lighting.Refresh(game.Hero.Position, game);

                // refresh explored (which needs both)
                Fov.RefreshExplored(game.Hero.Position, this);
            }

            mVisibilityDirty = false;
            mLightingDirty = false;
        }

        /// <summary>
        /// Marks the visibility as needing an update. Happens (for example) when the Hero moves.
        /// </summary>
        public void DirtyVisibility()
        {
            mVisibilityDirty = true;

            // changing the visibility affects the lighting too
            mLightingDirty = true;
        }

        /// <summary>
        /// Marks the lighting information as needing an update. Happens (for example) when an
        /// item that emits light is picked up.
        /// </summary>
        public void DirtyLighting()
        {
            mLightingDirty = true;
        }

        private void Items_ItemAdded(Item item, EventArgs e)
        {
            DirtyLighting();
        }

        private void Items_ItemRemoved(Item item, EventArgs e)
        {
            DirtyLighting();
        }

        private void Entities_EntityAdded(Entity entity, EventArgs e)
        {
            if (entity.GivesOffLight)
            {
                DirtyLighting();
            }
        }

        private void Entities_EntityRemoved(Entity entity, CollectionItemEventArgs<Entity> e)
        {
            if (entity.GivesOffLight)
            {
                DirtyLighting();
            }
        }

        private readonly Array2D<Tile> mTiles;
        private readonly ItemCollection mItems;
        private readonly EntityCollection mEntities;

        private Game mGame;

        private bool mVisibilityDirty;
        private bool mLightingDirty;
    }
}
