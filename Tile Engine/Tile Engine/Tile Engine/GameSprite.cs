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
        public Texture2D[] spritesheets;  //Sprite sheet containing all of the timelines related to this sprite
        public Vector2 coord;
        
        //Current position of sprite (top-left corner)
        public int spriteWidth;
        public int spriteHeight;
        
        //variables introduced to animation system
        private int framecount;         // number of frames in the animation
        private float TimePerFrame;     // the amount of time spent per frame
        private int Frame;              // the current frame number
        private float TotalElapsed;     
        private bool Paused;
        
        
        //CONSTRUCTOR
        public GameSprite(Texture2D[] spriteSheets, int frameCount, int framesPerSec)
        {
            this.spritesheets = spriteSheets;
            this.framecount = frameCount;
            TimePerFrame = (float)1 / framesPerSec;
            Frame = 0;
            TotalElapsed = 0;
            Paused = false;
        }

        public GameSprite(Texture2D spritesheet, int frameCount, int framesPerSec)
        {
            this.spritesheets = new Texture2D[1];
            this.spritesheets[0] = spritesheet;
            this.framecount = frameCount;
            TimePerFrame = (float)1 / framesPerSec;
            Frame = 0;
            TotalElapsed = 0;
            Paused = false;
        }

        //Updates the current frame
        public void UpdateFrame(float elapsed)
        {
            if (Paused)
                return;
            TotalElapsed += elapsed;
            if (TotalElapsed > TimePerFrame)
            {
                Frame++;
                Frame = Frame % framecount;
                TotalElapsed -= TimePerFrame;
            }
        }

        //draws the frame, if the direction is None = -1, we make sheet 0 so it won't break the array.
        public void DrawFrame(SpriteBatch batch, Variables.Direction direction)
        {
            int sheet = (int)direction;
            if (sheet < (int)Variables.Direction.Up)
            {
                sheet++;
            }
            DrawFrame(batch, Frame, sheet);
        }

        public void DrawFrame(SpriteBatch batch, int frame, int spriteSheet)
        {
            int FrameWidth = spritesheets[spriteSheet].Width / framecount;
            Rectangle sourceRect = new Rectangle(FrameWidth * frame, 0, FrameWidth, spritesheets[spriteSheet].Height);
            batch.Draw(spritesheets[spriteSheet], coord, sourceRect, Color.White);
        }

        public bool IsPaused
        {
            get { return Paused; }
        }

        public void Reset()
        {
            Frame = 0;
            TotalElapsed = 0f;
        }

        public void Stop()
        {
            Pause();
            Reset();
        }

        public void Play()
        {
            Paused = false;
        }

        public void Pause()
        {
            Paused = true;
        }

        public Vector2 getSpriteCenter()
        {
           return new Vector2(coord.X + spriteWidth / 2, coord.Y + spriteHeight / 2);
        }

        public Vector2 getCoord()
        { 
            return coord; 
        }
        
        public void setCoord(Vector2 value) 
        { 
            coord = value; 
        }
    }
}
