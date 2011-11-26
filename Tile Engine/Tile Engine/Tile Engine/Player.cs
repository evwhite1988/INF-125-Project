using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace Tile_Engine
{
    class Player
    {
        bool rdy = true;
        public int points;                     //player's current points
        public int MAX_ARROWS = 4;
        public GameSprite scoreBoard;         //player's personal scoreBoard
        public GameSprite playerBase;
        private static Vector2 OFFSET = new Vector2(80, 70);    //offset used when alligning the point string to the scoreBoard
        public Cursor cursor;                          //Cursor Sprite Object
        public PlayerIndex index;
        public List<Cell> p_arrows;            //arrows owned by the player
        Texture2D[] arrows;
        Texture2D s_Board;

        //CONSTRUCTOR
        public Player(Texture2D p_Base, Vector2 p_coord, Cursor cursor, PlayerIndex index)
        {
            //Set player index
            this.index = index;

            //set player cursor
            this.cursor = cursor;

            //intialize points to zero
            points = 0;

            //initialize the player's home base
            playerBase = new GameSprite(p_Base, 1, 1);
            playerBase.coord = p_coord;
            playerBase.spriteWidth = 64;
            playerBase.spriteHeight = 64;

            //initialize the player's list of arrows
            p_arrows = new List<Cell>();
            p_arrows.Capacity = MAX_ARROWS;
            arrows = new Texture2D[4];
        }

        public void LoadContent(ContentManager content, GraphicsDeviceManager graphics)
        {
            arrows[0] = content.Load<Texture2D>("arrow_down_" + ((int)index + 1));
            arrows[1] = content.Load<Texture2D>("arrow_left_" + ((int)index + 1));
            arrows[2] = content.Load<Texture2D>("arrow_right_" + ((int)index + 1));
            arrows[3] = content.Load<Texture2D>("arrow_up_" + ((int)index + 1));
            s_Board = content.Load<Texture2D>("player" + ((int)index + 1));

            Console.WriteLine((int)index + 1);

            //intialize the player score board
            scoreBoard = new GameSprite(s_Board, 1, 1);
            scoreBoard.coord = new Vector2(s_Board.Width * (int)index, graphics.PreferredBackBufferHeight);
            scoreBoard.spriteWidth = s_Board.Width;
            scoreBoard.spriteHeight = s_Board.Height;
        }


        // Method to add (or subtract) points from the player score.
        public void addPoints(int p)
        {
            points += p;
        }

        public void UpdateInput(Gameboard gameboard)
        {
            GamePadState currentState = GamePad.GetState(index);

            if (currentState.IsConnected)
            {
                //If Player presses UP on left thumbstick
                if (currentState.ThumbSticks.Left.Y > 0.0f)
                {
                    if (cursor.getSpriteCenter().Y >= Variables.cellHeigth / 2) 
                       cursor.updatePosition(Variables.Direction.Up);
                }

                //If Player presses DOWN on left thumbstick
                if (currentState.ThumbSticks.Left.Y < 0.0f)
                {
                    if(cursor.getSpriteCenter().Y <= (gameboard.numberOfRows - 1) * Variables.cellHeigth + Variables.cellHeigth/2)
                        cursor.updatePosition(Variables.Direction.Down);
                }

                //If Player presses RIGHT on left thumbstick
                if (currentState.ThumbSticks.Left.X > 0.0f)
                {
                    if(cursor.getSpriteCenter().X <= (gameboard.numberOfColumns - 1) * Variables.cellWidth + Variables.cellWidth/2)
                        cursor.updatePosition(Variables.Direction.Right);
                }

                //If Player presses LEFT on left thumbstick
                if (currentState.ThumbSticks.Left.X < 0.0f)
                {
                    if(cursor.getSpriteCenter().X >= Variables.cellWidth / 2)
                        cursor.updatePosition(Variables.Direction.Left);
                }

                int column = cursor.getCurrentColumn();
                int row = cursor.getCurrentRow();

                // Process input only if connected and button A is pressed.
                if (currentState.Buttons.A == ButtonState.Pressed)
                {
                    addArrow(column, row, Variables.Direction.Down, gameboard, 1);
                }

                // Process input only if connected and button X is pressed.
                else if (currentState.Buttons.X == ButtonState.Pressed)
                {
                    addArrow(column, row, Variables.Direction.Left, gameboard, 2);
                }

                // Process input only if connected and button Y is pressed.
                else if (currentState.Buttons.Y == ButtonState.Pressed)
                {
                    addArrow(column, row, Variables.Direction.Up, gameboard, 4);
                }

                // Process input only if connected and button B is pressed.
                else if (currentState.Buttons.B == ButtonState.Pressed)
                {
                    addArrow(column, row, Variables.Direction.Right, gameboard, 3);
                }

                // Process input only if connected and Right Trigger is pulled.
                else if (currentState.Triggers.Right > 0.5f)
                {
                    Cell c = gameboard.getCell(row, column);
                    bool match = false;
                    foreach (Cell temp in p_arrows)
                    {
                        if (c.Equals(temp))
                        {
                            gameboard.updateTile(column, row, Variables.Direction.None, Tile.cellBorder);
                            match = true;
                        }
                    }
                    if (match) p_arrows.Remove(c);
                }

                // Process input only if connected and Left Trigger is pulled.
                else if (currentState.Triggers.Left > 0.5f)
                {
                    foreach (Cell temp in p_arrows)
                    {
                        gameboard.updateTile(column, row, Variables.Direction.None, Tile.cellBorder);
                    }
                }
            }

            #region NoXboxController
            //FOLLOWING SECTION IS TO BE REMOVED, IT IS THE PLAYER 1 MOUSE DEBUG TOOL FOR DEVELOPER WITHOUT XBOX CONTROLLER
            //*************************************************************************************************************

            KeyboardState keyboardStateCurrent = Keyboard.GetState(); //current state of the keyboard
            MouseState mouseStateCurrent = Mouse.GetState();  //current state of the mouse
            Vector2 position = new Vector2(mouseStateCurrent.X, mouseStateCurrent.Y);

            //If player presses LEFT key
            if (keyboardStateCurrent.IsKeyDown(Keys.Left))
            {
                addArrow(position, Variables.Direction.Left, gameboard, 2);
            }

            //If player Presses UP key
            else if (keyboardStateCurrent.IsKeyDown(Keys.Up))
            {
                addArrow(position, Variables.Direction.Up, gameboard, 4);
            }

            //If player Presses RIGHT key
            else if (keyboardStateCurrent.IsKeyDown(Keys.Right))
            {
                addArrow(position, Variables.Direction.Right, gameboard, 3);
            }

            //If player presses DOWN key
            else if (keyboardStateCurrent.IsKeyDown(Keys.Down))
            {
                addArrow(position, Variables.Direction.Down, gameboard, 1);
            }

            //*************************************************************************************************************/
            #endregion
        }


        // Draws the player's score board.
        public void draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            scoreBoard.DrawFrame(spriteBatch, Variables.Direction.None);
            playerBase.DrawFrame(spriteBatch, Variables.Direction.None);
            spriteBatch.DrawString(font, points.ToString(), scoreBoard.coord + OFFSET, Color.Black);
        }

        private void removeArrow(int column, int row, Gameboard gameboard)
        {
            if (column >= Variables.columns || row >= Variables.rows) return;
            gameboard.updateTile(column, row, Variables.Direction.None, Tile.cellBorder);
            Cell cell = gameboard.getCell(row, column);
            p_arrows.Remove(cell);      
        }

        private void removeArrow(Cell cell, Gameboard gameboard)
        {
            gameboard.updateTile(cell.getPositionY(), cell.getPositionX(), Variables.Direction.None, Tile.cellBorder);
            p_arrows.Remove(cell);    
        }

        private void addArrow(int column, int row, Variables.Direction dir, Gameboard gameboard, int dirtex)
        {
            if (column >= Variables.columns || row >= Variables.rows) return;
            Cell cell = gameboard.getCell(row, column);

            if (!hasArrow(cell) && rdy)
            {
                if (p_arrows.Count >= MAX_ARROWS)
                {
                    Cell c = p_arrows[p_arrows.Count - 1];
                    removeArrow(c, gameboard);
                }

                p_arrows.Insert(0, cell);
                gameboard.updateTile(column, row, dir, arrows[dirtex - 1]);
            }
            else
            {
                removeArrow(cell, gameboard);
                p_arrows.Insert(0, cell);
                gameboard.updateTile(column, row, dir, arrows[dirtex - 1]);
            }
        }

        //FOLLOWING SECTION IS TO BE REMOVED, IT IS THE PLAYER 1 MOUSE DEBUG TOOL FOR DEVELOPER WITHOUT XBOX CONTROLLER
        //*************************************************************************************************************
        private void addArrow(Vector2 position, Variables.Direction dir, Gameboard gameboard, int dirtex)
        {
            int tempCol = (int)position.X / Variables.cellWidth;
            int tempRow = (int)position.Y / Variables.cellHeigth;
            Cell temp_c = gameboard.getCell(tempRow, tempCol);

            if (!hasArrow(temp_c) && rdy)
            {

                if (p_arrows.Count >= 4)
                {
                    Cell c = p_arrows[p_arrows.Count - 1];
                    int c_column = c.getPositionY();
                    int c_row = c.getPositionX();

                    gameboard.updateTile(c_column, c_row, Variables.Direction.None, Tile.cellBorder);

                    p_arrows.Remove(c);
                }

                p_arrows.Insert(0, temp_c);
                gameboard.updateTile(position, dir, arrows[dirtex - 1]);
            }
        }
        //**************************************************************************************************************/

        private bool hasArrow(Cell cell)
        {

            foreach (Cell c in p_arrows)
            {
                if (c.Equals(cell))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
