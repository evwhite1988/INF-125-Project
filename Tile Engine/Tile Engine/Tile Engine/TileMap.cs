using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Tile_Engine
{
    class TileMap
    {
        public List<MapRow> Rows = new List<MapRow>();
        public int MapWidth = 30;
        public int MapHeight = 30;


        public TileMap()
        {
            for (int y = 0; y < MapHeight; y++)
            {
                MapRow thisRow = new MapRow();
                for (int x = 0; x < MapWidth; x++)
                {
                    thisRow.Columns.Add(new MapCell(1));
                }
                Rows.Add(thisRow);
            }

            // Create Sample Map Data
            

            // End Create Sample Map Data
        }


        public void updateTile(Vector2 position)
        {
            int row = (int) position.X / Tile.cellBorder.Width;
            int column = (int)position.Y / Tile.cellBorder.Height;

            Rows[row].Columns[column].TileID = 2;
            Console.WriteLine("Cell (" + row + "," + column + ") updated");
        }

        public int getTileID(int row, int column)
        {
            return Rows[row].Columns[column].TileID;
        }
    }



    class MapRow
    {
        public List<MapCell> Columns = new List<MapCell>();
    }

}
