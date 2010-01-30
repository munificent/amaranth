using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    public static class RoomDecoration
    {
        /// <summary>
        /// Randomly chooses and applies a room decoration.
        /// </summary>
        /// <param name="room">Bounds of the room to decorate.</param>
        /// <param name="addDecoration">Callback to add a decoration at the given location.</param>
        public static void Decorate(Rect room, IRoomDecorator decorator)
        {
            // decorate it
            switch (Rng.Int(7))
            {
                case 0: DecorateVerticalWall(room, decorator); break;
                case 1: DecorateHorizontalWall(room, decorator); break;
                case 2: DecorateInnerPillars(room, decorator); break;
                case 3: DecorateOuterPillars(room, decorator); break;
                case 4: DecorateInnerRoom(room, decorator); break;
            }
        }

        public static void DecorateVerticalWall(Rect room, IRoomDecorator decorator)
        {
            int x = Rng.Int(room.Left + 1, room.Right - 1);

            foreach (Vec pos in Rect.Column(x, room.Top + 1, room.Height - 2))
            {
                decorator.AddDecoration(pos);
            }
        }

        public static void DecorateHorizontalWall(Rect room, IRoomDecorator decorator)
        {
            int y = Rng.Int(room.Top + 1, room.Bottom - 1);

            foreach (Vec pos in Rect.Row(room.Left + 1, y, room.Width - 2))
            {
                decorator.AddDecoration(pos);
            }
        }

        public static void DecorateInnerPillars(Rect room, IRoomDecorator decorator)
        {
            // must be odd sized and bigger than 3x3 to fit the pillars symmetrically
            if ((room.Width > 3) && (room.Height > 3) &&
                (room.Width % 2 == 1) && (room.Height % 2 == 1))
            {
                foreach (Vec pos in room.Inflate(-2))
                {
                    if (((pos.X - room.X) % 2 == 0) && ((pos.Y - room.Y) % 2 == 0))
                    {
                        decorator.AddDecoration(pos);
                    }
                }
            }
        }

        public static void DecorateOuterPillars(Rect room, IRoomDecorator decorator)
        {
            // must be odd sized and bigger than 3x3 to fit the pillars symmetrically
            if ((room.Width > 3) && (room.Height > 3) &&
                (room.Width % 2 == 1) && (room.Height % 2 == 1))
            {
                // trace with pillars
                foreach (Vec pos in room.Inflate(-1).Trace())
                {
                    if ((pos.X + pos.Y) % 2 == 0)
                    {
                        decorator.AddDecoration(pos);
                    }
                }
            }
        }

        /// <summary>
        /// Creates an inner room with a single entrace inside the room.
        /// </summary>
        /// <example>
        /// ##########
        /// #        #
        /// # ****** #
        /// # *    * #
        /// # *    * #
        /// #      * #
        /// # *    * #
        /// # ****** #
        /// #        #
        /// ##########
        /// </example>
        public static void DecorateInnerRoom(Rect room, IRoomDecorator decorator)
        {
            if ((room.Width > 4) && (room.Height > 4))
            {
                foreach (Vec pos in room.Inflate(-1).Trace())
                {
                    decorator.AddDecoration(pos);
                }

                // populate the inside
                foreach (Vec pos in room.Inflate(-2))
                {
                    decorator.AddInsideRoom(pos);
                }

                // add an opening
                Vec opening = Vec.Zero;
                switch (Rng.Int(4))
                {
                    case 0: opening = new Vec(Rng.Int(room.Left + 2, room.Right - 2), room.Y + 1); break;
                    case 1: opening = new Vec(room.X + 1, Rng.Int(room.Top + 2, room.Bottom - 2)); break;
                    case 2: opening = new Vec(Rng.Int(room.Left + 2, room.Right - 2), room.Bottom - 2); break;
                    case 3: opening = new Vec(room.Right - 2, Rng.Int(room.Top + 2, room.Bottom - 2)); break;
                }
                decorator.AddDoor(opening);
            }
        }
    }
}
