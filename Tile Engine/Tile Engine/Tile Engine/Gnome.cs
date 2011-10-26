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
        

        //CONSTRUCTOR
        public Gnome(Texture2D spritesheet, int framecount) : base(spritesheet, framecount)
        {
            this.position = new Vector2(0, 0);
            direction = Variables.Direction.Right;
            this.spriteWidth = 32;
            this.spriteHeight = 64;
        }

        //Updates the position of the Sprite on the gameboard
        public void updatePosition(Gameboard gameboard)
        {
            if (changeState)
            {
                if (this.position.X != ((lastColumn + 1) * Variables.cellWidth) + 16)
                    this.position = new Vector2(this.position.X + Variables.speed, this.position.Y);
                else if (this.position.Y != (lastRow + 1) * Variables.cellHeigth)
                    this.position = new Vector2(this.position.X, this.position.Y + Variables.speed);
                else
                    changeState = false;
            }

            else
            {
                collisionCheck(gameboard); //checks if the Sprite has colided with the gameboard edge or a wall
                newDirectionCheck();
            }
        }

        private void newDirectionCheck()
        {
            if (this.direction == Variables.Direction.Right)
            {
                this.position = new Vector2(this.position.X + Variables.speed, this.position.Y);
            }

            if (this.direction == Variables.Direction.Left)
            {
                this.position = new Vector2(this.position.X - Variables.speed, this.position.Y);
            }

            if (this.direction == Variables.Direction.Down)
            {
                this.position = new Vector2(this.position.X, this.position.Y + Variables.speed);
            }

            if (this.direction == Variables.Direction.Up)
            {
                this.position = new Vector2(this.position.X, this.position.Y - Variables.speed);
            }
        }

        public void updateState(Variables.Direction newDirection, Gameboard gameboard)
        {
            direction = newDirection;
            //changeState = true;
            lastColumn = getCurrentColumn(gameboard);
            lastRow = getCurrentRow(gameboard);
        }

        //Checks if Sprite has reached edge of gameboard and if so, changes direction clockwise
        public void collisionCheck(Gameboard gameboard)
        {
            int tileID = gameboard.getCell(this.getCurrentRow(gameboard), this.getCurrentColumn(gameboard)).TileID;

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

            if (this.direction == Variables.Direction.Right && this.position.X > (gameboard.numberOfColumns - 1) * Variables.cellWidth + 16)
            {
                direction = Variables.Direction.Down;
            }

            if (this.direction == Variables.Direction.Left && this.position.X < 0 + 16)
            {
                direction = Variables.Direction.Up;
            }

            if (this.direction == Variables.Direction.Up && this.position.Y < 0)
            {
                direction = Variables.Direction.Right;
            }

            if (this.direction == Variables.Direction.Down && this.position.Y > (gameboard.numberOfRows - 1) * Variables.cellHeigth)
            {
                direction = Variables.Direction.Left;
            }
        }
    }
}
