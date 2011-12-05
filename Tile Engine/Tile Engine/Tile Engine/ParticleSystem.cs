using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tile_Engine
{
    class ParticleSystem
    {
        public int totalParticles =3;
        public Texture2D sprite;
        public List<Particle> particleList = new List<Particle>();
        public bool started = false;
        GameSprite sprite_obj;

        public void Init(GameTime gameTime, Texture2D texture, GameSprite s)
        {
            sprite_obj = s;
            this.sprite = texture;
            Random r = new Random();
            for (int i = 0; i < totalParticles; i++)
            {
                Particle particle = new Particle();
                particle.birthTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
                particle.timeMax = 1500;
                particle.scale = .05f;
                particle.ModColor = Color.White;

                float particleDistance = (float)r.NextDouble() * 50;
                Vector2 displacement = new Vector2(particleDistance, 0);
                float angle = MathHelper.ToRadians(r.Next(360));
                displacement = Vector2.Transform(displacement, Matrix.CreateRotationZ(angle));

                particle.Dir = displacement;
                particle.acc = 1.0f * particle.Dir;
                particle.OriginalPosition = Vector2.Zero;

                particleList.Add(particle);
            }

            Start();
        }

        public void Start()
        {
            started = true;
        }

        public void Update(GameTime gameTime)
        {
            float f = (float)gameTime.TotalGameTime.TotalMilliseconds;

            for (int i = particleList.Count - 1; i >= 0; i--)
            {
                Particle p = particleList[i];
                float alive = f - p.birthTime;

                if (alive > p.timeMax)
                    particleList.RemoveAt(i);
                else
                {
                    if (p.OriginalPosition == Vector2.Zero)
                    {
                        p.OriginalPosition = sprite_obj.coord;
                    }
                    else
                    {
                        Vector2 displace = sprite_obj.coord - p.OriginalPosition;
                        p.NewPosition = (p.OriginalPosition + displace) + new Vector2(sprite_obj.spriteWidth/2, sprite_obj.spriteHeight/2);
                    }

                    float relativeAge = alive / p.timeMax;
                    p.Position = 0.5f * p.acc * relativeAge * relativeAge + p.Dir * relativeAge + p.NewPosition;

                    float invAge = 1.0f - relativeAge;
                    p.ModColor = new Color(new Vector4(invAge, invAge, invAge, invAge));

                    Vector2 posFromCenter = p.Position - p.NewPosition;
                    float distance = posFromCenter.Length();
                    p.scale = (50.0f + distance) / 200.0f;
                    particleList[i] = p;
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            for (int i = 0; i < particleList.Count; i++)
            {
                Particle p = particleList[i];
                sb.Draw(sprite, p.Position, null, p.ModColor, i, new Vector2(256, 256), p.scale, SpriteEffects.None, 1);
            }
            sb.End();
        }

        public bool isFinsihed()
        {
            if (particleList.Count == 0)
                return true;

            return false;
        }
    }
}
