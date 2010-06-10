using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bramble.Core;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// Unlike a regular <see cref="Dungeon"/>, a Maze has walls of "zero" thickness. Used as an intermediate
    /// data structure for building Dungeons. The outer walls of the Maze can be opened.
    /// </summary>
    public class Maze
    {
        public Rect Bounds { get { return new Rect(0, 0, mCells.Width - 1, mCells.Height - 1); } }

        /// <summary>
        /// Initializes a new solid (i.e. all cells closed) maze.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Maze(int width, int height)
        {
            // pad by one for the outer bottom and right walls
            mCells = new Array2D<Cell>(width + 1, height + 1);

            mCells.Fill((pos) => new Cell());
        }

        /// <summary>
        /// Implementation of the "growing tree" algorithm from here:
        /// http://www.astrolog.org/labyrnth/algrithm.htm.
        /// </summary>
        /// <remarks>
        /// This is a general algorithm, capable of creating Mazes of different textures. It requires
        /// storage up to the size of the Maze. Each time you carve a cell, add that cell to a list.
        /// Proceed by picking a cell from the list, and carving into an unmade cell next to it. If
        /// there are no unmade cells next to the current cell, remove the current cell from the list.
        /// The Maze is done when the list becomes empty. The interesting part that allows many possible
        /// textures is how you pick a cell from the list. For example, if you always pick the most
        /// recent cell added to it, this algorithm turns into the recursive backtracker. If you always
        /// pick cells at random, this will behave similarly but not exactly to Prim's algorithm. If you
        /// always pick the oldest cells added to the list, this will create Mazes with about as low a
        /// "river" factor as possible, even lower than Prim's algorithm. If you usually pick the most
        /// recent cell, but occasionally pick a random cell, the Maze will have a high "river" factor
        /// but a short direct solution. If you randomly pick among the most recent cells, the Maze will
        /// have a low "river" factor but a long windy solution.
        /// </remarks>
        public void GrowTree()
        {
            List<Vec> cells = new List<Vec>();

            // start with a random cell
            Vec pos = Rng.Vec(Bounds);

            Open(pos);
            cells.Add(pos);

            while (cells.Count > 0)
            {
                // weighting how the index is chosen here will affect the way the
                // maze looks. see the function description
                int index = Math.Abs(Rng.TriangleInt(0, cells.Count - 1));
                Vec cell = cells[index];

                // see which adjacent cells are open
                List<Direction> unmadeCells = new List<Direction>();

                if (CanCarve(cell, Direction.N)) unmadeCells.Add(Direction.N);
                if (CanCarve(cell, Direction.S)) unmadeCells.Add(Direction.S);
                if (CanCarve(cell, Direction.E)) unmadeCells.Add(Direction.E);
                if (CanCarve(cell, Direction.W)) unmadeCells.Add(Direction.W);

                if (unmadeCells.Count > 0)
                {
                    Direction direction = Rng.Item(unmadeCells);

                    Carve(cell, direction);

                    cells.Add(cell + direction);
                }
                else
                {
                    // no adjacent uncarved cells
                    cells.RemoveAt(index);
                }
            }
        }

        public void AddLoops(int chance)
        {
            if (chance > 0)
            {
                foreach (Vec cell in new Rect(0, 0, Bounds.Width - 1, Bounds.Height - 1))
                {
                    if (Rng.OneIn(chance))
                    {
                        if (IsOpen(cell) && IsOpen(cell + Direction.E))
                        {
                            Carve(cell, Direction.E);
                        }
                    }

                    if (Rng.OneIn(chance))
                    {
                        if (IsOpen(cell) && IsOpen(cell + Direction.S))
                        {
                            Carve(cell, Direction.S);
                        }
                    }
                }
            }
        }

        public void Sparsify(int sparseSteps)
        {
            for (int i = 0; i < sparseSteps; i++)
            {
                foreach (Vec cell in Bounds)
                {
                    // if it dead-ends
                    if (GetNumExits(cell) == 1)
                    {
                        // fill in the dead end
                        Fill(cell);
                    }
                }
            }
        }

        private bool IsOpen(Vec pos)
        {
            if (!Bounds.Contains(pos)) throw new ArgumentOutOfRangeException("pos");

            return mCells[pos].IsOpen;
        }

        /// <summary>
        /// Returns whether or not an opening can be carved from the given starting Cell to the adjacent Cell
        /// in the given Direction.
        /// </summary>
        /// <param name="pos">Position of starting Cell.</param>
        /// <param name="direction">Direction towards adjacent Cell to carve into.</param>
        /// <returns><c>true</c> if the starting Cell is in bounds and the destination Cell is filled (or out of bounds).</returns>
        private bool CanCarve(Vec pos, Direction direction)
        {
            // must start in bounds
            if (!Bounds.Contains(pos)) return false;

            // must end in bounds
            if (!Bounds.Contains(pos + direction)) return false;

            // destination must not be open
            if (mCells[pos + direction].IsOpen) return false;

            return true;
        }

        /// <summary>
        /// Gets the number of open walls surrounding the given Cell.
        /// </summary>
        /// <param name="pos">Position of Cell.</param>
        /// <returns>The Number of open walls surrounding the Cell.</returns>
        private int GetNumExits(Vec pos)
        {
            if (!Bounds.Contains(pos)) throw new ArgumentOutOfRangeException("pos");

            int exits = 0;

            if (mCells[pos].IsLeftWallOpen) exits++;
            if (mCells[pos].IsTopWallOpen) exits++;
            if (mCells[pos.OffsetX(1)].IsLeftWallOpen) exits++;
            if (mCells[pos.OffsetY(1)].IsTopWallOpen) exits++;

            return exits;
        }

        /// <summary>
        /// Opens the Cell at the given position. Does not open any surrounding walls.
        /// </summary>
        /// <param name="pos">Position of Cell.</param>
        private void Open(Vec pos)
        {
            if (!Bounds.Contains(pos)) throw new ArgumentOutOfRangeException("pos");

            mCells[pos].IsOpen = true;
        }

        /// <summary>
        /// Fills the Cell at the given position. Closes any surrounding walls.
        /// </summary>
        /// <param name="pos">Position of Cell.</param>
        private void Fill(Vec pos)
        {
            if (!Bounds.Contains(pos)) throw new ArgumentOutOfRangeException("pos");

            mCells[pos].IsOpen = false;
            mCells[pos].IsLeftWallOpen = false;
            mCells[pos].IsTopWallOpen = false;
            mCells[pos.OffsetX(1)].IsLeftWallOpen = false;
            mCells[pos.OffsetY(1)].IsTopWallOpen = false;
        }

        /// <summary>
        /// Carves a passage from the given starting position to the adjacent Cell in the given direction.
        /// Opens the destination Cell (if in bounds) and opens the wall between it and the starting Cell.
        /// </summary>
        /// <param name="pos">Position of starting Cell.</param>
        /// <param name="direction">Direction towards adjacent Cell to carve into.</param>
        private void Carve(Vec pos, Direction direction)
        {
            if (!Bounds.Contains(pos)) throw new ArgumentOutOfRangeException("pos");

            // open the destination
            if (Bounds.Contains(pos + direction))
            {
                mCells[pos + direction].IsOpen = true;
            }

            // cut the wall
            if (direction == Direction.N) mCells[pos].IsTopWallOpen = true;
            else if (direction == Direction.S) mCells[pos + direction].IsTopWallOpen = true;
            else if (direction == Direction.W) mCells[pos].IsLeftWallOpen = true;
            else if (direction == Direction.E) mCells[pos + direction].IsLeftWallOpen = true;
            else throw new ArgumentException("The direction must be one of N, S, E, or W.");
        }

        public void Draw(Action<Vec> carveOpening)
        {
            foreach (Vec pos in mCells.Bounds)
            {
                // valid cell in the maze

                // open the cell
                if (Bounds.Contains(pos) && mCells[pos].IsOpen)
                {
                    Vec tile = (pos * 2) + 1;
                    carveOpening(tile);
                }

                // open the left wall
                if ((pos.Y < Bounds.Height) && mCells[pos].IsLeftWallOpen)
                {
                    Vec tile = (pos * 2) + new Vec(0, 1);
                    carveOpening(tile);
                }

                // open the top wall
                if ((pos.X < Bounds.Width) && mCells[pos].IsTopWallOpen)
                {
                    Vec tile = (pos * 2) + new Vec(1, 0);
                    carveOpening(tile);
                }
            }
        }

        public void Draw(Array2D<Tile> tiles)
        {
            Draw(pos => tiles[pos].Type = TileType.Floor);
        }

        private readonly Array2D<Cell> mCells;

        private class Cell
        {
            public bool IsOpen;
            public bool IsLeftWallOpen;
            public bool IsTopWallOpen;
        }
    }
}
