using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amaranth.Util;

namespace Amaranth.Engine
{
    public delegate bool CreateFeature(IFeatureWriter writer, Connector connector, int depth);

    public static class FeatureFactory
    {
        public static Rect MakeStartingRoom(IFeatureWriter writer, int depth)
        {
            return CreateRoom(writer, null, depth);
        }

        public static bool MakeHall(IFeatureWriter writer, Connector connector, int depth)
        {
            // create a random hall
            int length = Rng.Int(writer.Options.HallLengthMin, writer.Options.HallLengthMax);

            // check to see if we can place it
            Rect bounds = Rect.Empty;
            if (connector.Direction == Direction.N) bounds = new Rect(connector.Position.X - 1, connector.Position.Y - length, 3, length + 1);
            if (connector.Direction == Direction.S) bounds = new Rect(connector.Position.X - 1, connector.Position.Y, 3, length + 1);
            if (connector.Direction == Direction.E) bounds = new Rect(connector.Position.X, connector.Position.Y - 1, length + 1, 3);
            if (connector.Direction == Direction.W) bounds = new Rect(connector.Position.X - length, connector.Position.Y - 1, length + 1, 3);

            if (!writer.IsOpen(bounds, null)) return false;

            // make sure the end corners aren't open unless the position in front of the end is too
            // prevents cases like:
            Vec pos = connector.Position + (connector.Direction.Offset * (length + 1));

            if (!writer.Bounds.Contains(pos)) return false;
            if (!writer.Bounds.Contains(pos + connector.Direction.RotateLeft90)) return false;
            if (!writer.Bounds.Contains(pos + connector.Direction.RotateRight90)) return false;
            // ####..
            // ####..
            // ....## <- new hall ends at corner of room
            // ######
            if ((writer.GetTile(pos + connector.Direction.RotateLeft90) != TileType.Wall) &&
                (writer.GetTile(pos) == TileType.Wall)) return false;

            if ((writer.GetTile(pos + connector.Direction.RotateRight90) != TileType.Wall) &&
                (writer.GetTile(pos) == TileType.Wall)) return false;

            // place the hall
            pos = connector.Position;
            for (int i = 0; i <= length; i++)
            {
                writer.SetTile(pos, TileType.Floor);

                pos += connector.Direction;
            }

            PlaceDoor(writer, connector.Position);
            PlaceDoor(writer, connector.Position + (connector.Direction.Offset * length));

            // add the connectors
            writer.AddHallConnector(connector.Position + (connector.Direction.Offset * length),
                                    connector.Direction);

            Populate(writer, bounds, 10, 10, depth);

            return true;
        }

        public static bool MakeJunction(IFeatureWriter writer, Connector connector, int depth)
        {
            // create a random junction
            Vec center = connector.Position + connector.Direction;

            bool left = false;
            bool right = false;
            bool straight = false;

            int choice = Rng.Int(100);
            if (choice < writer.Options.ChanceOfTurn)
            {
                if (Rng.OneIn(2)) left = true; else right = true;
            }
            else if (choice - writer.Options.ChanceOfTurn < writer.Options.ChanceOfFork)
            {
                if (Rng.OneIn(2)) left = true; else right = true;
                straight = true;
            }
            else if (choice - writer.Options.ChanceOfTurn
                            - writer.Options.ChanceOfFork < writer.Options.ChanceOfTee)
            {
                left = true;
                right = true;
            }
            else if (choice - writer.Options.ChanceOfTurn
                            - writer.Options.ChanceOfFork
                            - writer.Options.ChanceOfTee < writer.Options.ChanceOfFourWay)
            {
                left = true;
                right = true;
                straight = true;
            }
            else
            {
                straight = true;
            }

            // check to see if we can place it
            Rect rect = new Rect(center.Offset(-1, -1), 3, 3);
            if (!writer.IsOpen(rect, center + connector.Direction.Rotate180)) return false;

            // place the junction
            writer.SetTile(center, TileType.Floor);

            // add the connectors
            if (left) writer.AddRoomConnector(center + connector.Direction.RotateLeft90, connector.Direction.RotateLeft90);
            if (right) writer.AddRoomConnector(center + connector.Direction.RotateRight90, connector.Direction.RotateRight90);
            if (straight) writer.AddRoomConnector(center + connector.Direction, connector.Direction);

            return true;
        }

        public static bool MakeRoom(IFeatureWriter writer, Connector connector, int depth)
        {
            return CreateRoom(writer, connector, depth) != Rect.Empty;
        }

        public static bool MakeStair(IFeatureWriter writer, Connector connector, int depth)
        {
            // check to see if we can place it
            Rect rect = new Rect(connector.Position.Offset(-1, -1), 3, 3);
            if (!writer.IsOpen(rect, connector.Position + connector.Direction.Rotate180)) return false;

            TileType type = (Rng.Int(10) < 6) ? TileType.StairsDown : TileType.StairsUp;
            writer.SetTile(connector.Position, type);

            return true;
        }

        public static bool MakeMaze(IFeatureWriter writer, Connector connector, int depth)
        {
            // in maze units (i.e. thin walls), not tiles
            int width = Rng.Int(writer.Options.MazeSizeMin, writer.Options.MazeSizeMax);
            int height = Rng.Int(writer.Options.MazeSizeMin, writer.Options.MazeSizeMax);

            int tileWidth = width * 2 + 3;
            int tileHeight = height * 2 + 3;
            Rect bounds = CreateRectRoom(writer, connector, tileWidth, tileHeight);

            // bail if we failed
            if (bounds == Rect.Empty) return false;

            // the hallway around the maze
            foreach (Vec pos in bounds.Trace())
            {
                writer.SetTile(pos, TileType.Floor);
            }

            // sometimes make the walls low
            if (Rng.OneIn(2))
            {
                foreach (Vec pos in bounds.Inflate(-1))
                {
                    writer.SetTile(pos, TileType.LowWall);
                }
            }

            // add an opening in one corner
            Vec doorway;
            switch (Rng.Int(8))
            {
                case 0: doorway = bounds.TopLeft.Offset(2, 1); break;
                case 1: doorway = bounds.TopLeft.Offset(1, 2); break;
                case 2: doorway = bounds.TopRight.Offset(-3, 1); break;
                case 3: doorway = bounds.TopRight.Offset(-2, 2); break;
                case 4: doorway = bounds.BottomRight.Offset(-3, -2); break;
                case 5: doorway = bounds.BottomRight.Offset(-2, -3); break;
                case 6: doorway = bounds.BottomLeft.Offset(2, -2); break;
                case 7: doorway = bounds.BottomLeft.Offset(1, -3); break;
                default: throw new Exception();
            }
            PlaceDoor(writer, doorway);

            // carve the maze
            Maze maze = new Maze(width, height);
            maze.GrowTree();

            Vec offset = bounds.Position.Offset(1, 1);
            maze.Draw(pos => writer.SetTile(pos + offset, TileType.Floor));

            writer.LightRect(bounds, depth);

            // populate it
            int boostedDepth = depth + Rng.Int(depth / 5) + 2;
            Populate(writer, bounds.Inflate(-2), 200, 300, boostedDepth);

            // place the connectors
            AddRoomConnectors(writer, connector, bounds);

            return true;
        }

        public static bool MakePit(IFeatureWriter writer, Connector connector, int depth)
        {
            // pits use room size right now
            int width = Rng.Int(writer.Options.RoomSizeMin, writer.Options.RoomSizeMax);
            int height = Rng.Int(writer.Options.RoomSizeMin, writer.Options.RoomSizeMax);
            Rect bounds = CreateRectRoom(writer, connector, width, height);

            // bail if we failed
            if (bounds == Rect.Empty) return false;

            // light it
            writer.LightRect(bounds, depth);

            // choose a group
            IList<Race> races = writer.Content.Races.AllInGroup(Rng.Item(writer.Content.Races.Groups));

            // make sure we've got some races that aren't too out of depth
            races = new List<Race>(races.Where(race => race.Depth <= depth + 10));
            if (races.Count == 0) return false;

            // place the room
            foreach (Vec pos in bounds)
            {
                writer.SetTile(pos, TileType.Floor);
            }

            RoomDecoration.DecorateInnerRoom(bounds, new RoomDecorator(writer,
                pos => writer.AddEntity(new Monster(pos, Rng.Item(races)))));

            return true;
        }

        private static Rect CreateRoom(IFeatureWriter writer, Connector connector, int depth)
        {
            int width = Rng.Int(6, 13);
            int height = Rng.Int(6, 13);

            Rect bounds = CreateRectRoom(writer,connector, width, height);

            // bail if we failed
            if (bounds == Rect.Empty) return bounds;

            // place the room
            foreach (Vec pos in bounds)
            {
                writer.SetTile(pos, TileType.Floor);
            }

            TileType decoration = ChooseInnerWall();

            RoomDecoration.Decorate(bounds, new FeatureFactory.RoomDecorator(writer, pos => writer.Populate(pos, 60, 200, depth + Rng.Int(depth / 10))));

            writer.LightRect(bounds, depth);

            // place the connectors
            AddRoomConnectors(writer, connector, bounds);

            Populate(writer, bounds, 20, 20, depth);

            return bounds;
        }

        private static Rect CreateRectRoom(IFeatureWriter writer, Connector connector, int width, int height)
        {
            int x = 0;
            int y = 0;

            // position the room
            if (connector == null)
            {
                // initial room, so start near center
                x = Rng.TriangleInt((writer.Bounds.Width - width) / 2, (writer.Bounds.Width - width) / 2 - 4);
                y = Rng.TriangleInt((writer.Bounds.Height - height) / 2, (writer.Bounds.Height - height) / 2 - 4);
            }
            else if (connector.Direction == Direction.N)
            {
                // above the connector
                x = Rng.Int(connector.Position.X - width + 1, connector.Position.X + 1);
                y = connector.Position.Y - height;
            }
            else if (connector.Direction == Direction.E)
            {
                // to the right of the connector
                x = connector.Position.X + 1;
                y = Rng.Int(connector.Position.Y - height + 1, connector.Position.Y + 1);
            }
            else if (connector.Direction == Direction.S)
            {
                // below the connector
                x = Rng.Int(connector.Position.X - width + 1, connector.Position.X + 1);
                y = connector.Position.Y + 1;
            }
            else if (connector.Direction == Direction.W)
            {
                // to the left of the connector
                x = connector.Position.X - width;
                y = Rng.Int(connector.Position.Y - height + 1, connector.Position.Y + 1);
            }

            Rect bounds = new Rect(x, y, width, height);

            // check to see if the room can be positioned
            if (!writer.IsOpen(bounds.Inflate(1), (connector != null) ? (Vec?)connector.Position : (Vec?)null)) return Rect.Empty;

            return bounds;
        }

        private static void AddRoomConnectors(IFeatureWriter writer, Connector connector, Rect bounds)
        {
            AddRoomEdgeConnectors(writer, connector, Rect.Row(bounds.TopLeft + Direction.N, bounds.Width), Direction.N);
            AddRoomEdgeConnectors(writer, connector, Rect.Column(bounds.TopRight, bounds.Height), Direction.E);
            AddRoomEdgeConnectors(writer, connector, Rect.Row(bounds.BottomLeft, bounds.Width), Direction.S);
            AddRoomEdgeConnectors(writer, connector, Rect.Column(bounds.TopLeft + Direction.W, bounds.Height), Direction.W);
        }

        private static void AddRoomEdgeConnectors(IFeatureWriter writer, Connector connector, Rect edge, Direction dir)
        {
            bool skip = Rng.OneIn(2);

            foreach (Vec pos in edge)
            {
                // don't place connectors close to the incoming connector
                if (connector != null)
                {
                    if (Vec.IsDistanceWithin(connector.Position, pos, 1)) continue;
                }

                if (!skip && (Rng.Int(100) < writer.Options.ChanceOfRoomConnector))
                {
                    writer.AddRoomConnector(pos, dir);
                    skip = true;
                }
                else
                {
                    skip = false;
                }
            }
        }

        private static void PlaceDoor(IFeatureWriter writer, Vec pos)
        {
            int choice = Rng.Int(100);
            if (choice < writer.Options.ChanceOfOpenDoor)
            {
                writer.SetTile(pos, TileType.DoorOpen);
            }
            else if (choice - writer.Options.ChanceOfOpenDoor < writer.Options.ChanceOfClosedDoor)
            {
                writer.SetTile(pos, TileType.DoorClosed);
            }
            else
            {
                writer.SetTile(pos, TileType.Floor);
            }
            //### bob: add locked and secret doors
        }

        private static void Populate(IFeatureWriter writer, Rect bounds, int monsterDensity, int itemDensity, int depth)
        {
            // test every open tile
            foreach (Vec pos in bounds)
            {
                writer.Populate(pos, monsterDensity, itemDensity, depth);
            }
        }

        private static TileType ChooseInnerWall()
        {
            return Rng.Item(new TileType[] { TileType.Wall, TileType.LowWall });
        }

        public class RoomDecorator : IRoomDecorator
        {
            public RoomDecorator(IFeatureWriter writer, Action<Vec> insideRoom)
            {
                mInsideRoom = insideRoom;
                mWriter = writer;
                mDecoration = ChooseInnerWall();
            }

            public void AddDecoration(Vec pos)
            {
                mWriter.SetTile(pos, mDecoration);
            }

            public void AddInsideRoom(Vec pos)
            {
                mInsideRoom(pos);
            }

            public void AddDoor(Vec pos)
            {
                PlaceDoor(mWriter, pos);
            }

            private IFeatureWriter mWriter;
            private Action<Vec> mInsideRoom;
            private TileType mDecoration;
        }
    }
}
