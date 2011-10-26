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
    class Cursor: GameSprite
    {
        
        //CONSTRUCTOR
        public Cursor(Texture2D spritesheet, int framecount) : base(spritesheet, framecount)
        {
            this.position = new Vector2(0, 0);
            this.spriteWidth = 64;
            this.spriteHeight = 64;
        }

        //Updates the position of the Sprite on the gameboard
        public void updatePosition(Variables.Direction direction)
        {
            if (direction == Variables.Direction.Right)
            {
                this.position = new Vector2(this.position.X + Variables.speed, this.position.Y);
            }

            if (direction == Variables.Direction.Left)
            {
                this.position = new Vector2(this.position.X - Variables.speed, this.position.Y);
            }

            if (direction == Variables.Direction.Down)
            {
                this.position = new Vector2(this.position.X, this.position.Y + Variables.speed);
            }

            if (direction == Variables.Direction.Up)
            {
                this.position = new Vector2(this.position.X, this.position.Y - Variables.speed);
            }
        }
    }
}
