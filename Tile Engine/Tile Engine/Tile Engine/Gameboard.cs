using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tile_Engine
{
    class Gameboard
    {
        public List<MapRow> Rows = new List<MapRow>();
        public int numberOfColumns;
        public int numberOfRows;
        public List<Vector4> wallList;

        public Gameboard()
        {
            numberOfColumns = Variables.columns;
            numberOfRows = Variables.rows;
            wallList = new List<Vector4>();
            buildBoard();
           

            // Create Sample Map Data
            

            // End Create Sample Map Data
        }

        public void buildBoard()
        {
            for (int y = 0; y < numberOfRows; y++)
            {
                MapRow thisRow = new MapRow();
                for (int x = 0; x < numberOfColumns; x++)
                {
                    if ((y == 3 && x == 4) ||
                        (y == 3 && x == 7) ||
                        (y == 5 && x == 4) ||
                        (y == 5 && x == 7))
                    {
                        thisRow.Columns.Add(new Cell(y, x, -2));
                    }
                    else if ((y == 1 && (x == 1 || x == 10)) || 
                            (y == 7 && (x == 1 || x == 10)))
                    {
                        thisRow.Columns.Add(new Cell(y, x, -1));
                        //baseList.Add(new Vector2(x, y));
                    }
                    else thisRow.Columns.Add(new Cell(y, x, 0));
                }
                Rows.Add(thisRow);
            }

            getCell(1, 1).setOwnedBy(PlayerIndex.One);
            getCell(1, 10).setOwnedBy(PlayerIndex.Two);
            getCell(7, 1).setOwnedBy(PlayerIndex.Three);
            getCell(7, 10).setOwnedBy(PlayerIndex.Four);

            // x,y,z,w column1, row1, column2, row2
            wallList.Add(new Vector4(0, 4, 0, 5));
            wallList.Add(new Vector4(1, 4, 1, 5));
            wallList.Add(new Vector4(2, 4, 2, 5));
            wallList.Add(new Vector4(9, 3, 9, 4));
            wallList.Add(new Vector4(10, 3, 10, 4));
            wallList.Add(new Vector4(11, 3, 11, 4));

            wallList.Add(new Vector4(4, 0, 5, 0));
            wallList.Add(new Vector4(4, 1, 5, 1));
            wallList.Add(new Vector4(6, 7, 7, 7));
            wallList.Add(new Vector4(6, 8, 7, 8));

        }


        public void updateTile(int column, int row, Variables.Direction direction, Texture2D texture)
        {
            if (column >= Variables.columns || row >= Variables.rows) return;
            //int column = (int) position.X / Variables.cellWidth;
            //int row = (int) position.Y / Variables.cellHeigth;

            //Console.WriteLine("Position == " + position.X + ", " + position.Y);

            Cell cell = Rows[row].Columns[column];

            if(!cell.isBase && !cell.isSpawn)
            {
                switch (direction)
                {
                    case Variables.Direction.None:
                        Rows[row].Columns[column].setTileID(0, texture);
                        break;
                    case Variables.Direction.Up:
                        Rows[row].Columns[column].setTileID(1, texture);
                        break;
                    case Variables.Direction.Right:
                        Rows[row].Columns[column].setTileID(2, texture);
                        break;
                    case Variables.Direction.Down:
                        Rows[row].Columns[column].setTileID(3, texture);
                        break;
                    case Variables.Direction.Left:
                        Rows[row].Columns[column].setTileID(4, texture);
                        break;
                        }
            }
        }

        public void updateTile(Vector2 position, Variables.Direction direction, Texture2D texture)
        {
            int column = (int) position.X / Variables.cellWidth;
            int row = (int) position.Y / Variables.cellHeigth;

            Cell cell = Rows[row].Columns[column];

            if (!cell.isBase && !cell.isSpawn)
            {
                switch (direction)
                {
                    case Variables.Direction.None:
                        Rows[row].Columns[column].setTileID(0, texture);
                        break;
                    case Variables.Direction.Up:
                        Rows[row].Columns[column].setTileID(1, texture);
                        break;
                    case Variables.Direction.Right:
                        Rows[row].Columns[column].setTileID(2, texture);
                        break;
                    case Variables.Direction.Down:
                        Rows[row].Columns[column].setTileID(3, texture);
                        break;
                    case Variables.Direction.Left:
                        Rows[row].Columns[column].setTileID(4, texture);
                        break;
                }
            }
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

        /*
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
        */

        public void randomizeSpawnLocations()
        {
            Random random = new Random();

            for (int y = 0; y < numberOfRows; y++)
            {
                for (int x = 0; x < numberOfColumns; x++)
                {
                    Cell cell = Rows[y].Columns[x];
                    if (cell.getTileID() == -2)
                        cell.setTileID(0, Tile.cellBorder);       
                }
            }

            for (int i = 0; i < 4; i++)
            {
                Cell c = Rows[random.Next(Variables.rows)].Columns[random.Next(Variables.columns)];
                if (c.getTileID() != -1)
                    c.setTileID(-2, Tile.spawn);
                else
                    --i;
            }
        }

        public Cell getBase(int player)
        {
            for (int y = 0; y < numberOfRows; y++)
            {
                for (int x = 0; x < numberOfColumns; x++)
                {
                    Cell cell = Rows[y].Columns[x];
                    if (cell.isBase && cell.getOwnedBy() == player)
                        return cell;
                }
            }
            return null;
        }

        public List<Cell> getBases()
        {
            List<Cell> bases = new List<Cell>();

            for (int y = 0; y < numberOfRows; y++)
            {
                for (int x = 0; x < numberOfColumns; x++)
                {
                    Cell cell = Rows[y].Columns[x];
                    if (cell.isBase) bases.Add(cell);
                }
            }

            return bases;
        }

    }


    class MapRow
    {
        public List<Cell> Columns = new List<Cell>();
    }

}
