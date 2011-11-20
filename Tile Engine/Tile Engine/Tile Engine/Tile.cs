using System;
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
        static public Texture2D player1Home;
        static public Texture2D player2Home;
        static public Texture2D player3Home;
        static public Texture2D player4Home;
        static public Texture2D hole;
        static public Texture2D spawn;

        static public Rectangle getTexture()
        {
            return new Rectangle(0, 0, 64, 64);
        }

        static public Rectangle getHomeTexture()
        {
            return new Rectangle(0, 0, 64, 64);
        }

    }
}
