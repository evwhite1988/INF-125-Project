using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Tile_Engine
{
    class Particle
    {
        public float birthTime;
        public float timeMax;
        public Vector2 OriginalPosition;
        public Vector2 NewPosition;
        public Vector2 Position;
        public Vector2 Dir;
        public Vector2 acc;
        public float scale;
        public Color ModColor;
    }

}
