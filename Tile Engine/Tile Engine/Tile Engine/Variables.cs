using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tile_Engine
{
    static class Variables
    {
        public static int rows = 9;
        public static int columns = 12;
        public static int speed = 4;
        public static int cellWidth = 64;
        public static int cellHeigth = 64;
        public enum Direction { Up = 0, Down = 1, Left = 2, Right = 3, None = -1 }; //Directions for movement

        public static Direction randomDirection()
        {
            Random random = new Random();
            Direction d = (Direction)random.Next(4);
            return d;
        }
    }
}
