using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    /// <summary>
    /// Calculates the Hero's field of view of the dungeon.
    /// </summary>
    public class Fov
    {
        public const int MaxDistance = 26;	// this will need to change if the overhead view size changes

        /// <summary>
        /// Gets the maximum bounds of a field-of-view centered on the given position.
        /// Nothing outside this boundary will be updated.
        /// </summary>
        public static Rect GetBounds(Vec position)
        {
            return new Rect(position - MaxDistance, new Vec(MaxDistance * 2 + 1, MaxDistance * 2 + 1));
        }

        /// <summary>
        /// Updates the visible flags in the dungeon given the hero position.
        /// </summary>
	    public static void RefreshVisibility(Vec position, Dungeon dungeon)
	    {
		    // sweep through the octants
		    for (int octant = 0; octant < 8; octant++)
		    {
                RefreshOctant(position, octant, dungeon);
		    }
    		
		    // the starting position is always visible
            dungeon.SetTileVisible(position, true);
	    }

        /// <summary>
        /// Updates the explored flag of any tiles newly visible based on an FOV centered
        /// around the given position. This should only be called after the lighting
        /// information has been refreshed.
        /// </summary>
        public static void RefreshExplored(Vec position, Dungeon dungeon)
        {
            // figure out which ones need to be looked at
            Rect bounds = new Rect(position - MaxDistance, new Vec(MaxDistance * 2 + 1, MaxDistance * 2 + 1));

            // stay in bounds
            bounds = bounds.Intersect(dungeon.Bounds);

            foreach (Vec pos in bounds)
            {
                Tile tile = dungeon.Tiles[pos];

                if (tile.IsVisible && tile.IsLit && !tile.IsExplored)
                {
                    dungeon.SetTileExplored(pos);
                }
            }
        }

        private static void RefreshOctant(Vec start, int octant, Dungeon dungeon)
	    {
            Vec rowInc = Vec.Zero;
            Vec colInc = Vec.Zero;
    		
		    // figure out which direction to increment based on the octant
		    // octant 0 starts at 12 - 2 o'clock, and octants proceed clockwise from there
		    switch (octant)
		    {
		        case 0: rowInc.Y = -1; colInc.X =  1; break;
		        case 1: rowInc.X =  1; colInc.Y = -1; break;
		        case 2: rowInc.X =  1; colInc.Y =  1; break;
		        case 3: rowInc.Y =  1; colInc.X =  1; break;
		        case 4: rowInc.Y =  1; colInc.X = -1; break;
		        case 5: rowInc.X = -1; colInc.Y =  1; break;
		        case 6: rowInc.X = -1; colInc.Y = -1; break;
		        case 7: rowInc.Y = -1; colInc.X = -1; break;
		    }

            sShadows.Clear();

            // cache for performance
            Rect bounds = dungeon.Bounds;
            bool fullShadow = false;

		    // sweep through the rows ('rows' may be vertical or horizontal based on the incrementors)
            // start at row 1 to skip the center position
		    for (int row = 1; row < MaxDistance; row++)
		    {
                Vec pos = start + (rowInc * row);

                // if we've traversed out of bounds, bail
                // note: this improves performance, but works on the assumption that the starting
                // tile of the fov is in bounds
                if (!bounds.Contains(pos)) break;

			    for (int col = 0; col <= row; col++)
			    {
                    bool blocksLight = false;
                    bool isVisible = false;
                    Shadow projection = null;

                    // if we know the entire row is in shadow, we don't need to be more specific
                    if (!fullShadow)
                    {
                        blocksLight = !dungeon.Tiles[pos].IsTransparent;
                        projection = GetProjection(col, row);
                        isVisible = !IsInShadow(projection);
                    }

				    // set the visibility of this tile
                    dungeon.SetTileVisible(pos, isVisible);
					
				    // add any opaque tiles to the shadow map
                    if (blocksLight)
				    {
                        fullShadow = AddShadow(projection);
				    }
				
				    // move to the next column
                    pos += colInc;

                    // if we've traversed out of bounds, bail on this row
                    // note: this improves performance, but works on the assumption that the starting
                    // tile of the fov is in bounds
                    if (!bounds.Contains(pos)) break;
                }
		    }
	    }

        /// <summary>
        /// Creates a <see cref="Shadow"/> that corresponds to the projected silhouette
        /// of the given tile. This is used both to determine visibility (if any of
        /// the projection is visible, the tile is) and to add the tile to the shadow
        /// map.
        /// </summary>
        private static Shadow GetProjection(int col, int row)
        {
            // the bottom edge of row 0 is 1 wide
            float rowBottomWidth = row + 1;

            // the top edge of row 0 is 2 wide
            float rowTopWidth = row + 2;

            // unify the bottom and top edges of the tile
            float start = Math.Min(col / rowBottomWidth, col / rowTopWidth);
            float end   = Math.Max((col + 1.0f) / rowBottomWidth, (col + 1.0f) / rowTopWidth);

            return new Shadow(start, end);
        }

        private static bool IsInShadow(Shadow projection)
        {
            // optimization note: doing an explicit foreach here is
            // faster than sShadows.Any((shadow) => shadow.Contains(projection));

            // check the shadow list
            foreach (var shadow in sShadows)
            {
                if (shadow.Contains(projection)) return true;
            }

            return false;
        }

	    private static bool AddShadow(Shadow shadow)
	    {
		    int index = 0;
		    for (index = 0; index < sShadows.Count; index++)
		    {
			    // see if we are at the insertion point for this shadow
                if (sShadows[index].Start > shadow.Start)
			    {
				    // break out and handle inserting below
				    break;
			    }
		    }

		    // the new shadow is going here. see if it overlaps the previous or next shadow
		    bool overlapsPrev = false;
		    bool overlapsNext = false;
    		
		    if ((index > 0) && (sShadows[index - 1].End > shadow.Start))
		    {
			    overlapsPrev = true;
		    }

            if ((index < sShadows.Count) && (sShadows[index].Start < shadow.End))
		    {
			    overlapsNext = true;
		    }
    		
		    // insert and unify with overlapping shadows
		    if (overlapsNext)
		    {
			    if (overlapsPrev)
			    {
				    // overlaps both, so unify one and delete the other
                    sShadows[index - 1].Unify(shadow.Start, sShadows[index].End);
                    sShadows.RemoveAt(index);
			    }
			    else
			    {
				    // just overlaps the next shadow, so unify it with that
                    sShadows[index].Unify(shadow.Start, shadow.End);
			    }
		    }
		    else
		    {
			    if (overlapsPrev)
			    {
				    // just overlaps the previous shadow, so unify it with that
                    sShadows[index - 1].Unify(shadow.Start, shadow.End);
			    }
			    else
			    {
				    // does not overlap anything, so insert
                    sShadows.Insert(index, shadow);
			    }
		    }

            // see if we are now shadowing everything
            return (sShadows.Count == 1) && (sShadows[0].Start == 0) && (sShadows[0].End == 1.0f);
	    }

        static Fov()
        {
            sShadows = new List<Shadow>();
        }

        private static List<Shadow> sShadows;
    	
        /// <summary>
        /// Represents the 1D projection of a 2D shadow onto a normalized line. In other words,
        /// a range from 0.0 to 1.0.
        /// </summary>
	    private class Shadow
	    {
            public float Start { get { return mStart; } }
            public float End { get { return mEnd; } }

            public Shadow(float start, float end)
            {
                mStart = start;
                mEnd = end;
            }

            public override string ToString()
            {
                return "(" + mStart + "-" + mEnd + ")";
            }

            public bool Contains(Shadow projection)
            {
                return (mStart <= projection.Start) && (mEnd >= projection.End);
            }
    		
		    public void Unify(float start, float end)
		    {
			    // see if the shadow overlaps to the right
			    if (start <= mEnd)
			    {
				    mEnd = Math.Max(mEnd, end);
			    }
    			
			    // see if the shadow overlaps to the left
			    if (mStart <= end)
			    {
				    mStart = Math.Min(mStart, start);
			    }
		    }

		    private float mStart;
            private float mEnd;
        }
    }
}
