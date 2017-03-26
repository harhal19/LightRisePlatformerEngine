using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using LightRise.BaseClasses;
using LightRise.WinUtilsLib;
using System;

namespace LightRise.Main
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MainThread : Game
    {

        GraphicsDeviceManager Graphics;
        SpriteBatch SpriteBatch;

        RenderTarget2D[] Renders;
        public Map Map { get; protected set; }
        Camera Cam;
        Player Player;
        HackScreen hackScreen;

        public MainThread()
        {
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
            Tuple<Map, Point> tuple = WinUtils.LoadMap("Content/asd.lrmap");
            Map = tuple.Item1;
            Player = new Player(tuple.Item2);
            Player.SetHero(GraphicsDevice, 1 / 260f);
            Cam = new Camera(new Vector2(0, 0), new Vector2(32f, 32f));
            SimpleUtils.Init(GraphicsDevice);
            // TODO: Renders will be used for more fust drawing of the background... Later
            Renders = new RenderTarget2D[4];
            for (uint i = 0; i < Renders.Length; i++)
            {
                Renders[i] = new RenderTarget2D(GraphicsDevice, Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight);
            }

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            hackScreen = new FirstHack(Content.Load<SpriteFont>("HackFont"), SpriteBatch, Content.Load<Texture2D>("Terminal"));

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            StepState State = new StepState(gameTime, Keyboard.GetState(), Mouse.GetState());

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || State.Keyboard.IsKeyDown(Keys.Escape))
                Exit();

            float cam_spd = 0.1f;
            float dx = (State.Keyboard.IsKeyDown(Keys.Right) ? cam_spd : 0) - (State.Keyboard.IsKeyDown(Keys.Left) ? cam_spd : 0);
            float dy = (State.Keyboard.IsKeyDown(Keys.Down) ? cam_spd : 0) - (State.Keyboard.IsKeyDown(Keys.Up) ? cam_spd : 0);
            Cam.Position = new Vector2(Cam.Position.X + dx, Cam.Position.Y + dy);
            Player.Hero.Update(gameTime);

            Player.Step(State);
            Cam.Position = Player.Position - Size.ToVector2( ) / Cam.Scale / 2f;
            hackScreen.Update(gameTime, State);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            //SpriteBatch.Begin(transformMatrix: Matrix.CreateOrthographic(Cam.Scale.X, Cam.Scale.Y, -0.1f, 1f));
            //SpriteBatch.Begin(transformMatrix: Matrix.CreateOrthographicOffCenter(new Rectangle((int)(Cam.Position.X - Cam.Scale.X / 2), (int)(Cam.Position.Y - Cam.Scale.Y / 2), (int)Cam.Scale.X, (int)Cam.Scale.Y), 1f, 1000f));
            SpriteBatch.Begin();
            //Map.Draw(SpriteBatch, Cam);
            SpriteBatch.End( );
            //Player.Draw(SpriteBatch, Cam);
            //spineObj.Draw(Cam);
            hackScreen.Draw(Cam);

            base.Draw(gameTime);
        }
    }
}
