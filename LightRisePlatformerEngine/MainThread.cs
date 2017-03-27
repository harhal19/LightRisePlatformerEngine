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

        Action script1;
        public Action script2;
        Action script3;
        public GraphicsDeviceManager Graphics { get; protected set; }
        public SpriteBatch SpriteBatch;
        SpineObject SpineInstance;
        Texture2D Back;
        Texture2D BigDoor;
        bool Finish = false;
        Color finishColor = Color.White;

        public List<Instance> Instances = new List<Instance>( );
        public List<Instance> GUIes = new List<Instance>( );

        RenderTarget2D[ ] Renders;
        public Map Map { get; protected set; }
        Camera Cam;
        public Player Player { get; protected set; }
        public HackScreen HackScreen;
        public HackScreen FirstHackScreen;
        public HackScreen SecondHackScreen;
        public SpriteFont HackFont;
        public Texture2D Terminal;
        Door Door1;
        Door Door2;
        MainMenu mainMenu;

        public MainThread( ) {
            Graphics = new GraphicsDeviceManager(this);
            DisplayMode displayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
            IsMouseVisible = true;
#if DEBUG
            Graphics.IsFullScreen = false;
            Graphics.PreferredBackBufferWidth = 1024;
            Graphics.PreferredBackBufferHeight = 640;
            Cam = new Camera(new Vector2(0, 0), new Vector2(32f, 32f));
#else
            Graphics.IsFullScreen = true;
            Graphics.PreferredBackBufferWidth = displayMode.Width;
            Graphics.PreferredBackBufferHeight = displayMode.Height;
            Cam = new Camera(new Vector2(0, 0), new Vector2(64f, 64f));
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
            Tuple<Map, Point> tuple = WinUtils.LoadMap("Content/SampleFloor.lrmap");
            Map = tuple.Item1;
            Player = new Player(Map, tuple.Item2 + new Point(4, 1), GraphicsDevice);
            Instances.Add(new FirstComp(Player.Position.ToPoint() + new Point(25, -1), GraphicsDevice));
            Instances.Add(new SecondComp(Player.Position.ToPoint() + new Point(1, -1), GraphicsDevice));
            SimpleUtils.Init(GraphicsDevice);
            // TODO: Renders will be used for more fust drawing of the background... Later
            Renders = new RenderTarget2D[4];
            for (uint i = 0; i < Renders.Length; i++) {
                Renders[i] = new RenderTarget2D(GraphicsDevice, Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight);
            }

            base.Initialize( );
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent( ) {
            // Create a new SpriteBatch, which can be used to draw textures.
            //GUIes.Add(new FirstGUI(Graphics.GraphicsDevice, Graphics));
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            SpineInstance = new SpineObject(GraphicsDevice, "Sample", 1, new Vector2(20, 10));
            HackFont = Content.Load<SpriteFont>("HackFont");
            Terminal = Content.Load<Texture2D>("Terminal");
            Back = Content.Load<Texture2D>("SampleFloorBG");
            FirstHackScreen = new FirstHack(HackFont, SpriteBatch, Terminal, Instances[0] as Comp);
            FirstHack.Items = Player.Items;
            SecondHackScreen = new SecondHack(HackFont, SpriteBatch, Terminal, Instances[1] as Comp);
            SecondHack.Items = Player.Items;
            Texture2D doorText = Content.Load<Texture2D>("door");
            Texture2D computerTex = Content.Load<Texture2D>("Computer");
            (Instances[0] as Comp).texture = computerTex;
            (Instances[1] as Comp).texture = computerTex;
            Door1 = new Door(doorText, Player.Position.ToPoint() + new Point(22, -2), Instances[0] as Comp, Map);
            Door2 = new Door(doorText, Player.Position.ToPoint() + new Point(-1, -2), Instances[1] as Comp, Map);
            Player.Pick = Content.Load<SoundEffect>("Pick");
            (Instances[0] as Comp).Allowed = true;
            script1 = delegate ()
            {
                (Instances[0] as Comp).Allowed = false;
            };
            script2 = delegate ()
            {
                BigDoor = Content.Load<Texture2D>("BigDoor");
            };
            script3 = delegate ()
            {
                Finish = true;
                finishColor.A = 0;
            };
            //Player.GridPosition += new Point(4, 0);
            mainMenu = new MainMenu(Content.Load<Texture2D>("mainMenu"));
            MediaPlayer.Play(Content.Load<Song>("Music"));
            MediaPlayer.Volume = 0.6f;
            MediaPlayer.IsRepeating = true;
            // TODO: use this.Content to load your game content here*/
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

                //float cam_spd = 0.1f;
                /*float dx = (State.Keyboard.IsKeyDown(Keys.Right) ? cam_spd : 0) - (State.Keyboard.IsKeyDown(Keys.Left) ? cam_spd : 0);
                float dy = (State.Keyboard.IsKeyDown(Keys.Down) ? cam_spd : 0) - (State.Keyboard.IsKeyDown(Keys.Up) ? cam_spd : 0);*/
                //Cam.Position = new Vector2(Cam.Position.X + dx, Cam.Position.Y + dy);
                Player.Step(State);
                Player.Update(gameTime);
                /*Player.Hero.Update(gameTime);

                Player.Step(State);*/
                if (Player.Position.X > 40 && script1 != null)
                {
                    script1();
                    script1 = null;
                }
                if (Player.Position.X < 12 && Player.Position.Y < 11 && script3 != null)
                {
                    script3();
                    script3 = null;
                }
                //Cam.Position = Player.Position - Size.ToVector2() / Cam.Scale / 2f;
                if (HackScreen != null)
                    HackScreen.Update(gameTime, State);

                try
                {
                    foreach (var a in Instances)
                    {
                        a.Update(State);
                    }
                    foreach (var a in GUIes)
                    {
                        a.Update(State);
                    }
                }
                catch { }

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
                //SpriteBatch.Begin(transformMatrix: Matrix.CreateOrthographic(Cam.Scale.X, Cam.Scale.Y, -0.1f, 1f));
                //SpriteBatch.Begin(transformMatrix: Matrix.CreateOrthographicOffCenter(new Rectangle((int)(Cam.Position.X - Cam.Scale.X / 2), (int)(Cam.Position.Y - Cam.Scale.Y / 2), (int)Cam.Scale.X, (int)Cam.Scale.Y), 1f, 1000f));
                SpriteBatch.Begin();
                SpriteBatch.Draw(Back, new Rectangle(Cam.WorldToWindow(new Point(9, 1).ToVector2()), (new Point(80, 34).ToVector2() * Cam.Scale / 2f).ToPoint()), Color.White);
                //Map.Draw(SpriteBatch, Cam);
                Door1.Draw(SpriteBatch, Cam);
                Door2.Draw(SpriteBatch, Cam);
                foreach (var a in Instances)
                {
                    a.Draw(SpriteBatch, Cam);
                }
                if (BigDoor != null)
                    SpriteBatch.Draw(BigDoor, new Rectangle(Cam.WorldToWindow(new Vector2(11f, 6.7f)), (Cam.Scale * 2.3f).ToPoint()), Color.White);
                try
                {
                    SpriteBatch.End();
                }
                catch (InvalidOperationException) { }
                Player.Draw(SpriteBatch, Cam);

                SpriteBatch.Begin();
                foreach (var a in GUIes)
                {
                    a.Draw(SpriteBatch, Cam);
                }
                SpriteBatch.End();
                if (HackScreen != null)
                    HackScreen.Draw(Cam);
                if (Finish)
                {
                    SpriteBatch.Begin();
                    //if (finishColor.A < 100) finishColor.A++;
                    SpriteBatch.Draw(SimpleUtils.WhiteRect, new Rectangle(0, 0, SpriteBatch.GraphicsDevice.Viewport.Width, SpriteBatch.GraphicsDevice.Viewport.Height), Color.Black);
                    SpriteBatch.DrawString(HackFont, "Demo version finished", Vector2.One * 50, Color.Green);
                    SpriteBatch.End();
                }
            }
            else
                mainMenu.Draw(SpriteBatch);
            base.Draw(gameTime);

        }
    }
}
