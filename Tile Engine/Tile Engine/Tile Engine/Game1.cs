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
        //PLAYERS
        Player player1;                       
        Player player2;
        Player player3;
        Player player4;

        MouseState mPreviousMouseState;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Gameboard gameboard;                    //Gameboard object
        List<Gnome> gnomeList { get; set; }     //List of Gnome Sprite Objects    
        List<Player> playerList;

        //11-3-11: changed this to be texture arrays, splitting up the different directional animations for use in the updated GameSprite.cs
        Texture2D[] gnomeTex;                     //Gnome textures
        Texture2D[] evilGnomeTex;                 //Evil Gnome Textures
        Texture2D[] randomGnomeTex;               //Random Gnome Textures
        Texture2D[] scoreboards;                  //Player scoreBoard textures
        Texture2D cursorTex;
        Texture2D background;
        Texture2D wallTexVerticle;
        Texture2D wallTexHorizontal;
        SpriteFont gameChange;
        int frameCount = 8;

        int timer = 180000;

        int evilGnomeSpawnTimeRemaining;
        int gnomeSpawnTimeRemaining;
        int randomGnomeSpawnTimeRemaining;
        
        int numOfGnomes;                        //Number of gnomes on the field;
        int currentMainMenuIndex;        

        enum GameState { MainMenu, InGame, Credits, Instructions };
        GameState currentGameState = GameState.MainMenu;

        
        Random random;

        public SpriteFont scoreFont;
        

        //Main menu art files, courtesy of Sage's Scrolls
        MenuSelection[] mainMenuItems;  //MenuSelection class defined below. Tweaked and Reused from past games. 
        Texture2D mainMenuIconDimL;
        Texture2D mainMenuIconDimR;
        Texture2D mainMenuIconDimC;
        Texture2D mainMenuIconLitL;
        Texture2D mainMenuIconLitR;
        Texture2D mainMenuIconLitC;

        Texture2D creditText; //For the credits menu;


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
            mPreviousMouseState = Mouse.GetState();

            random = new Random();
            gnomeList = new List<Gnome>();
            playerList = new List<Player>();
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Gnomes will spawn at random intervals based on the given inputs.
            Variables.evilGnomeSpawnTime = random.Next(Variables.evilGnomeSpawnMin, Variables.evilGnomeSpawnMax);
            Variables.gnomeSpawnTime = random.Next(Variables.gnomeSpawnMin, Variables.gnomeSpawnMax);
            Variables.randomGnomeSpawnTime = random.Next(Variables.randomGnomeSpawnMin, Variables.randomGnomeSpawnMax);
            numOfGnomes = 0;


            evilGnomeSpawnTimeRemaining = Variables.evilGnomeSpawnTime;
            gnomeSpawnTimeRemaining = Variables.gnomeSpawnTime;
            randomGnomeSpawnTimeRemaining = Variables.randomGnomeSpawnTime;

            mainMenuItems = new MenuSelection[4];
            gnomeTex = new Texture2D[4];
            evilGnomeTex = new Texture2D[4];
            randomGnomeTex = new Texture2D[4];
            scoreboards = new Texture2D[4];

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            loadTextures();

            #region MenuContentLoad

            mainMenuIconDimL = Content.Load<Texture2D>("button-dim-left");
            mainMenuIconDimR = Content.Load<Texture2D>("button-dim-right");
            mainMenuIconDimC = Content.Load<Texture2D>("button-dim-middle");
            mainMenuIconLitL = Content.Load<Texture2D>("button-lit-left");
            mainMenuIconLitR = Content.Load<Texture2D>("button-lit-right");
            mainMenuIconLitC = Content.Load<Texture2D>("button-lit-middle");

            int initialX = (Window.ClientBounds.Width / 2) - ((mainMenuIconDimL.Width + mainMenuIconDimR.Width + 350) / 2);
            int initialY = (Window.ClientBounds.Height / 4) + 75;

            mainMenuItems[0] = new MenuSelection("Play", mainMenuIconDimL, mainMenuIconDimR, mainMenuIconDimC,
                mainMenuIconLitL, mainMenuIconLitR, mainMenuIconLitC, initialX, initialY, 350, scoreFont);
            mainMenuItems[1] = new MenuSelection("Instructions", mainMenuIconDimL, mainMenuIconDimR, mainMenuIconDimC,
                mainMenuIconLitL, mainMenuIconLitR, mainMenuIconLitC, initialX, initialY + 90, 350, scoreFont);
            mainMenuItems[2] = new MenuSelection("Credits", mainMenuIconDimL, mainMenuIconDimR, mainMenuIconDimC,
                mainMenuIconLitL, mainMenuIconLitR, mainMenuIconLitC, initialX, initialY + 180, 350, scoreFont);
            mainMenuItems[3] = new MenuSelection("Exit", mainMenuIconDimL, mainMenuIconDimR, mainMenuIconDimC,
                mainMenuIconLitL, mainMenuIconLitR, mainMenuIconLitC, initialX, initialY + 270, 350, scoreFont);

            #endregion

            //Window fits to the gameboard.
            gameboard = new Gameboard();
            this.graphics.PreferredBackBufferWidth = gameboard.numberOfColumns * Variables.cellWidth;
            this.graphics.PreferredBackBufferHeight = (gameboard.numberOfRows * Variables.cellHeigth);

            initializePlayers();

            //increase screen size to fit scoreBoard
            this.graphics.PreferredBackBufferHeight = this.graphics.PreferredBackBufferHeight + scoreboards[0].Height;

        }

        private void initializePlayers()
        {
            //Initialize first player now that the texture is loaded. The position is manually set to the Tile with Tile ID -1, Although
            //this needs to be revised to place it at a specified home base. 

            player1 = new Player(Tile.player1Home,
                new Vector2(Tile.cellBorder.Width, Tile.cellBorder.Height),
                new Cursor(cursorTex, 1), PlayerIndex.One);
            playerList.Add(player1);

            player2 = new Player(Tile.player2Home,
                new Vector2(Tile.cellBorder.Width * 10, Tile.cellBorder.Height),
                new Cursor(cursorTex, 1), PlayerIndex.Two);
            playerList.Add(player2);

            player3 = new Player(Tile.player3Home,
                new Vector2(Tile.cellBorder.Width, Tile.cellBorder.Height * 7),
                new Cursor(cursorTex, 1), PlayerIndex.Three);
            playerList.Add(player3);

            player4 = new Player(Tile.player4Home,
                new Vector2(Tile.cellBorder.Width * 10, Tile.cellBorder.Height * 7),
                new Cursor(cursorTex, 1), PlayerIndex.Four);
            playerList.Add(player4);

            foreach (Player p in playerList)
                p.LoadContent(this.Content, this.graphics);
        }

        private void loadTextures()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            scoreFont = Content.Load<SpriteFont>("pointFont");

            background = Content.Load<Texture2D>("background");

            Tile.textureSet = Content.Load<Texture2D>("part1_tileset");
            Tile.cellBorder = Content.Load<Texture2D>("grass");
            Tile.player1Home = Content.Load<Texture2D>("dog-house");
            Tile.player2Home = Content.Load<Texture2D>("dog-house-p2");
            Tile.player3Home = Content.Load<Texture2D>("dog-house-p3");
            Tile.player4Home = Content.Load<Texture2D>("dog-house-p4");
            Tile.hole = Content.Load<Texture2D>("hole");
            Tile.spawn = Content.Load<Texture2D>("spawn");
            wallTexHorizontal = Content.Load<Texture2D>("wall_horizontal");
            wallTexVerticle = Content.Load<Texture2D>("wall_verticle");
            gnomeTex[0] = Content.Load<Texture2D>("gnomesBack");
            gnomeTex[1] = Content.Load<Texture2D>("gnomesFront");
            gnomeTex[2] = Content.Load<Texture2D>("gnomesLeft");
            gnomeTex[3] = Content.Load<Texture2D>("gnomesRight");
            evilGnomeTex[0] = Content.Load<Texture2D>("gnomes-evilBack");
            evilGnomeTex[1] = Content.Load<Texture2D>("gnomes-evilFront");
            evilGnomeTex[2] = Content.Load<Texture2D>("gnomes-evilLeft");
            evilGnomeTex[3] = Content.Load<Texture2D>("gnomes-evilRight");
            randomGnomeTex[0] = Content.Load<Texture2D>("gnomes-randomBack");
            randomGnomeTex[1] = Content.Load<Texture2D>("gnomes-randomFront");
            randomGnomeTex[2] = Content.Load<Texture2D>("gnomes-randomLeft");
            randomGnomeTex[3] = Content.Load<Texture2D>("gnomes-randomRight");
            cursorTex = Content.Load<Texture2D>("cursor"); //gmae cursor
            gameChange = Content.Load<SpriteFont>("pointFont");

            for (int i = 0; i < 4; i++)
            {
                scoreboards[i] = Content.Load<Texture2D>("Player" + (i + 1));
            }

            creditText = Content.Load<Texture2D>("credits");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        ////////////////////////////////// DRAW METHODS /////////////////////////////////////////////////////////
        
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            this.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.Draw(background, new Rectangle(0, 0, this.graphics.PreferredBackBufferWidth, this.graphics.PreferredBackBufferHeight), Color.White);

            switch (currentGameState)
            {
                case GameState.MainMenu:
                    drawMainMenu(gameTime);
                    break;

                case GameState.Instructions:
                    drawInstructions(gameTime);
                    break;

                case GameState.InGame:
                    drawGame(gameTime);
                    break;

                case GameState.Credits:
                    drawCredits();
                    break;
            }

            spriteBatch.End();
            graphics.ApplyChanges();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        private void drawMainMenu(GameTime gameTime)
        {
            for (int i = 0; i < mainMenuItems.Length; i++)
            {
                if (i == currentMainMenuIndex)
                    mainMenuItems[i].Draw(gameTime, spriteBatch, true);
                else
                    mainMenuItems[i].Draw(gameTime, spriteBatch, false);
            }
        }

        private void drawInstructions(GameTime gameTime)
        {
            spriteBatch.DrawString(scoreFont, "Placeholder", Vector2.One, Color.White);
        }

        private void drawCredits()
        {
            Vector2 creditPos = new Vector2(graphics.GraphicsDevice.Viewport.Width / 3.5f,
                     graphics.GraphicsDevice.Viewport.Height / 3.5f);
            spriteBatch.Draw(creditText, creditPos, Color.White);
        }

        private void drawGame(GameTime gameTime)
        {
            drawGameBoard(gameTime);
            drawGnomes(gameTime);
            drawPlayers(gameTime);
        }

        private void drawGameBoard(GameTime gameTime)
        {
            for (int y = 0; y < Variables.rows; y++)
            {
                for (int x = 0; x < Variables.columns; x++)
                {
                    Cell cell = gameboard.getCell(y, x);
                    int tileID = cell.getTileID();

                    spriteBatch.Draw(Tile.cellBorder,
                        new Rectangle((x * Variables.cellWidth), (y * Variables.cellHeigth), Tile.cellBorder.Width, Tile.cellBorder.Height),
                        Tile.getTexture(), Color.White);

                    //If tileID = -2, use hole tile
                    if (tileID == -2)
                    {
                        spriteBatch.Draw(
                        Tile.spawn,
                        new Rectangle((x * Variables.cellWidth), (y * Variables.cellHeigth), Tile.cellBorder.Width, Tile.cellBorder.Height),
                        Tile.getTexture(),
                        Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(
                            cell.getTexture(),
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
                    wallTexHorizontal,
                    new Rectangle(((int)wall.Z * Variables.cellWidth), ((int)wall.W * Variables.cellHeigth), wallTexHorizontal.Width, wallTexHorizontal.Height),
                    new Rectangle(0, 0, wallTexHorizontal.Width, wallTexHorizontal.Height),
                    Color.White);

                }
                //If row is the same, draw verticle wall
                else if (wall.Y == wall.W)
                {
                    spriteBatch.Draw(
                    wallTexVerticle,
                    new Rectangle(((int)wall.Z * Variables.cellWidth - wallTexVerticle.Width / 2), ((int)wall.W * Variables.cellHeigth), wallTexVerticle.Width, wallTexVerticle.Height),
                    new Rectangle(0, 0, wallTexVerticle.Width, wallTexVerticle.Height),
                    Color.White);
                }
            }

            timer -= gameTime.ElapsedGameTime.Milliseconds;
            spriteBatch.DrawString(gameChange, "TIME: " + (timer / 1000), new
            Vector2(350, 10), Color.Red);
        }

        private void drawGnomes(GameTime gameTime)
        {
            foreach (Gnome gnome in gnomeList)
            {
                gnome.DrawFrame(spriteBatch, gnome.direction);
            }
        }

        private void drawPlayers(GameTime gameTime)
        {

            foreach (Player player in playerList)
            {
                player.cursor.DrawFrame(spriteBatch, Variables.Direction.None);
                player.draw(spriteBatch, scoreFont);
            }
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardStateCurrent = Keyboard.GetState(); //Will remove later, for debugging menu
            GamePadState currentState = GamePad.GetState(PlayerIndex.One);
            switch (currentGameState)
            {
                case GameState.MainMenu:
                    manageMenu();

                    //currently only the mouse works correctly in navigating the main menu screen. The following allows us to skip to
                    //the actual game by just pressing 'A' on the controller.


                    if (currentState.IsConnected)
                    {
                        if (currentState.IsButtonDown(Buttons.A))
                            currentGameState = GameState.InGame;
                    }
                    break;

                case GameState.Credits:
                    //GamePadState currentState = GamePad.GetState(PlayerIndex.One);
                    if (currentState.IsConnected)
                    {
                        if (currentState.IsButtonDown(Buttons.A))
                            currentGameState = GameState.MainMenu;

                    }
                    if (keyboardStateCurrent.IsKeyDown(Keys.Escape))
                    {

                        currentGameState = GameState.MainMenu;
                    }
                    break;

                case GameState.Instructions:
                    if (keyboardStateCurrent.IsKeyDown(Keys.Escape))
                    {

                        currentGameState = GameState.MainMenu;
                    }
                    break;


                case GameState.InGame:

                    if (checkGameEnd()) endGame();
                    removeGnomes();             //Removes gnomes
                    spawnGnomes(gameTime);      //Spawns new gnomes
                    UpdateInput();              //handles new user inputs

                    // For each gnome on the gameboard, update its position
                    foreach (Gnome gnome in gnomeList)
                    {
                        gnome.updatePosition(gameboard, gameTime);
                    }

                    base.Update(gameTime);
                    break;

            }
        }

        private void endGame()
        {
            currentGameState = GameState.Credits;
        }

        private bool checkGameEnd()
        {
            if (timer <= 0) return true;
            else return false;
        }

        private void UpdateInput()
        {
            foreach (Player player in playerList)
            {
                GamePadState currentState = GamePad.GetState(player.index);

                if (currentState.IsConnected)
                {
                    //If Player presses UP on left thumbstick
                    if (currentState.ThumbSticks.Left.Y > 0.0f)
                    {
                        if (player.cursor.getSpriteCenter().Y >= Variables.cellHeigth / 2)
                            player.cursor.updatePosition(Variables.Direction.Up);
                    }

                    //If Player presses DOWN on left thumbstick
                    if (currentState.ThumbSticks.Left.Y < 0.0f)
                    {
                        if (player.cursor.getSpriteCenter().Y <= (gameboard.numberOfRows - 1) * Variables.cellHeigth + Variables.cellHeigth / 2)
                            player.cursor.updatePosition(Variables.Direction.Down);
                    }

                    //If Player presses RIGHT on left thumbstick
                    if (currentState.ThumbSticks.Left.X > 0.0f)
                    {
                        if (player.cursor.getSpriteCenter().X <= (gameboard.numberOfColumns - 1) * Variables.cellWidth + Variables.cellWidth / 2)
                            player.cursor.updatePosition(Variables.Direction.Right);
                    }

                    //If Player presses LEFT on left thumbstick
                    if (currentState.ThumbSticks.Left.X < 0.0f)
                    {
                        if (player.cursor.getSpriteCenter().X >= Variables.cellWidth / 2)
                            player.cursor.updatePosition(Variables.Direction.Left);
                    }

                    int column = player.cursor.getCurrentColumn();
                    int row = player.cursor.getCurrentRow();

                    Cell cell = gameboard.getCell(row, column);

                    if (currentState.Buttons.A == ButtonState.Pressed ||
                        currentState.Buttons.X == ButtonState.Pressed ||
                        currentState.Buttons.Y == ButtonState.Pressed ||
                        currentState.Buttons.B == ButtonState.Pressed ||
                        currentState.Triggers.Right > 0.5f ||
                        currentState.Triggers.Left > 0.5f)
                    {
                        switch (cell.getOwnedBy())
                        {
                            case 0:
                                break;

                            case 1:
                                player1.removeArrow(cell, gameboard);
                                break;

                            case 2:
                                player2.removeArrow(cell, gameboard);
                                break;

                            case 3:
                                player3.removeArrow(cell, gameboard);
                                break;

                            case 4:
                                player4.removeArrow(cell, gameboard);
                                break;
                        }

                        // Process input only if connected and button A is pressed.
                        if (currentState.Buttons.A == ButtonState.Pressed)
                        {
                            player.addArrow(column, row, Variables.Direction.Down, gameboard, 1);
                        }

                        // Process input only if connected and button X is pressed.
                        else if (currentState.Buttons.X == ButtonState.Pressed)
                        {
                            player.addArrow(column, row, Variables.Direction.Left, gameboard, 2);
                        }

                        // Process input only if connected and button Y is pressed.
                        else if (currentState.Buttons.Y == ButtonState.Pressed)
                        {
                            player.addArrow(column, row, Variables.Direction.Up, gameboard, 4);
                        }

                        // Process input only if connected and button B is pressed.
                        else if (currentState.Buttons.B == ButtonState.Pressed)
                        {
                            player.addArrow(column, row, Variables.Direction.Right, gameboard, 3);
                        }

                        // Process input only if connected and Right Trigger is pulled.
                        else if (currentState.Triggers.Right > 0.5f)
                        {

                        }

                        // Process input only if connected and Left Trigger is pulled.
                        else if (currentState.Triggers.Left > 0.5f)
                        {
                            List<Cell> cells = new List<Cell>();
                            cells.AddRange(player.p_arrows);
                            foreach (Cell c in cells)
                            {
                                player.removeArrow(c, gameboard);
                            }
                        }
                    }
                }

                #region NoXboxController
                //FOLLOWING SECTION IS TO BE REMOVED, IT IS THE PLAYER 1 MOUSE DEBUG TOOL FOR DEVELOPER WITHOUT XBOX CONTROLLER

                KeyboardState keyboardStateCurrent = Keyboard.GetState(); //current state of the keyboard
                MouseState mouseStateCurrent = Mouse.GetState();  //current state of the mouse
                Vector2 position = new Vector2(mouseStateCurrent.X, mouseStateCurrent.Y);

                //If player presses LEFT key
                if (keyboardStateCurrent.IsKeyDown(Keys.Left))
                {
                    player.addArrow(position, Variables.Direction.Left, gameboard, 2);
                }

                //If player Presses UP key
                else if (keyboardStateCurrent.IsKeyDown(Keys.Up))
                {
                    player.addArrow(position, Variables.Direction.Up, gameboard, 4);
                }

                //If player Presses RIGHT key
                else if (keyboardStateCurrent.IsKeyDown(Keys.Right))
                {
                    player.addArrow(position, Variables.Direction.Right, gameboard, 3);
                }

                //If player presses DOWN key
                else if (keyboardStateCurrent.IsKeyDown(Keys.Down))
                {
                    player.addArrow(position, Variables.Direction.Down, gameboard, 1);
                }
                #endregion
            }
        }

        private void spawnGnomes(GameTime gameTime)
        {
            if (numOfGnomes < 50)
            {
                gnomeSpawnTimeRemaining -= gameTime.ElapsedGameTime.Milliseconds;
                evilGnomeSpawnTimeRemaining -= gameTime.ElapsedGameTime.Milliseconds;
                randomGnomeSpawnTimeRemaining -= gameTime.ElapsedGameTime.Milliseconds;
                List<Vector2> spawnPoints = gameboard.getSpawnPoints();

                if (evilGnomeSpawnTimeRemaining < 0)
                {
                    int spawnPoint = random.Next(4);
                    gnomeList.Add(new Gnome(evilGnomeTex, frameCount, (int)spawnPoints[spawnPoint].Y, 
                        (int)spawnPoints[spawnPoint].X, Variables.speed / 2));

                    evilGnomeSpawnTimeRemaining = random.Next(Variables.evilGnomeSpawnMin, Variables.evilGnomeSpawnMax);
                }
                else if (gnomeSpawnTimeRemaining < 0)
                {
                    int spawnPoint = random.Next(4);
                    gnomeList.Add(new Gnome(gnomeTex, frameCount, (int)spawnPoints[spawnPoint].Y, 
                        (int)spawnPoints[spawnPoint].X, Variables.speed));

                    gnomeSpawnTimeRemaining = random.Next(Variables.gnomeSpawnMin, Variables.gnomeSpawnMax);
                }
                else if (randomGnomeSpawnTimeRemaining < 0)
                {
                    int spawnPoint = random.Next(4);
                    gnomeList.Add(new Gnome(randomGnomeTex, frameCount, (int)spawnPoints[spawnPoint].Y,
                        (int)spawnPoints[spawnPoint].X, (int)(Variables.speed * 1.5)));

                    randomGnomeSpawnTimeRemaining = random.Next(Variables.randomGnomeSpawnMin, Variables.randomGnomeSpawnMax);
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
                        if (gnome.spritesheets == evilGnomeTex)
                        {
                            playerList[player].addPoints(-50);
                        }
                        else if (gnome.spritesheets == gnomeTex)
                        {
                            playerList[player].addPoints(10);
                        }
                        else if (gnome.spritesheets == randomGnomeTex)
                        {
                            playerList[player].addPoints(100);
                            randomEvent();
                        }

                        gnomeList.Remove(gnome);
                        --numOfGnomes;
                        break;
                    }
                }
            }
        }

        private void randomEvent()
        {
            gameboard.changeSpawns();
        }


        private void manageMenu()
        {
            //Find out where the mouse currently is at, change selection accordingly
            currentMainMenuIndex = -1;
            for (int i = 0; i < mainMenuItems.Length; i++)
            {
                MenuSelection z = mainMenuItems[i];
                MouseState currentMouseState = Mouse.GetState();

                if (currentMouseState.Y < z.GetMouseSelectionArea().Bottom
                    && currentMouseState.Y > z.GetMouseSelectionArea().Top
                    && currentMouseState.X < z.GetMouseSelectionArea().Right
                    && currentMouseState.X > z.GetMouseSelectionArea().Left)
                {
                    currentMainMenuIndex = i;
                }
            }

            //When the mouse clicks, pick the selection.
            MouseState mouse = Mouse.GetState();
            if ((mouse.LeftButton == ButtonState.Pressed && mPreviousMouseState.LeftButton == ButtonState.Released)  && currentMainMenuIndex != -1)
            {
                string selection = mainMenuItems[currentMainMenuIndex].GetTitle();
                if (selection == "Play")
                {
                    currentGameState = GameState.InGame;
                }
                else if (selection == "Instructions")
                {
                    currentGameState = GameState.Instructions;
                }
                else if (selection == "Exit")
                {
                    this.Exit();
                }
                else
                {
                    currentGameState = GameState.Credits;
                }
            }
            mPreviousMouseState = mouse;
        }
    }


    class MenuSelection
    {
        private string title;
        private Rectangle mouseSelectionBox;
        private Vector2 position;
        private int centerTextureWidth;
        private SpriteFont textFont;

        Texture2D iconTextureDimL;
        Texture2D iconTextureDimR;
        Texture2D iconTextureDimC;
        Texture2D iconTextureLitL;
        Texture2D iconTextureLitR;
        Texture2D iconTextureLitC;

        public MenuSelection(string title,
            Texture2D iconTextureDimL, Texture2D iconTextureDimR, Texture2D iconTextureDimC,
            Texture2D iconTextureLitL, Texture2D iconTextureLitR, Texture2D iconTextureLitC,
            int x, int y, int centerTextureWidth, SpriteFont textFont)
        {
            this.title = title;
            this.iconTextureDimL = iconTextureDimL;
            this.iconTextureDimR = iconTextureDimR;
            this.iconTextureDimC = iconTextureDimC;
            this.iconTextureLitL = iconTextureLitL;
            this.iconTextureLitR = iconTextureLitR;
            this.iconTextureLitC = iconTextureLitC;
            this.mouseSelectionBox = new Rectangle(x, y,
                iconTextureDimL.Width + iconTextureDimR.Width + centerTextureWidth,
                iconTextureDimC.Height);
            this.position = new Vector2(x, y);
            this.centerTextureWidth = centerTextureWidth;
            this.textFont = textFont;
        }

        public string GetTitle()
        {
            return title;
        }

        public Rectangle GetMouseSelectionArea()
        {
            return mouseSelectionBox;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, bool isSelected)
        {
            if (isSelected)
            {
                spriteBatch.Draw(iconTextureLitL, new Rectangle((int)position.X, (int)position.Y, iconTextureLitL.Width, iconTextureLitL.Height), Color.White);
                spriteBatch.Draw(iconTextureLitC, new Rectangle((int)position.X + iconTextureLitL.Width, (int)position.Y, centerTextureWidth, iconTextureLitC.Height), Color.White);
                spriteBatch.Draw(iconTextureLitR, new Rectangle((int)position.X + iconTextureLitL.Width + centerTextureWidth, (int)position.Y, iconTextureLitR.Width, iconTextureLitR.Height), Color.White);
                spriteBatch.DrawString(textFont, title,
                    new Vector2((int)position.X + +iconTextureLitL.Width + (centerTextureWidth / 2) - textFont.MeasureString(title).X / 2,
                        (int)position.Y + (iconTextureLitC.Height / 10)),
                        Color.Black);
            }
            else
            {
                spriteBatch.Draw(iconTextureDimL, new Rectangle((int)position.X, (int)position.Y, iconTextureDimL.Width, iconTextureDimL.Height), Color.White);
                spriteBatch.Draw(iconTextureDimC, new Rectangle((int)position.X + iconTextureDimL.Width, (int)position.Y, centerTextureWidth, iconTextureDimC.Height), Color.White);
                spriteBatch.Draw(iconTextureDimR, new Rectangle((int)position.X + iconTextureDimL.Width + centerTextureWidth, (int)position.Y, iconTextureLitR.Width, iconTextureDimR.Height), Color.White);
                spriteBatch.DrawString(textFont, title,
                    new Vector2((int)position.X + iconTextureDimL.Width + (centerTextureWidth / 2) - textFont.MeasureString(title).X / 2,
                        (int)position.Y + (iconTextureLitC.Height / 10)),
                    Color.Black);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, int alphaValue)
        {
            spriteBatch.Draw(iconTextureDimL, new Rectangle((int)position.X, (int)position.Y, iconTextureLitL.Width, iconTextureLitL.Height), new Color(255, 255, 255, (byte)MathHelper.Clamp(alphaValue, 0, 255)));
            spriteBatch.Draw(iconTextureDimC, new Rectangle((int)position.X + iconTextureLitL.Width, (int)position.Y, centerTextureWidth, iconTextureLitC.Height), new Color(255, 255, 255, (byte)MathHelper.Clamp(alphaValue, 0, 255)));
            spriteBatch.Draw(iconTextureDimR, new Rectangle((int)position.X + iconTextureLitL.Width + centerTextureWidth, (int)position.Y, iconTextureLitR.Width, iconTextureLitR.Height), new Color(255, 255, 255, (byte)MathHelper.Clamp(alphaValue, 0, 255)));
            spriteBatch.DrawString(textFont, title,
                new Vector2((int)position.X + +iconTextureLitL.Width + (centerTextureWidth / 2) - textFont.MeasureString(title).X / 2,
                    (int)position.Y + (iconTextureLitC.Height / 10)),
                    new Color(0, 0, 0, (byte)MathHelper.Clamp(alphaValue, 0, 255)));
        }
    }
}
