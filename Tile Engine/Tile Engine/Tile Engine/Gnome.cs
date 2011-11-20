using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Tile_Engine
{
    class Gnome : GameSprite
    {
        public Variables.Direction direction;  //Current direction the Sprite is moving
        public bool changeState = false;
        public int lastColumn = 0;
        public int lastRow = 0;
        private static int FRAMES_PER_SEC = 15;

        

        //CONSTRUCTOR
        public Gnome(Texture2D[] spritesheets, int framecount) : base(spritesheets, framecount, FRAMES_PER_SEC)
        {
            this.coord = new Vector2(0, 0);
            direction = Variables.Direction.Right;
            this.spriteWidth = 35;
            this.spriteHeight = 65;
        }

        //CONSTRUCTOR
        public Gnome(Texture2D[] spritesheets, int framecount, int row, int column)
            : base(spritesheets, framecount, FRAMES_PER_SEC)
        {
            this.coord = new Vector2(column * Variables.cellWidth, row * Variables.cellHeigth);
            direction = Variables.Direction.Right;
            this.spriteWidth = 35;
            this.spriteHeight = 65;
        }

        //Updates the position of the Sprite on the gameboard
        public void updatePosition(Gameboard gameboard, GameTime gameTime)
        {
            collisionCheck(gameboard); //checks if the Sprite has colided with the gameboard edge or a wall
            directionTileCheck(gameboard);
            
            if (this.direction == Variables.Direction.Right)
            {
                this.coord = new Vector2(this.coord.X + Variables.speed, this.coord.Y);
            }

            if (this.direction == Variables.Direction.Left)
            {
                this.coord = new Vector2(this.coord.X - Variables.speed, this.coord.Y);
            }

            if (this.direction == Variables.Direction.Down)
            {
                this.coord = new Vector2(this.coord.X, this.coord.Y + Variables.speed);
            }

            if (this.direction == Variables.Direction.Up)
            {
                this.coord = new Vector2(this.coord.X, this.coord.Y - Variables.speed);
            }

            UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        public void directionTileCheck(Gameboard gameboard)
        {
            if (getSpriteCenter().X >= getCurrentColumn(gameboard) * Variables.cellWidth + Variables.cellWidth / 2.5
                && getSpriteCenter().X <= (getCurrentColumn(gameboard) + 1) * Variables.cellWidth - Variables.cellWidth / 2.5
                && getSpriteCenter().Y >= getCurrentRow(gameboard) * Variables.cellHeigth + Variables.cellHeigth / 2.5
                && getSpriteCenter().Y <= (getCurrentRow(gameboard) + 1) * Variables.cellHeigth - Variables.cellHeigth / 2.5)
            {

                int tileID = gameboard.getCell(this.getCurrentRow(gameboard), this.getCurrentColumn(gameboard)).getTileID();

                switch (tileID)
                {
                    case 1:
                        direction = Variables.Direction.Up;
                        break;
                    case 2:
                        direction = Variables.Direction.Right;
                        break;
                    case 3:
                        direction = Variables.Direction.Down;
                        break;
                    case 4:
                        direction = Variables.Direction.Left;
                        break;
                    default: break;
                }
            }
        }

        //Checks if Sprite has reached edge of gameboard and if so, changes direction clockwise
        public void collisionCheck(Gameboard gameboard)
        {
            foreach (Vector4 wall in gameboard.wallList)
            {
                //Check Horizontal Wall
                if (wall.X == wall.Z)
                {
                    if (this.direction == Variables.Direction.Up &&
                        getSpriteCenter().Y <= (wall.W * Variables.cellHeigth) + Variables.cellHeigth / 2 &&
                        getSpriteCenter().Y >= (wall.W * Variables.cellHeigth) &&
                        getSpriteCenter().X >= (wall.Z * Variables.cellWidth) &&
                        getSpriteCenter().X <= ((wall.Z + 1) * Variables.cellWidth))
                    {
                        direction = Variables.Direction.Right;
                    }

                    if (this.direction == Variables.Direction.Down &&
                        getSpriteCenter().Y >= (wall.W * Variables.cellHeigth) - Variables.cellHeigth / 2 &&
                        getSpriteCenter().Y <= (wall.W * Variables.cellHeigth) &&
                        getSpriteCenter().X >= (wall.X * Variables.cellWidth) &&
                        getSpriteCenter().X <= ((wall.Z + 1) * Variables.cellWidth))
                    {
                        direction = Variables.Direction.Left;
                    }
                }
                //Check Verticle Wall
                else if (wall.Y == wall.W)
                {
                    if (this.direction == Variables.Direction.Right &&
                        getSpriteCenter().X >= (wall.Z * Variables.cellWidth) - Variables.cellWidth / 2 &&
                        getSpriteCenter().X <= (wall.Z * Variables.cellWidth) &&
                        getSpriteCenter().Y >= (wall.Y * Variables.cellHeigth) &&
                        getSpriteCenter().Y <= ((wall.W + 1) * Variables.cellHeigth))
                    {
                        direction = Variables.Direction.Down;
                    }

                    if (this.direction == Variables.Direction.Left &&
                        getSpriteCenter().X <= (wall.Z * Variables.cellWidth) + Variables.cellWidth / 2 &&
                        getSpriteCenter().X >= (wall.Z * Variables.cellWidth) &&
                        getSpriteCenter().Y >= (wall.Y * Variables.cellHeigth) &&
                        getSpriteCenter().Y <= ((wall.W + 1) * Variables.cellHeigth))
                    {
                        direction = Variables.Direction.Up;
                    }
                }
            }

            if (this.direction == Variables.Direction.Right && getSpriteCenter().X > (gameboard.numberOfColumns - 1) * Variables.cellWidth + Variables.cellWidth/2)
            {
                direction = Variables.Direction.Down;
            }

            if (this.direction == Variables.Direction.Left && getSpriteCenter().X < Variables.cellWidth / 2)
            {
                direction = Variables.Direction.Up;
            }

            if (this.direction == Variables.Direction.Up && getSpriteCenter().Y < Variables.cellHeigth / 2)
            {
                direction = Variables.Direction.Right;
            }

            if (this.direction == Variables.Direction.Down && getSpriteCenter().Y > (gameboard.numberOfRows - 1) * Variables.cellHeigth + Variables.cellHeigth/2)
            {
                direction = Variables.Direction.Left;
            }
               
        }


        public int getCurrentColumn(Gameboard gameboard)
        {
            for (int i = 0; i < gameboard.numberOfColumns; i++)
            {
                if (getSpriteCenter().X >= (i * Variables.cellWidth) && getSpriteCenter().X <= ((i + 1) * Variables.cellWidth))
                {
                    return i;
                };
            }

            return 0;

        }

        public int getCurrentRow(Gameboard gameboard)
        {
            for (int i = 0; i < gameboard.numberOfRows; i++)
            {
                if (getSpriteCenter().Y >= (i * Variables.cellHeigth) && getSpriteCenter().Y <= (i + 1) * Variables.cellHeigth)
                {
                    return i;
                }
            }

            return 0;
        }
    }
}
