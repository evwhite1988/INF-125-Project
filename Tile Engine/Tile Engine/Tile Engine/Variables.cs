using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tile_Engine
{
    static class Variables
    {
        public static int frameCount = 8;
        public static int rows = 9;
        public static int columns = 12;
        public static int speed = 4;
        public static int cellWidth = 64;
        public static int cellHeigth = 64;
        public enum Direction { Up = 0, Down = 1, Left = 2, Right = 3, None = -1 }; //Directions for movement

        public static int evilGnomeSpawnTime;                 //Time between evil-gnome spawns
        public static int gnomeSpawnTime;                     //Time between gnome spawns
        public static int randomGnomeSpawnTime;

        //Intervals for random time selection: To Be Adjusted
        public static int evilGnomeSpawnMin = 3000;
        public static int evilGnomeSpawnMax = 3000;
        public static int gnomeSpawnMin = 250;
        public static int gnomeSpawnMax = 250;
        public static int randomGnomeSpawnMin = 10000;
        public static int randomGnomeSpawnMax = 10000;

        public static int evilGnomeScore = -50;
        public static int gnomeScore = 10;
        public static int randScore = 100;

        public static Direction randomDirection()
        {
            Random random = new Random();
            Direction d = (Direction)random.Next(4);
            return d;
        }
    }
}
