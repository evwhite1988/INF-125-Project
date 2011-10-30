using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tile_Engine
{
    static class Variables
    {
        public static int rows = 16;
        public static int columns = 16;
        public static int speed = 4;
        public static int cellWidth = 64;
        public static int cellHeigth = 64;
        public enum Direction { Up, Down, Left, Right, None }; //Directions for movement
    }
}
