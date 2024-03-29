﻿using System;
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
        private Texture2D cellTexture;
        public bool isSpawn;
        public bool isBase;
        private int ownedBy;


        private int TileID; // -2 = hole; -1 = home; 0 = null; 1 = up; 2 = right; 3 = down; 4 = left

        private Vector2 size { get; set; } //(height, width);

        private Vector2 coordinates { get; set; } //(x-coordinate, y-coordinate)

        private Vector2 position { get; set; }// (row, column)


        public Cell(int row, int col, int tileID)
        {
            setTileID(tileID, Tile.cellBorder);
            size = new Vector2(cellHeight, cellWidth);
            position = new Vector2(row, col);
            coordinates = positionToCoord(row, col);
            ownedBy = 0;
        }

        static public Rectangle getRecTexture()
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

        public int getOwnedBy()
        {
            return ownedBy;
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

        public int getTileID()
        {
            return TileID;
        }

        public void setOwnedBy(int i)
        {
            ownedBy = i;
        }

        public void setOwnedBy(PlayerIndex i)
        {
            switch(i)
            {
                case(PlayerIndex.One):
                    ownedBy = 1;
                    break;

                case (PlayerIndex.Two):
                    ownedBy = 2;
                    break;

                case (PlayerIndex.Three):
                    ownedBy = 3;
                    break;

                case (PlayerIndex.Four):
                    ownedBy = 4;
                    break;
            }
        }

        public void setTileID(int id, Texture2D texture)
        {
            TileID = id;
            if (id == -2)
            {
                isSpawn = true;
                isBase = false;
            }
            else if (id == -1)
            {
                isSpawn = false;
                isBase = true;
            }
            else
            {
                isSpawn = false;
                isBase = false;
            }

            cellTexture = texture;
        }

        public Texture2D getTexture()
        {
            return cellTexture;
        }
    }
}
