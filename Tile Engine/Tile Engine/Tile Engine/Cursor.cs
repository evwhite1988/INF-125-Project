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
        public Cursor(Texture2D spritesheet, int framecount) : base(spritesheet, framecount, 1)
        {
            this.coord = new Vector2(0, 0);
            this.spriteWidth = 64;
            this.spriteHeight = 64;
        }

        //Updates the position of the Sprite on the gameboard
        public void updatePosition(Variables.Direction direction)
        {
            if (direction == Variables.Direction.Right)
            {
                this.coord = new Vector2(this.coord.X + Variables.speed, this.coord.Y);
            }

            if (direction == Variables.Direction.Left)
            {
                this.coord = new Vector2(this.coord.X - Variables.speed, this.coord.Y);
            }

            if (direction == Variables.Direction.Down)
            {
                this.coord = new Vector2(this.coord.X, this.coord.Y + Variables.speed);
            }

            if (direction == Variables.Direction.Up)
            {
                this.coord = new Vector2(this.coord.X, this.coord.Y - Variables.speed);
            }
        }


        public int getCurrentColumn()
        {
            for (int i = 0; i < Variables.columns; i++)
            {
                if (getSpriteCenter().X >= (i * Variables.cellWidth) && getSpriteCenter().X <= ((i + 1) * Variables.cellWidth))
                {
                    return i;
                };
            }

            return -1;

        }

        public int getCurrentRow()
        {
            for (int i = 0; i < Variables.rows; i++)
            {
                if (getSpriteCenter().Y >= (i * Variables.cellHeigth) && getSpriteCenter().Y <= (i + 1) * Variables.cellHeigth)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
