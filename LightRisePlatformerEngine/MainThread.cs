using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using LightRise.BaseClasses;
using LightRise.WinUtilsLib;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace LightRise.Main {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MainThread : Game {

        //Action script1;
        //public Action script2;
        //Action script3;
        public GraphicsDeviceManager Graphics { get; protected set; }
        public SpriteBatch SpriteBatch;
        Level level;
        //SpineObject SpineInstance;
        //Texture2D BigDoor;
        //bool Finish = false;
        //Color finishColor = Color.White;

        //public List<Instance> Instances = new List<Instance>( );
        //public List<Instance> GUIes = new List<Instance>( );

        //RenderTarget2D[ ] Renders;
        //public Map Map { get; protected set; }
        public Player Player { get; protected set; }
        /*public HackScreen HackScreen;
        public HackScreen FirstHackScreen;
        public HackScreen SecondHackScreen;
        public SpriteFont HackFont;
        public Texture2D Terminal;
        Door Door1;
        Door Door2;*/
        MainMenu mainMenu;

        public MainThread( ) {
            Graphics = new GraphicsDeviceManager(this);
            DisplayMode displayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
            IsMouseVisible = true;
#if DEBUG
            Graphics.IsFullScreen = false;
            Graphics.PreferredBackBufferWidth = 1024;
            Graphics.PreferredBackBufferHeight = 640;
#else
            Graphics.IsFullScreen = true;
            Graphics.PreferredBackBufferWidth = displayMode.Width;
            Graphics.PreferredBackBufferHeight = displayMode.Height;
#endif
            Content.RootDirectory = "Content";
        }

        public int Width { get { return Graphics.PreferredBackBufferWidth; } }
        public int Height { get { return Graphics.PreferredBackBufferHeight; } }
        public Point Size { get { return new Point(Width, Height); } }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize( ) {

            base.Initialize( );
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        /// 

        void generate()
        {
            level = new Level1(Content, Graphics);
            Player = (level as Level1).Player;
            (level as Level1).NextLevel += () =>
            {
                level = new Level2(Content, Graphics);
                Player = (level as Level2).Player;
                (level as Level2).NextLevel += () =>
                {
                    mainMenu = new MainMenu(Content.Load<Texture2D>("mainMenu"));
                    generate();
                };
            };
            (level as Level1).Restart += generate;
        }

        protected override void LoadContent( ) {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            mainMenu = new MainMenu(Content.Load<Texture2D>("mainMenu"));
            generate();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent( ) {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            StepState State = new StepState(gameTime, Keyboard.GetState(), Mouse.GetState());
            if (mainMenu == null)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed /*|| State.Keyboard.IsKeyDown(Keys.Escape)*/)
                    Exit();
                Player.Step(State);
                level.Update(gameTime);
            }
            else
                if (State.Keyboard.GetPressedKeys().Length > 0)
                    mainMenu = null;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            if (mainMenu == null)
            {
                level.Draw(gameTime, SpriteBatch);
            }
            else
                mainMenu.Draw(SpriteBatch);
            base.Draw(gameTime);

        }
    }
}
