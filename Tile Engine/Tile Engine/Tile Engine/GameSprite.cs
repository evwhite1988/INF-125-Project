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
        Texture2D spritesheet;
        public enum Direction { Up, Down, Left, Right };
        public Direction direction;
        public Vector2 position{get; set;}
        public bool changeState = false;
        public int lastColumn = 0;
        public int lastRow = 0;
        
        


        public GameSprite(Texture2D spritesheet, int framecount)
        {
            this.spritesheet = spritesheet;
            this.position = new Vector2(16, 0);
            direction = Direction.Right;
        }


        public Rectangle framePosition(){

            return new Rectangle(0, 0, 32, 64);

        }

        public void updateMovement(Gameboard b)
        {
            if (changeState)
            {
                if (this.position.X != ((lastColumn + 1) * 64) + 16)
                    this.position = new Vector2(this.position.X + 4, this.position.Y);
                else if (this.position.Y != (lastRow + 1) * 64)
                    this.position = new Vector2(this.position.X, this.position.Y + 4);
                else
                    changeState = false;
            }

            else
            {
                if (this.direction == GameSprite.Direction.Right)
                {
                    for (int i = 16; i <= b.numberOfColumns * 64; )
                    {
                        if (this.position.X < i)
                        {
                            this.position = new Vector2(this.position.X + 4, this.position.Y);
                            break;
                        }
                        else
                            i = i + 64;
                    }
                }
                if (this.direction == GameSprite.Direction.Left)
                {
                    for (int i = (b.numberOfColumns * 64) + 16; i >= 16; )
                    {
                        if (this.position.X > i)
                        {
                            this.position = new Vector2(this.position.X - 4, this.position.Y);
                            break;
                        }
                        else
                            i = i - 64;
                    }
                }
                if (this.direction == GameSprite.Direction.Down)
                {
                    for (int i = 0; i <= (17 * 64) - 64; )
                    {
                        if (this.position.Y < i)
                        {
                            this.position = new Vector2(this.position.X, this.position.Y + 4);
                            break;
                        }
                        else
                            i = i + 64;
                    }
                }
                if (this.direction == GameSprite.Direction.Up)
                {
                    for (int i = (17 * 64) - 64; i >= 0; )
                    {
                        if (this.position.Y > i)
                        {
                            this.position = new Vector2(this.position.X, this.position.Y - 4);
                            break;
                        }
                        else
                            i = i - 64;
                    }
                }
            }
        }


        public int getCurrentColumn(Gameboard b)
        {
            for (int i = 0; i < b.numberOfColumns; i++)
            {
                if (this.position.X > (i * 64) + 16 && this.position.X <= ((i + 1) * 64) + 16)
                {
                    return i; 
                };
            }

            return -1;

        }

        public int getCurrentRow(Gameboard b)
        {
            for (int i = 0; i < b.numberOfColumns; i++)
            {
                if (this.position.Y >= i * 64 && this.position.Y <= (i + 1) * 64)
                {
                    return i;
                }
            }

            return -1;
        }

        public void updateState(Direction d, Gameboard b)
        {
            direction = d;
            changeState = true;
            lastColumn = getCurrentColumn(b);
            lastRow = getCurrentRow(b);
        }

        public void draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(spritesheet, position, framePosition(), Color.White);
            
        }
    }
}
