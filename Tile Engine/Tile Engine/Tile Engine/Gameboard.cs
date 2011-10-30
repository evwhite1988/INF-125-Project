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
                    if ((y == 2 && x == 2) || 
                        (y == 2 && x == 13) || 
                        (y == 13 && x == 2) || 
                        (y == 13 && x == 13))
                    {
                        thisRow.Columns.Add(new Cell(-2));
                    }
                    else if ((y == 5 && (x == 5 || x == 10)) || (y == 10 && (x == 5 || x == 10)))
                    {
                        thisRow.Columns.Add(new Cell(-1));
                    }
                    else thisRow.Columns.Add(new Cell(0));
                }
                Rows.Add(thisRow);
            }

            // Create Sample Map Data
            

            // End Create Sample Map Data
        }


        public void updateTile(int column, int row, Variables.Direction direction)
        {
            //int column = (int) position.X / Variables.cellWidth;
            //int row = (int) position.Y / Variables.cellHeigth;

            //Console.WriteLine("Position == " + position.X + ", " + position.Y);

            Cell cell = Rows[row].Columns[column];

            if(!cell.isBase && !cell.isSpawn)
            {
                switch (direction)
                {
                    case Variables.Direction.None:
                        Rows[row].Columns[column].setTileID(0);
                        break;
                    case Variables.Direction.Up:
                        Rows[row].Columns[column].setTileID(1);
                        break;
                    case Variables.Direction.Right:
                        Rows[row].Columns[column].setTileID(2);
                        break;
                    case Variables.Direction.Down:
                        Rows[row].Columns[column].setTileID(3);
                        break;
                    case Variables.Direction.Left:
                        Rows[row].Columns[column].setTileID(4);
                        break;
                        }
            }
        }

        public void updateTile(Vector2 position, Variables.Direction direction)
        {
            int column = (int) position.X / Variables.cellWidth;
            int row = (int) position.Y / Variables.cellHeigth;

            Console.WriteLine("Position == " + position.X + ", " + position.Y);

            Cell cell = Rows[row].Columns[column];

            if (!cell.isBase && !cell.isSpawn)
            {
                switch (direction)
                {
                    case Variables.Direction.None:
                        Rows[row].Columns[column].setTileID(0);
                        break;
                    case Variables.Direction.Up:
                        Rows[row].Columns[column].setTileID(1);
                        break;
                    case Variables.Direction.Right:
                        Rows[row].Columns[column].setTileID(2);
                        break;
                    case Variables.Direction.Down:
                        Rows[row].Columns[column].setTileID(3);
                        break;
                    case Variables.Direction.Left:
                        Rows[row].Columns[column].setTileID(4);
                        break;
                }
            }
        }

        public void updateTile(Vector2 position)
        {
            int column = (int) position.X / Variables.cellWidth;
            int row = (int) position.Y / Variables.cellHeigth;

            Cell cell = Rows[row].Columns[column];

            int tileID = cell.getTileID();
            if (tileID == 4)
            {
                Rows[row].Columns[column].setTileID(1);
            }
            else Rows[row].Columns[column].setTileID(tileID + 1);
        }

        public Cell getCell(int row, int column)
        {
            return Rows[row].Columns[column];
        }


        public List<Vector2> getSpawnPoints()
        {
            List<Vector2> spawnList = new List<Vector2>();
            for (int y = 0; y < numberOfRows; y++)
            {
                for (int x = 0; x < numberOfColumns; x++)
                {
                    Cell cell = Rows[y].Columns[x];
                    if (cell.isSpawn)
                    {
                        spawnList.Add(new Vector2(x, y));
                    }
                }
            }

            return spawnList;
        }

        public List<Vector2> getBases()
        {
            List<Vector2> baseList = new List<Vector2>();
            for (int y = 0; y < numberOfRows; y++)
            {
                for (int x = 0; x < numberOfColumns; x++)
                {
                    Cell cell = Rows[y].Columns[x];
                    if (cell.isBase)
                    {
                        baseList.Add(new Vector2(x, y));
                    }
                }
            }

            return baseList;
        }
    }


    class MapRow
    {
        public List<Cell> Columns = new List<Cell>();
    }

}
