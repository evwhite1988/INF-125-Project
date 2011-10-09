using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Tile_Engine
{
    class Gameboard
    {
        public List<MapRow> Rows = new List<MapRow>();
        public int numberOfColumns = 30;
        public int numberOfRows = 30;


        public Gameboard()
        {
            for (int y = 0; y < numberOfRows; y++)
            {
                MapRow thisRow = new MapRow();
                for (int x = 0; x < numberOfColumns; x++)
                {
                    thisRow.Columns.Add(new Cell(1));
                }
                Rows.Add(thisRow);
            }

            // Create Sample Map Data
            

            // End Create Sample Map Data
        }


        public void updateTile(Vector2 position)
        {
            int row = (int) position.X / 64;
            int column = (int)position.Y / 64;

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
        public List<Cell> Columns = new List<Cell>();
    }

}
