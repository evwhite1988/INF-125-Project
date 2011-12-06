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
        ///////////////////////////////////// SYSTEM VARIABLES ///////////////////////////////////////////
        MouseState mPreviousMouseState;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private static Random random;

        ///////////////////////////////////// MENUS ///////////////////////////////////////////////////////

        enum GameState { MainMenu, InGame, Credits, Instructions };
        GameState currentGameState = GameState.MainMenu;
        int currentMainMenuIndex;
        int currentPauseMenuIndex;
        bool buffer = true;

        ///////////////////////////////////// GAME VARIABLES //////////////////////////////////////////////

        Gameboard gameboard;                    //Gameboard object

        //PLAYERS
        List<Player> playerList;
        private Player player1;                       
        private Player player2;
        private Player player3;
        private Player player4;

        //GNOMES
        List<Gnome> gnomeList { get; set; }     //List of Gnome Sprite Objects    
        int evilGnomeSpawnTimeRemaining;
        int gnomeSpawnTimeRemaining;
        int randomGnomeSpawnTimeRemaining;

        int gnomeScore = Variables.gnomeScore;
        int randScore = Variables.randScore;
        int evilScore = Variables.evilGnomeScore;

        int gnomeSpeed = Variables.speed;

        int timer = 120000;

        ///////////////////////////////////// TEXTURES ////////////////////////////////////////////////////////

        //11-3-11: changed this to be texture arrays, splitting up the different directional animations for use in the updated GameSprite.cs
        Texture2D[] gnomeTex;                     //Gnome textures
        Texture2D[] evilGnomeTex;                 //Evil Gnome Textures
        Texture2D[] randomGnomeTex;               //Random Gnome Textures
        Texture2D[] scoreboards;                  //Player scoreBoard textures
        Texture2D cursorTexp1;
        Texture2D cursorTexp2;
        Texture2D cursorTexp3;
        Texture2D cursorTexp4;
        Texture2D background;
        Texture2D[] instructions;
        Texture2D wallTexVerticle;
        Texture2D wallTexHorizontal;
        Texture2D creditText;                     //For the credits menu;
        Texture2D gnome_effect;
        private Texture2D[] base_effect;
        Texture2D[] sb_lit;
        SpriteFont gameChange;
        SpriteFont scoreFont;
        List<ParticleSystem> activeEffects;


        /////////////////////////////////SOUND//////////////////////////////////////////////////////////////////
        SoundEffect enterSound;
        Song bgm;

        //Main menu art files, courtesy of Sage's Scrolls
        MenuSelection[] mainMenuItems;  //MenuSelection class defined below. Tweaked and Reused from past games. 
        MenuSelection[] instructionOptions;
        MenuSelection[] pauseMenuItems;
        Texture2D mainMenuIconDimL;
        Texture2D mainMenuIconDimR;
        Texture2D mainMenuIconDimC;
        Texture2D mainMenuIconLitL;
        Texture2D mainMenuIconLitR;
        Texture2D mainMenuIconLitC;
        private bool isPlaying;
        private int instructionsPage = 0;
        private bool isPaused;
        private Texture2D pauseOverlay;


        /// <summary>
        /// CONSTRUCTOR
        /// </summary>
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
            isPaused = false;
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

            evilGnomeSpawnTimeRemaining = Variables.evilGnomeSpawnTime;
            gnomeSpawnTimeRemaining = Variables.gnomeSpawnTime;
            randomGnomeSpawnTimeRemaining = Variables.randomGnomeSpawnTime;

            mainMenuItems = new MenuSelection[4];
            instructionOptions = new MenuSelection[2];
            pauseMenuItems = new MenuSelection[3];
            currentPauseMenuIndex = 0;

            gnomeTex = new Texture2D[4];
            evilGnomeTex = new Texture2D[4];
            randomGnomeTex = new Texture2D[4];
            scoreboards = new Texture2D[4];
            instructions = new Texture2D[2];
            base_effect = new Texture2D[4];
            sb_lit = new Texture2D[4];

            activeEffects = new List<ParticleSystem>();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            loadTextures();
            enterSound = Content.Load<SoundEffect>("pickup2");
            bgm = Content.Load<Song>("bgm_music");
            MediaPlayer.IsRepeating = true;

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

            instructionOptions[0] = new MenuSelection("Controls", mainMenuIconDimL, mainMenuIconDimR, mainMenuIconDimC,
                mainMenuIconLitL, mainMenuIconLitR, mainMenuIconLitC, Window.ClientBounds.Width - 225 , 20, 100, scoreFont);
            instructionOptions[1] = new MenuSelection("Instructions", mainMenuIconDimL, mainMenuIconDimR, mainMenuIconDimC,
                mainMenuIconLitL, mainMenuIconLitR, mainMenuIconLitC, Window.ClientBounds.Width - 225, 20, 100, scoreFont);
           

            // Set up Pause Menu.
            initialY = (Window.ClientBounds.Height / 4) + 25;
            pauseMenuItems[0] = new MenuSelection("Resume", mainMenuIconDimL, mainMenuIconDimR, mainMenuIconDimC,
                mainMenuIconLitL, mainMenuIconLitR, mainMenuIconLitC, initialX, initialY, 350, scoreFont);
            pauseMenuItems[1] = new MenuSelection("Instructions", mainMenuIconDimL, mainMenuIconDimR, mainMenuIconDimC,
                mainMenuIconLitL, mainMenuIconLitR, mainMenuIconLitC, initialX, initialY + 90, 350, scoreFont);
            pauseMenuItems[2] = new MenuSelection("Return to Title", mainMenuIconDimL, mainMenuIconDimR, mainMenuIconDimC,
                mainMenuIconLitL, mainMenuIconLitR, mainMenuIconLitC, initialX, initialY + 180, 350, scoreFont);

            #endregion

            //Window fits to the gameboard.
            gameboard = new Gameboard();
            this.graphics.PreferredBackBufferWidth = gameboard.numberOfColumns * Variables.cellWidth;
            this.graphics.PreferredBackBufferHeight = (gameboard.numberOfRows * Variables.cellHeigth);

            initializePlayers();

            //increase screen size to fit scoreBoard
            this.graphics.PreferredBackBufferHeight = this.graphics.PreferredBackBufferHeight + scoreboards[0].Height;
        }

        /// <summary>
        /// Initializes all players
        /// </summary>
        private void initializePlayers()
        {
            //Initialize first player now that the texture is loaded. The position is manually set to the Tile with Tile ID -1, Although
            //this needs to be revised to place it at a specified home base. 

            player1 = new Player(Tile.player1Home,
                new Vector2(Tile.cellBorder.Width, Tile.cellBorder.Height),
                new Cursor(cursorTexp1, 1), PlayerIndex.One);
            playerList.Add(player1);

            player2 = new Player(Tile.player2Home,
                new Vector2(Tile.cellBorder.Width * 10, Tile.cellBorder.Height),
                new Cursor(cursorTexp2, 1), PlayerIndex.Two);
            playerList.Add(player2);

            player3 = new Player(Tile.player3Home,
                new Vector2(Tile.cellBorder.Width, Tile.cellBorder.Height * 7),
                new Cursor(cursorTexp3, 1), PlayerIndex.Three);
            playerList.Add(player3);

            player4 = new Player(Tile.player4Home,
                new Vector2(Tile.cellBorder.Width * 10, Tile.cellBorder.Height * 7),
                new Cursor(cursorTexp4, 1), PlayerIndex.Four);
            playerList.Add(player4);

            foreach (Player p in playerList)
                p.LoadContent(this.Content, this.graphics);
        }

        /// <summary>
        /// Loads All Textures
        /// </summary>
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
            cursorTexp1 = Content.Load<Texture2D>("cursor_p1");
            cursorTexp2 = Content.Load<Texture2D>("cursor_p2");
            cursorTexp3 = Content.Load<Texture2D>("cursor_p3");
            cursorTexp4 = Content.Load<Texture2D>("cursor_p4");
            gameChange = Content.Load<SpriteFont>("pointFont");
            instructions[0] = Content.Load<Texture2D>("instructions_1");
            instructions[1] = Content.Load<Texture2D>("instructions_2");
            pauseOverlay = Content.Load<Texture2D>("pauseMenuOverlay");
            gnome_effect = Content.Load<Texture2D>("random_effect");
            base_effect[0] = Content.Load<Texture2D>("p1_switch");
            base_effect[1] = Content.Load<Texture2D>("p2_switch");
            base_effect[2] = Content.Load<Texture2D>("p3_switch");
            base_effect[3] = Content.Load<Texture2D>("p1_switch");
            sb_lit[0] = Content.Load<Texture2D>("Player1_lit");
            sb_lit[1] = Content.Load<Texture2D>("Player2_lit");
            sb_lit[2] = Content.Load<Texture2D>("Player3_lit");
            sb_lit[3] = Content.Load<Texture2D>("Player4_lit");

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
        

        /// This is called when the game should draw itself.
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

            foreach (Gnome g in gnomeList)
            {
                if (g.hasEffect)
                    g.drawEffect(gameTime, spriteBatch);
            }

            foreach (ParticleSystem p in activeEffects)
                p.Draw(spriteBatch);

            graphics.ApplyChanges();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }


        /// Draws the Main Menu
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


        /// Draws the Instructions Menu
        private void drawInstructions(GameTime gameTime)
        {
            spriteBatch.Draw(instructions[instructionsPage], new Rectangle(0, (instructionsPage * 50) - 50, 
                instructions[instructionsPage].Width, instructions[instructionsPage].Height), Color.White);
            if (currentMainMenuIndex != -1)
                instructionOptions[instructionsPage].Draw(gameTime, spriteBatch, true);
            else
                instructionOptions[instructionsPage].Draw(gameTime, spriteBatch, false);
        }


        /// Draws the Credits Menu
        private void drawCredits()
        {
            Vector2 creditPos = new Vector2(0,
                     0);
            spriteBatch.Draw(creditText, creditPos, Color.White);
        }


        /// Calls methods to draw game content and gameplay updates
        private void drawGame(GameTime gameTime)
        {
            drawGameBoard(gameTime);
            drawGnomes(gameTime);
            drawPlayers(gameTime);
            if (isPaused)
            {
                spriteBatch.Draw(pauseOverlay, new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height), new Color(255, 255, 255, 200));
                for (int i = 0; i < pauseMenuItems.Length; i++)
                {
                    if (i == currentPauseMenuIndex)
                        pauseMenuItems[i].Draw(gameTime, spriteBatch, true);
                    else
                        pauseMenuItems[i].Draw(gameTime, spriteBatch, false);
                }
            }
        }


        /// Draws the gameboard, cells, walls, and timer
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

            spriteBatch.DrawString(gameChange, "TIME: " + (timer / 1000), new
            Vector2(350, 10), Color.Red);
        }


        /// Calls draw() on each gnome on the gameboard
        private void drawGnomes(GameTime gameTime)
        {
            foreach (Gnome gnome in gnomeList)
            {
                gnome.DrawFrame(spriteBatch, gnome.direction);
            }
        }


        /// Calls draw() for each active player
        private void drawPlayers(GameTime gameTime)
        {

            foreach (Player player in playerList)
            {
                player.cursor.DrawFrame(spriteBatch, Variables.Direction.None);
                player.draw(spriteBatch, scoreFont);
            }
        }

        ///////////////////////////////// UPDATE METHODS ////////////////////////////////////////////////////////


        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardStateCurrent = Keyboard.GetState(); //Will remove later, for debugging menu
            GamePadState currentState = GamePad.GetState(PlayerIndex.One);
            switch (currentGameState)
            {
                case GameState.MainMenu:
                    isPlaying = false;
                    manageMenu();
                    break;

                case GameState.Credits:
                    isPlaying = false;

                    if (currentState.IsConnected)
                    {
                        if (currentState.IsButtonDown(Buttons.A))
                        {
                            if (instructionsPage == 0)
                                instructionsPage++;
                            else
                                --instructionsPage;
                        }
                    }
                    if (keyboardStateCurrent.IsKeyDown(Keys.Escape) || currentState.IsButtonDown(Buttons.B))
                    {
                        currentGameState = GameState.MainMenu;
                    }
                    break;

                case GameState.Instructions:
                    isPlaying = false;
                    manageInstructions();
                    if (keyboardStateCurrent.IsKeyDown(Keys.Escape) || currentState.IsButtonDown(Buttons.B))
                    {
                        currentGameState = GameState.MainMenu;
                    }
                    break;


                case GameState.InGame:
                    if (!isPlaying)
                    {
                        MediaPlayer.Play(bgm);
                        isPlaying = true;
                    }
                    if (keyboardStateCurrent.IsKeyDown(Keys.Escape) || currentState.IsButtonDown(Buttons.Start))
                    {
                        isPaused = !isPaused;
                    }

                    if (isPaused)
                    {
                        //GAMEPAD INPUT
                        //If Player presses UP on left thumbstick
                        if (currentState.ThumbSticks.Left.Y == 0.0f && buffer == false)
                        {
                            buffer = true;
                        }

                        //If Player presses UP on left thumbstick
                        else if (currentState.ThumbSticks.Left.Y > 0.0f && buffer == true)
                        {
                            if (currentPauseMenuIndex > 0)
                            {
                                currentPauseMenuIndex = currentPauseMenuIndex - 1;
                                buffer = false;
                            }
                        }

                        //If Player presses DOWN on left thumbstick
                        else if (currentState.ThumbSticks.Left.Y < 0.0f && buffer == true)
                        {
                            if (currentPauseMenuIndex < pauseMenuItems.Length - 1)
                            {
                                currentPauseMenuIndex = currentPauseMenuIndex + 1;
                                buffer = false;
                            }
                        }

                        //MOUSE INPUT
                        for (int i = 0; i < pauseMenuItems.Length; i++)
                        {
                            MenuSelection z = pauseMenuItems[i];
                            MouseState currentMouseState = Mouse.GetState();

                            if (currentMouseState.Y < z.GetMouseSelectionArea().Bottom
                                && currentMouseState.Y > z.GetMouseSelectionArea().Top
                                && currentMouseState.X < z.GetMouseSelectionArea().Right
                                && currentMouseState.X > z.GetMouseSelectionArea().Left)
                            {
                                currentPauseMenuIndex = i;
                            }
                        }

                        MouseState mouse = Mouse.GetState();
                        currentState = GamePad.GetState(PlayerIndex.One);

                        if (((mouse.LeftButton == ButtonState.Pressed && mPreviousMouseState.LeftButton == ButtonState.Released) || 
                            currentState.Buttons.A == ButtonState.Pressed) && currentPauseMenuIndex != -1)
                        {
                            string selection = pauseMenuItems[currentPauseMenuIndex].GetTitle();
                            if (selection == "Resume")
                            {
                                isPaused = false;
                            }
                            else if (selection == "Instructions")
                            {
                                currentGameState = GameState.Instructions;
                            }
                            else if (selection == "Return to Title")
                            {
                                currentGameState = GameState.MainMenu;
                                disposeGame();
                                isPaused = false;
                                MediaPlayer.Stop();
                            }
                        }
                        mPreviousMouseState = mouse;
                    }
                    else
                    {
                        timer -= gameTime.ElapsedGameTime.Milliseconds;
                        if (checkGameEnd()) endGame();
                        removeGnomes(gameTime);             //Removes gnomes
                        spawnGnomes(gameTime);      //Spawns new gnomes
                        UpdateInput();              //handles new user inputs

                        // For each gnome on the gameboard, update its position
                        foreach (Gnome gnome in gnomeList)
                        {
                            gnome.updatePosition(gameboard, gameTime);
                        }

                        for (int i = 0; i < activeEffects.Count; i++)
                        {
                            activeEffects[i].Update(gameTime);
                            if (activeEffects[i].isFinsihed())
                                activeEffects.RemoveAt(i);
                        }
                    }
                    base.Update(gameTime);
                    break;
            }
        }

        ///////////////////////////////// INPUT METHODS ////////////////////////////////////////////////////////

        /// Player Input Handler
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
                        {
                            player.cursor.updatePosition(Variables.Direction.Up);
                        }
                    }

                    //If Player presses DOWN on left thumbstick
                    if (currentState.ThumbSticks.Left.Y < 0.0f)
                    {
                        if (player.cursor.getSpriteCenter().Y <= (gameboard.numberOfRows - 1) * Variables.cellHeigth + Variables.cellHeigth / 2)
                        {
                            player.cursor.updatePosition(Variables.Direction.Down);
                        }
                    }

                    //If Player presses RIGHT on left thumbstick
                    if (currentState.ThumbSticks.Left.X > 0.0f)
                    {
                        if (player.cursor.getSpriteCenter().X <= (gameboard.numberOfColumns - 1) * Variables.cellWidth + Variables.cellWidth / 2)
                        {
                            player.cursor.updatePosition(Variables.Direction.Right);
                        }
                    }

                    //If Player presses LEFT on left thumbstick
                    if (currentState.ThumbSticks.Left.X < 0.0f)
                    {
                        if (player.cursor.getSpriteCenter().X >= Variables.cellWidth / 2)
                        {
                            player.cursor.updatePosition(Variables.Direction.Left);
                        }
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


        /// Manages the Inputs while on Main Menu
        private void manageMenu()
        {
           //GAMEPAD INPUT
            GamePadState currentState = GamePad.GetState(PlayerIndex.One);
            
            //If Player presses UP on left thumbstick
            if (currentState.ThumbSticks.Left.Y == 0.0f && buffer == false)
            {
                buffer = true;
            }
            
            //If Player presses UP on left thumbstick
            else if (currentState.ThumbSticks.Left.Y > 0.0f && buffer == true)
            {
                if (currentMainMenuIndex > 0)
                {
                    currentMainMenuIndex = currentMainMenuIndex - 1;
                    buffer = false;
                }
            }

            //If Player presses DOWN on left thumbstick
            else if (currentState.ThumbSticks.Left.Y < 0.0f && buffer == true)
            {
                if (currentMainMenuIndex < mainMenuItems.Length - 1)
                {
                    currentMainMenuIndex = currentMainMenuIndex + 1;
                    buffer = false;
                }
            }

            //MOUSE INPUT
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

            //MENU ITEM SELECTION
            MouseState mouse = Mouse.GetState();
            if (((mouse.LeftButton == ButtonState.Pressed && mPreviousMouseState.LeftButton == ButtonState.Released) ||
                currentState.Buttons.A == ButtonState.Pressed) && currentMainMenuIndex != -1)
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

        
        private void manageInstructions()
        {
            //Find out where the mouse currently is at, change selection accordingly
            currentMainMenuIndex = -1;
            for (int i = 0; i < instructionOptions.Length; i++)
            {
                MenuSelection z = instructionOptions[i];
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
            if ((mouse.LeftButton == ButtonState.Pressed && mPreviousMouseState.LeftButton == ButtonState.Released) && currentMainMenuIndex != -1)
            {
                if (instructionsPage > 0)
                    --instructionsPage;
                else
                    instructionsPage = 1;
            }
            mPreviousMouseState = mouse;
        }


        ////////////////////////////// HELPER METHODS ////////////////////////////////////////////////////////////
       
        private void endGame()
        {
            isPaused = true;
           
            while (true)
            {
                GamePadState currentState = GamePad.GetState(PlayerIndex.One);
                MouseState mouse = Mouse.GetState();

                if (currentState.Buttons.A == ButtonState.Pressed || mouse.LeftButton == ButtonState.Pressed)
                {
                    break;
                }
            }
            disposeGame();
            currentGameState = GameState.Credits;
        }

        public static float RandomBetween(float min, float max)
        {
            return min + (float)random.NextDouble() * (max - min);
        }

        private bool checkGameEnd()
        {
            if (timer <= 0) return true;
            else return false;
        }

        private void spawnGnomes(GameTime gameTime)
        {
            if (gnomeList.Count < 50)
            {
                gnomeSpawnTimeRemaining -= gameTime.ElapsedGameTime.Milliseconds;
                evilGnomeSpawnTimeRemaining -= gameTime.ElapsedGameTime.Milliseconds;
                randomGnomeSpawnTimeRemaining -= gameTime.ElapsedGameTime.Milliseconds;
                List<Vector2> spawnPoints = gameboard.getSpawnPoints();

                if (evilGnomeSpawnTimeRemaining < 0)
                {
                    int spawnPoint = random.Next(4);
                    gnomeList.Add(new Gnome(evilGnomeTex, Variables.frameCount, (int)spawnPoints[spawnPoint].Y,
                        (int)spawnPoints[spawnPoint].X, gnomeSpeed / 2));

                    evilGnomeSpawnTimeRemaining = random.Next(Variables.evilGnomeSpawnMin, Variables.evilGnomeSpawnMax);
                }
                else if (gnomeSpawnTimeRemaining < 0)
                {
                    int spawnPoint = random.Next(4);
                    gnomeList.Add(new Gnome(gnomeTex, Variables.frameCount, (int)spawnPoints[spawnPoint].Y,
                        (int)spawnPoints[spawnPoint].X, gnomeSpeed));

                    gnomeSpawnTimeRemaining = random.Next(Variables.gnomeSpawnMin, Variables.gnomeSpawnMax);
                }
                else if (randomGnomeSpawnTimeRemaining < 0)
                {
                    int spawnPoint = random.Next(4);
                    Gnome gnome = new Gnome(randomGnomeTex, Variables.frameCount, (int)spawnPoints[spawnPoint].Y,
                        (int)spawnPoints[spawnPoint].X, (int)(gnomeSpeed * 1.5));
                    gnome.addEffect(gameTime, gnome_effect);
                    gnomeList.Add(gnome);
                    randomGnomeSpawnTimeRemaining = random.Next(Variables.randomGnomeSpawnMin, Variables.randomGnomeSpawnMax);
                }
            }

        }

        private void removeGnomes(GameTime gameTime)
        {
            List<Cell> bases = gameboard.getBases();
            foreach (Cell currentBase in bases)
            {
                foreach (Gnome gnome in gnomeList)
                {
                    if (gnome.getCurrentRow(gameboard) == currentBase.getPositionX() && gnome.getCurrentColumn(gameboard) == currentBase.getPositionY())
                    {
                        enterSound.Play();
                        int player = currentBase.getOwnedBy();
                        if (gnome.spritesheets == evilGnomeTex)
                        {
                            playerList[player -1].addPoints(evilScore, scoreboards[player -1]);
                        }
                        else if (gnome.spritesheets == gnomeTex)
                        {
                            playerList[player - 1].addPoints(gnomeScore, sb_lit[player -1]);
                        }
                        else if (gnome.spritesheets == randomGnomeTex)
                        {
                            playerList[player - 1].addPoints(randScore, sb_lit[player -1]);
                            randomEvent(gameTime);
                        }

                        gnomeList.Remove(gnome);
                        break;
                    }
                }
            }
        }

        private void randomEvent(GameTime gameTime)
        {
            normalizeVariables(gameTime);

            int e = random.Next(3);

            switch (e)
            {
                case 0:
                    swapBases(gameTime);
                    break;
                case 1:
                    doubleScore(gameTime);
                    break;
                case 2:
                    doubleSpeed(gameTime);
                    break;
            }

            //swapBases(gameTime);
        }

        private void normalizeVariables(GameTime gameTime)
        {
            gnomeScore = Variables.gnomeScore;
            evilScore = Variables.evilGnomeScore;
            gnomeSpeed = Variables.speed;
        }

        private void doubleSpeed(GameTime gameTime)
        {
            gnomeSpeed = (int)(gnomeSpeed * 1.5);
            foreach (Gnome g in gnomeList)
            {
                ParticleSystem effect = new ParticleSystem();
                effect.Init(gameTime, base_effect[0], g, 1000, 1.0f, 1.0f);
                activeEffects.Add(effect);
            }

        }


        private void doubleScore(GameTime gameTime)
        {
            gnomeScore = 50;
            evilScore = -300;

            for(int i = 0; i < playerList.Count; i++)
            {
                ParticleSystem effect = new ParticleSystem();
                effect.Init(gameTime, base_effect[i], playerList[i].scoreBoard, 2000, 1000.0f, 5.0f);
                activeEffects.Add(effect);
            }

        }

        public void swapBases(GameTime gameTime)
        {
            List<Cell> bases = gameboard.getBases();
            foreach (Player p in playerList)
            {
                Player temp = getRandomPlayer();
                while (temp.Equals(p))
                    temp = getRandomPlayer();

                Vector2 base1_coord = p.playerBase.coord;
                p.playerBase.coord = temp.playerBase.coord;
                int row1 = ((int)(p.playerBase.coord.Y) / Variables.cellHeigth);
                int col1 = ((int)(p.playerBase.coord.X) / Variables.cellWidth);
                gameboard.getCell(row1, col1).setOwnedBy(p.index);

                temp.playerBase.coord = base1_coord;
                int row2 = ((int)(temp.playerBase.coord.Y) / Variables.cellHeigth);
                int col2 = ((int)(temp.playerBase.coord.X) / Variables.cellWidth);
                gameboard.getCell(row2, col2).setOwnedBy(temp.index);

            }

            for (int i = 0; i < playerList.Count; i++)
            {
                ParticleSystem effect = new ParticleSystem();
                effect.Init(gameTime, base_effect[i], playerList[i].playerBase, 3000, 1.0f, 5.0f);
                activeEffects.Add(effect);
            }
        }

        private Player getRandomPlayer()
        {
            int i = random.Next(playerList.Count() + 1);

            switch (i)
            {
                case 1:
                    return player1;

                case 2:
                    return player2;

                case 3:
                    return player3;

                case 4:
                    return player4;

                default:
                    return player1;
            }
        }

        private void disposeGame()
        {
            gnomeList.Clear();
            foreach (Player p in playerList)
            {
                p.dispose();
            }
            gameboard = new Gameboard();
            timer = 120000;
            gnomeSpawnTimeRemaining = Variables.gnomeSpawnTime;
            evilGnomeSpawnTimeRemaining = Variables.evilGnomeSpawnTime;
            randomGnomeSpawnTimeRemaining = Variables.randomGnomeSpawnTime;
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
                spriteBatch.Draw(iconTextureLitL, new Rectangle((int)position.X - 20, (int)position.Y, iconTextureLitL.Width, iconTextureLitL.Height), Color.White);
                spriteBatch.Draw(iconTextureLitC, new Rectangle((int)position.X - 20 + iconTextureLitL.Width, (int)position.Y, centerTextureWidth, iconTextureLitC.Height), Color.White);
                spriteBatch.Draw(iconTextureLitR, new Rectangle((int)position.X - 20 + iconTextureLitL.Width + centerTextureWidth, (int)position.Y, iconTextureLitR.Width, iconTextureLitR.Height), Color.White);
                spriteBatch.DrawString(textFont, title,
                    new Vector2((int)position.X - 20 + +iconTextureLitL.Width + (centerTextureWidth / 2) - textFont.MeasureString(title).X / 2,
                        (int)position.Y + (iconTextureLitC.Height / 10)),
                        Color.Black);
            }
            else
            {
                spriteBatch.Draw(iconTextureDimL, new Rectangle((int)position.X - 20, (int)position.Y, iconTextureDimL.Width, iconTextureDimL.Height), Color.White);
                spriteBatch.Draw(iconTextureDimC, new Rectangle((int)position.X - 20 + iconTextureDimL.Width, (int)position.Y, centerTextureWidth, iconTextureDimC.Height), Color.White);
                spriteBatch.Draw(iconTextureDimR, new Rectangle((int)position.X - 20 + iconTextureDimL.Width + centerTextureWidth, (int)position.Y, iconTextureLitR.Width, iconTextureDimR.Height), Color.White);
                spriteBatch.DrawString(textFont, title,
                    new Vector2((int)position.X - 20 + iconTextureDimL.Width + (centerTextureWidth / 2) - textFont.MeasureString(title).X / 2,
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
