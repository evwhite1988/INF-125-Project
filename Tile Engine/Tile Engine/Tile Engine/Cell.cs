using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tile_Engine
{
    
    class Cell
    {

        static private int cellHeight = 64;
        static private int cellWidth = 64;

        public int TileID { get; set; } // 0 = null; 1 = up; 2 = right; 3 = down; 4 = left

        private Vector2 size { get; set; } //(height, width);

        private Vector2 coordinates { get; set; } //(x-coordinate, y-coordinate)

        private Vector2 position { get; set; }// (row, column)

       
        public Cell(int tileID)
        {
            TileID = tileID;

        }

        public Cell()
        {
            size = new Vector2(cellHeight, cellWidth);
        }

        public Cell(int row, int col)
        {
            size = new Vector2(cellHeight, cellWidth);
            position = new Vector2(row, col);
            coordinates = positionToCoord(row, col);
        }

        static public Rectangle getTexture()
        {
            return new Rectangle(32, 0, 32, 32);
        }

        private Vector2 positionToCoord(int row, int col)
        {
            Vector2 coord = new Vector2();
            coord.X = col * this.getWidth();
            coord.Y = row * this.getHeight();

            return coord;
        }


        public int getHeight()
        {
            return (int)size.X;
        }

        public int getWidth()
        {
            return (int)size.Y;
        }


        public int getCoordX()
        {
            return (int)coordinates.X;
        }

        public int getCoordY()
        {
            return (int)coordinates.Y;
        }


        public int getPositionX()
        {
            return (int)position.X;
        }

        public int getPositionY()
        {
            return (int)position.Y;
        }



    }
}
