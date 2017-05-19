using LightRise.BaseClasses;
using LightRise.WinUtilsLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpineTester
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpineObject testSpine;
        Form1 dialogForm;
        Camera Cam;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        void LoadSpine(string fileName)
        {
            testSpine = new SpineObject(GraphicsDevice, fileName.Substring(0, fileName.Length - 5), 1, Vector2.Zero, "head", true);
        }

        void LoadAnimation(string name, bool loop)
        {
            testSpine.State.SetAnimation(0, name, loop);
        }

        void SetScale(float scale)
        {
            Cam.Scale = Vector2.One * scale;
            //testSpine.Scale = scale;
        }

        string[] GetAnimationArray()
        {
            string[] animationsArray = new string[testSpine.Skeleton.Data.Animations.Items.Length];
            for (int i = 0; i < animationsArray.Length; i++)
                animationsArray[i] = testSpine.Skeleton.Data.Animations.Items[i].Name;
            return animationsArray;
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
            dialogForm = new Form1(LoadSpine, LoadAnimation, SetScale, GetAnimationArray, testSpine);
            dialogForm.Show();
            Cam = new Camera( - GraphicsDevice.Viewport.Bounds.Size.ToVector2() / 2, Vector2.One);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            Cam.Position = -GraphicsDevice.Viewport.Bounds.Size.ToVector2() / Cam.Scale.X / 2;
            if (testSpine != null)
                testSpine.Update(gameTime);

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            if (testSpine != null)
                testSpine.Draw(Cam);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
