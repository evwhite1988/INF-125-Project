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
    class GameSprite
    {
        Texture2D spritesheet;  //Sprite sheet containing all of the timelines related to this sprite
        public enum Direction { Up, Down, Left, Right }; //Directions for movement
        public Direction direction;  //Current direction the Sprite is moving
        public Vector2 position{get; set;}  //Current position of sprite
        public bool changeState = false;
        public int lastColumn = 0;
        public int lastRow = 0;
        
        

        //CONSTRUCTOR
        public GameSprite(Texture2D spritesheet, int framecount)
        {
            this.spritesheet = spritesheet;
            this.position = new Vector2(16, 0);
            direction = Direction.Right;
        }

        //
        public Rectangle framePosition(){

            return new Rectangle(0, 0, 32, 64);

        }

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
                if (this.direction == GameSprite.Direction.Right)
                {
                    for (int i = 16; i <= gameboard.numberOfColumns * Variables.cellWidth; )
                    {
                        if (this.position.X < i)
                        {
                            this.position = new Vector2(this.position.X + Variables.speed, this.position.Y);
                            break;
                        }
                        else
                            i = i + Variables.cellWidth;
                    }
                }
                if (this.direction == GameSprite.Direction.Left)
                {
                    for (int i = (gameboard.numberOfColumns * Variables.cellWidth) + 16; i >= 16; )
                    {
                        if (this.position.X > i)
                        {
                            this.position = new Vector2(this.position.X - Variables.speed, this.position.Y);
                            break;
                        }
                        else
                            i = i - Variables.cellWidth;
                    }
                }
                if (this.direction == GameSprite.Direction.Down)
                {
                    for (int i = 0; i <= (17 * Variables.cellHeigth) - Variables.cellHeigth; )
                    {
                        if (this.position.Y < i)
                        {
                            this.position = new Vector2(this.position.X, this.position.Y + Variables.speed);
                            break;
                        }
                        else
                            i = i + Variables.cellHeigth;
                    }
                }
                if (this.direction == GameSprite.Direction.Up)
                {
                    for (int i = (17 * Variables.cellHeigth) - Variables.cellHeigth; i >= 0; )
                    {
                        if (this.position.Y > i)
                        {
                            this.position = new Vector2(this.position.X, this.position.Y - Variables.speed);
                            break;
                        }
                        else
                            i = i - Variables.cellHeigth;
                    }
                }
            }
        }


        public int getCurrentColumn(Gameboard gameboard)
        {
            for (int i = 0; i < gameboard.numberOfColumns; i++)
            {
                if (this.position.X > (i * Variables.cellWidth) + 16 && this.position.X <= ((i + 1) * Variables.cellWidth) + 16)
                {
                    return i; 
                };
            }

            return -1;

        }

        public int getCurrentRow(Gameboard gameboard)
        {
            for (int i = 0; i < gameboard.numberOfRows; i++)
            {
                if (this.position.Y >= i * Variables.cellHeigth && this.position.Y <= (i + 1) * Variables.cellHeigth)
                {
                    return i;
                }
            }

            return -1;
        }

        public void updateState(Direction newDirection, Gameboard gameboard)
        {
            direction = newDirection;
            changeState = true;
            lastColumn = getCurrentColumn(gameboard);
            lastRow = getCurrentRow(gameboard);
        }

        public void draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(spritesheet, position, framePosition(), Color.White);
            
        }
    }
}
