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
        Gameboard gameboard = new Gameboard();  //Gameboard object
        List<Gnome> gnomeList;                  //List of Gnome Sprite Objects                       
        Cursor cursor;                          //Cursor Sprite Object
        Texture2D gnomeTex;
        GameTime timer;
        

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
            gnomeList = new List<Gnome>();
            spriteBatch = new SpriteBatch(GraphicsDevice);
            timer = new GameTime();

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
            Tile.arrowUp = Content.Load<Texture2D>("arrow_up");
            Tile.arrowDown = Content.Load<Texture2D>("arrow_down");
            Tile.arrowLeft = Content.Load<Texture2D>("arrow_left");
            Tile.arrowRight = Content.Load<Texture2D>("arrow_right");
            gnomeTex = Content.Load<Texture2D>("gnomes");
            cursor = new Cursor(Content.Load<Texture2D>("cursor"), 1); //game cursor

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
            //Spawns gnomes (5/sec)
            if (gameTime.TotalGameTime.Milliseconds % 200 == 0)
            {
                gnomeList.Add(new Gnome(gnomeTex, 1));
                timer = new GameTime();
            }

            MouseState mouseStateCurrent = Mouse.GetState();  //current state of the mouse
            KeyboardState keyboardStateCurrent = Keyboard.GetState(); //current state of the keyboard

            if (mouseStateCurrent.LeftButton == ButtonState.Pressed)
            {
                    Vector2 mousePosition = new Vector2(mouseStateCurrent.X, mouseStateCurrent.Y);
                    gameboard.updateTile(mousePosition);
            }

            UpdateInput();

            foreach(Gnome gnome in gnomeList)
            {
                gnome.updatePosition(gameboard);
            }

            base.Update(gameTime);
        }

        void UpdateInput()
        {
            // Get the current gamepad state.
            GamePadState currentState = GamePad.GetState(PlayerIndex.One);

            if (currentState.IsConnected)
            {
                //If Player presses UP on left thumbstick
                if (currentState.ThumbSticks.Left.Y > 0.0f)
                {
                    cursor.updatePosition(Variables.Direction.Up);
                }

                //If Player presses DOWN on left thumbstick
                if (currentState.ThumbSticks.Left.Y < 0.0f)
                {
                    cursor.updatePosition(Variables.Direction.Down);
                }

                //If Player presses RIGHT on left thumbstick
                if (currentState.ThumbSticks.Left.X > 0.0f)
                {
                    cursor.updatePosition(Variables.Direction.Right);
                }

                //If Player presses LEFT on left thumbstick
                if (currentState.ThumbSticks.Left.X < 0.0f)
                {
                    cursor.updatePosition(Variables.Direction.Left);
                }

                // Process input only if connected and button A is pressed.
                if (currentState.Buttons.A == ButtonState.Pressed)
                {
                    Vector2 cursorPosition = cursor.position;
                    gameboard.updateTile(cursorPosition, Variables.Direction.Down);
                }
                
                // Process input only if connected and button X is pressed.
                else if (currentState.Buttons.X == ButtonState.Pressed)
                {
                    Vector2 cursorPosition = cursor.position;
                    gameboard.updateTile(cursorPosition, Variables.Direction.Left);
                }

                // Process input only if connected and button Y is pressed.
                else if (currentState.Buttons.Y == ButtonState.Pressed)
                {
                    Vector2 cursorPosition = cursor.position;
                    gameboard.updateTile(cursorPosition, Variables.Direction.Up);
                }

                // Process input only if connected and button B is pressed.
                else if (currentState.Buttons.B == ButtonState.Pressed)
                {
                    Vector2 cursorPosition = cursor.position;
                    gameboard.updateTile(cursorPosition, Variables.Direction.Right);
                }

                // Process input only if connected and Right Trigger is pulled.
                else if (currentState.Triggers.Right > 0.5f)
                {
                    Vector2 cursorPosition = cursor.position;
                    gameboard.updateTile(cursorPosition, Variables.Direction.None);
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            
            spriteBatch.Begin();
           // graphics.GraphicsDevice.Clear(Color.White);

            

            for (int y = 0; y < Variables.rows; y++)
            {
                for (int x = 0; x < Variables.columns; x++)
                {
                    Cell cell = gameboard.getCell(y, x);
                    int tileID = cell.TileID;

                    if (tileID == 1)
                    {
                        spriteBatch.Draw(
                        Tile.arrowUp,
                        new Rectangle((x * Variables.cellWidth), (y * Variables.cellHeigth), Tile.cellBorder.Width, Tile.cellBorder.Height),
                        Tile.getTexture(),
                        Color.White);
                    }
                    else if (tileID == 2)
                    {
                        spriteBatch.Draw(
                        Tile.arrowRight,
                        new Rectangle((x * Variables.cellWidth), (y * Variables.cellHeigth), Tile.cellBorder.Width, Tile.cellBorder.Height),
                        Tile.getTexture(),
                        Color.White);
                    }
                    else if (tileID == 3)
                    {
                        spriteBatch.Draw(
                        Tile.arrowDown,
                        new Rectangle((x * Variables.cellWidth), (y * Variables.cellHeigth), Tile.cellBorder.Width, Tile.cellBorder.Height),
                        Tile.getTexture(),
                        Color.White);
                    }
                    else if (tileID == 4)
                    {
                        spriteBatch.Draw(
                        Tile.arrowLeft,
                        new Rectangle((x * Variables.cellWidth), (y * Variables.cellHeigth), Tile.cellBorder.Width, Tile.cellBorder.Height),
                        Tile.getTexture(),
                        Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(
                        Tile.cellBorder,
                        new Rectangle((x * Variables.cellWidth), (y * Variables.cellHeigth), Tile.cellBorder.Width, Tile.cellBorder.Height),
                        Tile.getTexture(),
                        Color.White);
                    }
                }
            }

            foreach (Gnome gnome in gnomeList)
            {
                gnome.draw(spriteBatch);
            }

            cursor.draw(spriteBatch);

            spriteBatch.End();
            graphics.ApplyChanges();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
