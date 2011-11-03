using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Tile_Engine
{
    class Player
    {
        public int points;                     //player's current points
        public GameSprite scoreBoard;         //player's personal scoreBoard
        public GameSprite playerBase;
        private static Vector2 OFFSET = new Vector2(80, 70);    //offset used when alligning the point string to the scoreBoard


        //CONSTRUCTOR
        public Player(Texture2D s_Board, Texture2D p_Base, Vector2 s_coord, Vector2 p_coord)
        {
            //intialize points to zero
            points = 0;

            //intialize the player score board
            scoreBoard = new GameSprite(s_Board, 1, 1);
            scoreBoard.coord = s_coord;
            scoreBoard.spriteWidth = s_Board.Width;
            scoreBoard.spriteHeight = s_Board.Height;

            //initialize the player's home base
            playerBase = new GameSprite(p_Base, 1, 1);
            playerBase.coord = p_coord;
            playerBase.spriteWidth = 64;
            playerBase.spriteHeight = 64;
        }


        // Method to add (or subtract) points from the player score.
        public void addPoints(int p)
        {
            points += p;
        }


        // Draws the player's score board.
        public void draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            scoreBoard.DrawFrame(spriteBatch, Variables.Direction.None);
            playerBase.DrawFrame(spriteBatch, Variables.Direction.None);
            spriteBatch.DrawString(font, points.ToString(), scoreBoard.coord + OFFSET, Color.Black);
        }
    }
}
