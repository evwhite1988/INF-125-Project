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

        Player player1;                         //Player one
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Gameboard gameboard = new Gameboard();  //Gameboard object
        List<Gnome> gnomeList { get; set; }     //List of Gnome Sprite Objects    
        List<Player> playerList;           
        Cursor cursor;                          //Cursor Sprite Object
        Texture2D gnomeTex;                     //Gnome texture
        Texture2D evilGnomeTex;                 //Evil gnome texture
        Texture2D player1_sb;                    //Player1's score board
        int evilGnomeSpawnTime;                 //Time between evil-gnome spawns
        int evilGnomeSpawnTimeRemaining; 
        int gnomeSpawnTime;                     //Time between gnome spawns
        int gnomeSpawnTimeRemaining;
        int numOfGnomes;                        //Number of gnomes on the field;

        //Intervals for random time selection: To Be Adjusted
        int evilGnomeSpawnMin = 5000;
        int evilGnomeSpawnMax = 10000;
        int gnomeSpawnMin = 500;
        int gnomeSpawnMax = 2000;


        Texture2D wallTexVerticle;
        Texture2D wallTexHorizontal;
        Random random;

        public SpriteFont scoreFont;    


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

            //Window fits to the gameboard.
            this.graphics.PreferredBackBufferWidth = gameboard.numberOfColumns * Variables.cellWidth;
            this.graphics.PreferredBackBufferHeight = (gameboard.numberOfRows * Variables.cellHeigth);


            random = new Random();
            gnomeList = new List<Gnome>();
            playerList = new List<Player>();
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Gnomes will spawn at random intervals based on the given inputs.
            evilGnomeSpawnTime = random.Next(evilGnomeSpawnMin, evilGnomeSpawnMax);
            gnomeSpawnTime = random.Next(gnomeSpawnMin, gnomeSpawnMax);
            numOfGnomes = 0;


            evilGnomeSpawnTimeRemaining = evilGnomeSpawnTime;
            gnomeSpawnTimeRemaining = gnomeSpawnTime;

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
            Tile.cellBorder = Content.Load<Texture2D>("grass");
            Tile.arrowUp = Content.Load<Texture2D>("arrow_up");
            Tile.arrowDown = Content.Load<Texture2D>("arrow_down");
            Tile.arrowLeft = Content.Load<Texture2D>("arrow_left");
            Tile.arrowRight = Content.Load<Texture2D>("arrow_right");
            Tile.home = Content.Load<Texture2D>("dog-house");
            Tile.hole = Content.Load<Texture2D>("hole");
            Tile.spawn = Content.Load<Texture2D>("spawn");
            wallTexHorizontal = Content.Load<Texture2D>("wall_horizontal");
            wallTexVerticle = Content.Load<Texture2D>("wall_verticle");
            gnomeTex = Content.Load<Texture2D>("gnomes");
            evilGnomeTex = Content.Load<Texture2D>("gnomes-evil");
            cursor = new Cursor(Content.Load<Texture2D>("cursor"), 1); //game cursor
            player1_sb = Content.Load<Texture2D>("Player1");
            scoreFont = Content.Load<SpriteFont>("pointFont");

            //Initialize first player now that the texture is loaded. The position is manually set to the Tile with Tile ID -1, Although
            //this needs to be revised to place it at a specified home base. 

            player1 = new Player(player1_sb, 
                Tile.home,
                new Vector2(0, this.graphics.PreferredBackBufferHeight),
                new Vector2(Tile.cellBorder.Width, Tile.cellBorder.Height));
            playerList.Add(player1);
            
            //increase screen size to fit scoreBoard

            this.graphics.PreferredBackBufferHeight = this.graphics.PreferredBackBufferHeight + player1_sb.Height;

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
            removeGnomes();             //Removes gnomes
            spawnGnomes(gameTime);      //Spawns new gnomes
            UpdateInput();              //handles new user inputs

            // For each gnome on the gameboard, update its position
            foreach(Gnome gnome in gnomeList)
            {
                gnome.updatePosition(gameboard);
            }

            base.Update(gameTime);
        }

        void UpdateInput()
        {
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
                    int column = cursor.getCurrentColumn();
                    int row = cursor.getCurrentRow();
                    gameboard.updateTile(column, row, Variables.Direction.Down);
                }
                
                // Process input only if connected and button X is pressed.
                else if (currentState.Buttons.X == ButtonState.Pressed)
                {
                    int column = cursor.getCurrentColumn();
                    int row = cursor.getCurrentRow();
                    gameboard.updateTile(column, row, Variables.Direction.Left);
                }

                // Process input only if connected and button Y is pressed.
                else if (currentState.Buttons.Y == ButtonState.Pressed)
                {
                    int column = cursor.getCurrentColumn();
                    int row = cursor.getCurrentRow();
                    gameboard.updateTile(column, row, Variables.Direction.Up);
                }

                // Process input only if connected and button B is pressed.
                else if (currentState.Buttons.B == ButtonState.Pressed)
                {
                    int column = cursor.getCurrentColumn();
                    int row = cursor.getCurrentRow();
                    gameboard.updateTile(column, row, Variables.Direction.Right);
                }

                // Process input only if connected and Right Trigger is pulled.
                else if (currentState.Triggers.Right > 0.5f)
                {
                    int column = cursor.getCurrentColumn();
                    int row = cursor.getCurrentRow();
                    gameboard.updateTile(column, row, Variables.Direction.None);
                }
            }


            KeyboardState keyboardStateCurrent = Keyboard.GetState(); //current state of the keyboard
            MouseState mouseStateCurrent = Mouse.GetState();  //current state of the mouse

            //If player presses LEFT key
            if (keyboardStateCurrent.IsKeyDown(Keys.Left))
            {
                Vector2 mousePosition = new Vector2(mouseStateCurrent.X, mouseStateCurrent.Y);
                gameboard.updateTile(mousePosition, Variables.Direction.Left);
            }

            //If player Presses UP key
            else if (keyboardStateCurrent.IsKeyDown(Keys.Up))
            {
                Vector2 mousePosition = new Vector2(mouseStateCurrent.X, mouseStateCurrent.Y);
                gameboard.updateTile(mousePosition, Variables.Direction.Up);
            }

            //If player Presses RIGHT key
            else if (keyboardStateCurrent.IsKeyDown(Keys.Right))
            {
                Vector2 mousePosition = new Vector2(mouseStateCurrent.X, mouseStateCurrent.Y);
                gameboard.updateTile(mousePosition, Variables.Direction.Right);
            }

            //If player presses DOWN key
            else if (keyboardStateCurrent.IsKeyDown(Keys.Down))
            {
                Vector2 mousePosition = new Vector2(mouseStateCurrent.X, mouseStateCurrent.Y);
                gameboard.updateTile(mousePosition, Variables.Direction.Down);
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
            
            //Draws the tiles on the gameboard
            for (int y = 0; y < Variables.rows; y++)
            {
                for (int x = 0; x < Variables.columns; x++)
                {
                    Cell cell = gameboard.getCell(y, x);
                    int tileID = cell.getTileID();

                    spriteBatch.Draw( Tile.cellBorder, 
                        new Rectangle((x * Variables.cellWidth), (y * Variables.cellHeigth), Tile.cellBorder.Width, Tile.cellBorder.Height),
                        Tile.getTexture(),Color.White);
                    
                    //If tileID = -2, use hole tile
                    if (tileID == -2)
                    {
                       spriteBatch.Draw(
                       Tile.spawn,
                       new Rectangle((x * Variables.cellWidth), (y * Variables.cellHeigth), Tile.cellBorder.Width, Tile.cellBorder.Height),
                       Tile.getTexture(),
                       Color.White);
                    }

                    //Commented Out to work on implemented the player home base in the Player class
                    /*If tileID = -1, use home tile
                    else if (tileID == -1)
                    {
                        spriteBatch.Draw(
                        Tile.home,
                        new Rectangle((x * Variables.cellWidth), (y * Variables.cellHeigth), Tile.cellBorder.Width, Tile.cellBorder.Height),
                        Tile.getHomeTexture(),
                        Color.White);
                    }*/

                    else if (tileID == 1)
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
                }
            }

            List<Vector4> wallList = gameboard.wallList;
            foreach (Vector4 wall in wallList)
            {
                //If column is the same, draw horizontal wall
                if (wall.X == wall.Z)
                {
                    spriteBatch.Draw(
                    wallTexVerticle,
                    new Rectangle(((int)wall.Z * Variables.cellWidth), ((int)wall.W * Variables.cellHeigth), wallTexHorizontal.Width, wallTexHorizontal.Height),
                    new Rectangle(0, 0, wallTexHorizontal.Width, wallTexHorizontal.Height),
                    Color.White);
                    
                }
                //If row is the same, draw verticle wall
                else if (wall.Y == wall.W)
                {
                    spriteBatch.Draw(
                    wallTexHorizontal,
                    new Rectangle(((int)wall.Z * Variables.cellWidth - wallTexVerticle.Width / 2), ((int)wall.W * Variables.cellHeigth), wallTexVerticle.Width, wallTexVerticle.Height),
                    new Rectangle(0, 0, wallTexVerticle.Width, wallTexVerticle.Height),
                    Color.White);
                }
            }

            foreach (Gnome gnome in gnomeList)
            {
                gnome.draw(spriteBatch);
            }

            cursor.draw(spriteBatch);
            player1.draw(spriteBatch, scoreFont);

            spriteBatch.End();
            graphics.ApplyChanges();

            // TODO: Add your drawing code here



            base.Draw(gameTime);
        }

        private void spawnGnomes(GameTime gameTime)
        {
            if (numOfGnomes < 50)
            {
                gnomeSpawnTimeRemaining -= gameTime.ElapsedGameTime.Milliseconds;
                evilGnomeSpawnTimeRemaining -= gameTime.ElapsedGameTime.Milliseconds;
                List<Vector2> spawnPoints = gameboard.getSpawnPoints();

                if (evilGnomeSpawnTimeRemaining < 0)
                {
                    //foreach (Vector2 spawnPoint in spawnPoints)
                    //{
                    //    gnomeList.Add(new Gnome(evilGnomeTex, 1, (int)spawnPoint.Y, (int)spawnPoint.X));
                    //    numOfGnomes++;
                    //}

                    //Gnome spawn from a random spawn point, intead of all four at once.
                    int spawnPoint = random.Next(4);
                    gnomeList.Add(new Gnome(evilGnomeTex, 1, (int)spawnPoints[spawnPoint].Y, (int)spawnPoints[spawnPoint].X));

                    evilGnomeSpawnTimeRemaining = random.Next(evilGnomeSpawnMin, evilGnomeSpawnMax);
                }
                else if (gnomeSpawnTimeRemaining < 0)
                {
                    //foreach (Vector2 spawnPoint in spawnPoints)
                    //{
                    //    gnomeList.Add(new Gnome(gnomeTex, 1, (int)spawnPoint.Y, (int)spawnPoint.X));
                    //    numOfGnomes++;
                    //}

                    //Gnome spawn from a random spawn point, intead of all four at once.
                    int spawnPoint = random.Next(4);
                    gnomeList.Add(new Gnome(gnomeTex, 1, (int)spawnPoints[spawnPoint].Y, (int)spawnPoints[spawnPoint].X));

                    gnomeSpawnTimeRemaining = random.Next(gnomeSpawnMin, gnomeSpawnMax);
                }
            }

        }

        private void removeGnomes()
        {
            List<Vector2> bases = gameboard.getBases();
            foreach (Vector2 homebase in bases)
            {
                foreach (Gnome gnome in gnomeList)
                {
                    if (gnome.getCurrentRow(gameboard) == homebase.Y && gnome.getCurrentColumn(gameboard) == homebase.X)
                    {
                        int player = bases.IndexOf(homebase);
                        playerList[player].addPoints(100);

                        gnomeList.Remove(gnome);
                        --numOfGnomes;
                        break;
                    }
                }
            }
        }
    }
}
