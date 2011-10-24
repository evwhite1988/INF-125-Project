﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tile_Engine
{
    static class Tile
    {
        static public Texture2D textureSet;
        static public Texture2D cellBorder;
        static public Texture2D arrowUp;
        static public Texture2D arrowDown;
        static public Texture2D arrowLeft;
        static public Texture2D arrowRight;

        static public Rectangle getTexture()
        {
            return new Rectangle(0, 0, 64, 64);
        }

    }
}
