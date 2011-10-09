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
        public Vector2 position{get; set;}
        
        


        public GameSprite(Texture2D spritesheet, int framecount)
        {
            this.spritesheet = spritesheet;
            this.position = new Vector2(0, 0);

        }


        public Rectangle framePosition(){

            return new Rectangle(0, 0, 32, 64);

        }

        public void draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(spritesheet, position, framePosition(), Color.White);
            
        }


    }
}
