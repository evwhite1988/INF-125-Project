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
        public int numberOfColumns;
        public int numberOfRows;


        public Gameboard()
        {
            numberOfColumns = Variables.columns;
            numberOfRows = Variables.rows;

            for (int y = 0; y < numberOfRows; y++)
            {
                MapRow thisRow = new MapRow();
                for (int x = 0; x < numberOfColumns; x++)
                {
                    thisRow.Columns.Add(new Cell(0));
                }
                Rows.Add(thisRow);
            }

            // Create Sample Map Data
            

            // End Create Sample Map Data
        }


        public void updateTile(Vector2 position)
        {
            int column = (int) position.X / Variables.cellWidth;
            int row = (int) position.Y / Variables.cellHeigth;

            Console.WriteLine("Position == " + position.X + ", " + position.Y);

            int tileID = Rows[row].Columns[column].TileID;
            if (tileID == 4)
            {
                Rows[row].Columns[column].TileID = 1;
            }
            else Rows[row].Columns[column].TileID = tileID + 1;

            Console.WriteLine("Cell (" + row + "," + column + ") updated");
        }

        public Cell getCell(int row, int column)
        {
            return Rows[row].Columns[column];
        }
    }



    class MapRow
    {
        public List<Cell> Columns = new List<Cell>();
    }

}
