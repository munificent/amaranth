using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Amaranth.Engine
{
    public class FeatureCreepGeneratorOptions
    {
        public int MaxTries { get; set; }
        public int MinimumOpenPercent { get; set; }

        // room
        public int RoomSizeMin { get; set; }
        public int RoomSizeMax { get; set; }
        public int ChanceOfRoomConnector { get; set; }

        public int MazeSizeMin { get; set; }
        public int MazeSizeMax { get; set; }

        // hall
        public int HallLengthMin { get; set; }
        public int HallLengthMax { get; set; }

        // junction
        public int ChanceOfTurn { get; set; }
        public int ChanceOfFork { get; set; }
        public int ChanceOfTee { get; set; }
        public int ChanceOfFourWay { get; set; }

        // door
        public int ChanceOfOpenDoor { get; set; }
        public int ChanceOfClosedDoor { get; set; }

        public FeatureCreepGeneratorOptions()
        {
            MaxTries = 5000;
            MinimumOpenPercent = 20;

            RoomSizeMin = 3;
            RoomSizeMax = 10;

            MazeSizeMin = 4;
            MazeSizeMax = 12;

            ChanceOfRoomConnector = 13;

            HallLengthMin = 2;
            HallLengthMax = 7;

            ChanceOfTurn = 50;
            ChanceOfFork = 20;
            ChanceOfTee = 20;
            ChanceOfFourWay = 5;

            ChanceOfOpenDoor = 20;
            ChanceOfClosedDoor = 10;
        }
    }
}
