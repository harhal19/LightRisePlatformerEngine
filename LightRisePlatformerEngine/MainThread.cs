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
        public GraphicsDeviceManager Graphics { get; protected set; }
        public SpriteBatch SpriteBatch;
        Level level;
        public Player Player { get; protected set; }
        Texture2D soundOn;
        Texture2D soundOff;
        MainMenu mainMenu;
        StepState State;

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
            MediaPlayer.IsMuted = Settings.Default.SoundOn;
            soundOn = Content.Load<Texture2D>("soundOn");
            soundOff = Content.Load<Texture2D>("soundOff");
            MediaPlayer.Play(Content.Load<Song>("Music"));
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
            if (State == null)
                State = new StepState(gameTime, Keyboard.GetState(), Mouse.GetState());
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && State.Mouse.LeftButton == ButtonState.Released)
                if (new Rectangle(GraphicsDevice.Viewport.Width - 10 - 176 * 2, 10, 176 * 2, 50 * 2).Contains(Mouse.GetState().Position))
                {
                    MediaPlayer.IsMuted = !MediaPlayer.IsMuted;
                    Settings.Default.SoundOn = MediaPlayer.IsMuted;
                    Settings.Default.Save();
                }
            State = new StepState(gameTime, Keyboard.GetState(), Mouse.GetState());
            if (mainMenu == null)
            {
                if (State.Keyboard.IsKeyDown(Keys.Escape))
                {
                    mainMenu = new MainMenu(Content.Load<Texture2D>("mainMenu"));
                    generate();
                }
                Player.Step(State);
                level.Update(gameTime);
            }
            else
            {
                if (State.Keyboard.IsKeyDown(Keys.Escape))
                    Exit();
                else
                if (State.Keyboard.GetPressedKeys().Length > 0)
                    mainMenu = null;
            }
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
            SpriteBatch.Begin();
            if (MediaPlayer.IsMuted)
                SpriteBatch.Draw(soundOff, new Rectangle(GraphicsDevice.Viewport.Width - 10 - 176 * 2, 10, 176 * 2, 50 * 2), Color.White);
            else
                SpriteBatch.Draw(soundOn, new Rectangle(GraphicsDevice.Viewport.Width - 10 - 176 * 2, 10, 176 * 2, 50 * 2), Color.White);
            SpriteBatch.End();
            base.Draw(gameTime);

        }
    }
}
