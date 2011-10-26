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
        public Texture2D spritesheet;  //Sprite sheet containing all of the timelines related to this sprite
        public Vector2 position{get; set;}  //Current position of sprite
        public int spriteWidth;
        public int spriteHeight;
        
        

        //CONSTRUCTOR
        public GameSprite(Texture2D spritesheet, int framecount)
        {
            this.spritesheet = spritesheet;
            this.position = new Vector2(0, 0);
            
        }

        //
        public Rectangle framePosition()
        {
            return new Rectangle(0, 0, spriteWidth, spriteHeight);
        }


        public int getCurrentColumn(Gameboard gameboard)
        {
            for (int i = 0; i < gameboard.numberOfColumns; i++)
            {
                if (this.position.X >= (i * Variables.cellWidth) + 32 - (spriteWidth / 2) && this.position.X <= ((i + 1) * Variables.cellWidth) - 32 - (spriteWidth / 2))
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
                if (this.position.Y >= (i * Variables.cellHeigth) + 32 - (spriteHeight / 2) && this.position.Y <= (i + 1) * Variables.cellHeigth - 32 - (spriteHeight / 2))
                {
                    return i;
                }
            }

            return 0;
        }

        public void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(spritesheet, position, framePosition(), Color.White);    
        }
    }
}
