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
        public Vector2 coord{get; set;}  //Current position of sprite (top-left corner)
        public int spriteWidth;
        public int spriteHeight;
        
        

        //CONSTRUCTOR
        public GameSprite(Texture2D spritesheet, int framecount)
        {
            this.spritesheet = spritesheet;
            this.coord = new Vector2(0, 0);
        }

        //
        public Rectangle framePosition()
        {
            return new Rectangle(0, 0, spriteWidth, spriteHeight);
        }

        public void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(spritesheet, coord, framePosition(), Color.White);    
        }

        public Vector2 getSpriteCenter()
        {
           return new Vector2(coord.X + spriteWidth / 2, coord.Y + spriteHeight / 2);
        }
    }
}
