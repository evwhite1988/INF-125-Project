using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Tile_Engine
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Gameboard gameboard = new Gameboard();
        bool clicked = false;
        int squaresAcross = 30;
        int squaresDown = 30;
        int speed = 5;
        GameSprite gnome;
        Texture2D gnomeTex;
        

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            this.IsMouseVisible = true;
            this.graphics.PreferredBackBufferWidth = graphics.GraphicsDevice.DisplayMode.Width;
            this.graphics.PreferredBackBufferHeight = graphics.GraphicsDevice.DisplayMode.Height;
    

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.

            Tile.textureSet = Content.Load<Texture2D>("part1_tileset");
            Tile.cellBorder = Content.Load<Texture2D>("tile");
            gnomeTex = Content.Load<Texture2D>("gnomes");

            gnome = new GameSprite(gnomeTex, 1);

            spriteBatch = new SpriteBatch(GraphicsDevice);



            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            MouseState ms = Mouse.GetState();
            KeyboardState ks = Keyboard.GetState();

            if (ms.LeftButton == ButtonState.Pressed)
            {

                if (ms.LeftButton != ButtonState.Released)
                {
                    Vector2 mousePosition = new Vector2(ms.X, ms.Y);
                    gameboard.updateTile(mousePosition);
                }

            }

            if(ks.IsKeyDown(Keys.Down))
            {
                gnome.position = new Vector2(gnome.position.X, gnome.position.Y + speed);

            }
            if (ks.IsKeyDown(Keys.Up))
            {
                gnome.position = new Vector2(gnome.position.X, gnome.position.Y - speed);

            }
            if (ks.IsKeyDown(Keys.Left))
            {
                gnome.position = new Vector2(gnome.position.X - speed, gnome.position.Y);

            }
            if (ks.IsKeyDown(Keys.Right))
            {
                gnome.position = new Vector2(gnome.position.X + speed, gnome.position.Y);

            }



            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            
            spriteBatch.Begin();
           // graphics.GraphicsDevice.Clear(Color.White);

            

            for (int y = 0; y < squaresDown; y++)
            {
                for (int x = 0; x < squaresAcross; x++)
                {
                    if (gameboard.getTileID(x, y) == 1)
                    {
                        spriteBatch.Draw(
                        Tile.cellBorder,
                        new Rectangle((x * 64), (y * 64), Tile.cellBorder.Width, Tile.cellBorder.Height),
                        Tile.getTexture(),
                        Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(
                        Tile.textureSet,
                        new Rectangle((x * 64), (y * 64), Tile.cellBorder.Width, Tile.cellBorder.Height),
                        Tile.getTexture(),
                        Color.White);
                    }
                }
            }
            gnome.draw(spriteBatch);

            spriteBatch.End();
            graphics.ApplyChanges();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
