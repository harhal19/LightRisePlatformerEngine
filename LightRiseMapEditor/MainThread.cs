using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using LightRise.BaseClasses;
using LightRise.WinUtilsLib;
using System;
using System.Collections.Generic;

namespace LightRise.MapEditor {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MainThread : Game {

        public static readonly Point MAP_SIZE;
        static MainThread( ) {

            // ATTENSION: MAP_SIZE must be only positive
            MAP_SIZE = new Point(64, 64);

        }

        List<Interaction> ListOfInstances = new List<Interaction>( );
        GraphicsDeviceManager Graphics;
        SpriteBatch SpriteBatch;
        Texture2D Back;

        public Point MousePosition = new Point( );
        public Point MousePreviousPosition;
        KeyboardState PreviousKeyboardState;

        public Point PlayerPosition;

        Camera Cam;
        public uint CurrentValue = 1;

        Map[ ][ ] Maps;

        Point SelectedMap;
        Point SelectedPoint;

        public MainThread( ) {
            Graphics = new GraphicsDeviceManager(this);
            IsMouseVisible = true;
            Mouse.WindowHandle = Window.Handle;
            Content.RootDirectory = "Content";
            Graphics.PreferredBackBufferWidth = 1600;
            Graphics.PreferredBackBufferHeight = 900;
            Graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize( ) {
            Maps = SimpleUtils.Create2DArray(1, 1, new Map(MAP_SIZE));
            //Maps[0][0].Randomize( );
            PlayerPosition = new Point(12, 7);
            for (uint x = 10; x <= 14; x++) {
                Maps[0][0][x, 9] = Map.WALL;
            }
            Cam = new Camera(new Vector2(0, 0), new Vector2(16f, 16f));
            SimpleUtils.Init(GraphicsDevice);

            base.Initialize( );
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent( ) {
            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            Back = Content.Load<Texture2D>("L1");

            // TODO: use this.Content to load your game content here
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
        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState( ).IsKeyDown(Keys.Escape))
                Exit( );

            MouseState MouseState = Mouse.GetState( );
            KeyboardState KeyboardState = Keyboard.GetState( );

            MousePreviousPosition = MousePosition;
            MousePosition = MouseState.Position;
            if (MouseState.RightButton == ButtonState.Pressed) {
                Cam.Position = Cam.Position + (MousePreviousPosition - MousePosition).Vector2( ) / Cam.Scale;
            }

            Vector2 MouseWorldPosition = MousePosition.Vector2( ) / Cam.Scale + Cam.Position;
            SelectedMap = MouseWorldPosition.FloorToPoint( ) / MAP_SIZE;
            SelectedPoint = MouseWorldPosition.FloorToPoint( ).Mod(MAP_SIZE);
            if (MouseWorldPosition.X < 0) {
                SelectedMap.X -= 1;
                SelectedPoint.X = MAP_SIZE.X + SelectedPoint.X;
            }
            if (MouseWorldPosition.Y < 0) {
                SelectedMap.Y -= 1;
                SelectedPoint.Y = MAP_SIZE.Y + SelectedPoint.Y;
            }

            if (KeyboardState.IsKeyDown(Keys.D1)) {
                CurrentValue = Map.WALL;
            } else if (KeyboardState.IsKeyDown(Keys.D2)) {
                CurrentValue = Map.LEFT_SHELF;
            } else if (KeyboardState.IsKeyDown(Keys.D3)) {
                CurrentValue = Map.RIGHT_SHELF;
            } else if (KeyboardState.IsKeyDown(Keys.D4)) {
                CurrentValue = Map.LADDER;
            } else if (KeyboardState.IsKeyDown(Keys.Tab)) {
                CurrentValue = Map.EMPTY;
            } else if (KeyboardState.IsKeyDown(Keys.OemTilde)) {
                try {
                    /*string ID = Interaction.InputBox("ID of the object", "Input", "");
                    foreach (var a in ListOfInstances) {
                        if (a.ID == ID) {
                            throw new Exception("It is not possible to create an object with this ID");
                        }
                    }
                    ListOfInstances.Add(new Interaction(SelectedMap * MAP_SIZE + SelectedPoint, ID));*/
                } catch { }
            }

            if (MouseState.LeftButton == ButtonState.Pressed) {
                { // TODO: Refactor and optimize this code-block later
                    while (SelectedMap.X < 0) {
                        Map[ ][ ] newMaps = SimpleUtils.Create2DArray<Map>((uint)Maps.Length + 1, (uint)Maps[0].Length, null);
                        for (uint i = 0; i < Maps.Length; i++) {
                            for (uint j = 0; j < Maps[0].Length; j++) {
                                newMaps[i + 1][j] = Maps[i][j];
                            }
                        }
                        for (uint y = 0; y < newMaps[0].Length; y++) {
                            newMaps[0][y] = new Map(MAP_SIZE);
                        }
                        Maps = newMaps;
                        Cam.Position += new Vector2(MAP_SIZE.X, 0f);
                        SelectedMap.X += 1;
                        PlayerPosition.X += MAP_SIZE.X;
                    }
                    while (SelectedMap.Y < 0) {
                        Map[ ][ ] newMaps = SimpleUtils.Create2DArray<Map>((uint)Maps.Length, (uint)Maps[0].Length + 1, null);
                        for (uint i = 0; i < Maps.Length; i++) {
                            for (uint j = 0; j < Maps[0].Length; j++) {
                                newMaps[i][j + 1] = Maps[i][j];
                            }
                        }
                        for (uint x = 0; x < newMaps.Length; x++) {
                            newMaps[x][0] = new Map(MAP_SIZE);
                        }
                        Maps = newMaps;
                        Cam.Position += new Vector2(0f, MAP_SIZE.Y);
                        SelectedMap.Y += 1;
                        PlayerPosition.Y += MAP_SIZE.X;
                    }
                    while (SelectedMap.X >= Maps.Length) {
                        Map[ ][ ] newMaps = SimpleUtils.Create2DArray<Map>((uint)Maps.Length + 1, (uint)Maps[0].Length, null);
                        for (uint i = 0; i < Maps.Length; i++) {
                            for (uint j = 0; j < Maps[0].Length; j++) {
                                newMaps[i][j] = Maps[i][j];
                            }
                        }
                        for (uint y = 0; y < newMaps[0].Length; y++) {
                            newMaps[Maps.Length][y] = new Map(MAP_SIZE);
                        }
                        Maps = newMaps;
                    }
                    while (SelectedMap.Y >= Maps[0].Length) {
                        Map[ ][ ] newMaps = SimpleUtils.Create2DArray<Map>((uint)Maps.Length, (uint)Maps[0].Length + 1, null);
                        for (uint i = 0; i < Maps.Length; i++) {
                            for (uint j = 0; j < Maps[0].Length; j++) {
                                newMaps[i][j] = Maps[i][j];
                            }
                        }
                        for (uint x = 0; x < newMaps.Length; x++) {
                            newMaps[x][Maps[0].Length] = new Map(MAP_SIZE);
                        }
                        Maps = newMaps;
                    }
                }
                Point pos = SelectedMap * MAP_SIZE + SelectedPoint;
                if (!pos.Equals(PlayerPosition) &&
                    !pos.Equals(PlayerPosition + new Point(0, 1))) {
                    Maps[SelectedMap.X][SelectedMap.Y][(uint)SelectedPoint.X, (uint)SelectedPoint.Y] = CurrentValue;
                }
            }

            if (KeyboardState.IsKeyDown(Keys.S) && !PreviousKeyboardState.IsKeyDown(Keys.S)) {
                Map mapToSave = Map.ConvertToBig(Maps);
                WinUtils.Save(mapToSave, PlayerPosition);
            }

            if (KeyboardState.IsKeyDown(Keys.O) && !PreviousKeyboardState.IsKeyDown(Keys.O)) {
                Tuple<Map, Point> mapToLoad = WinUtils.LoadMap( );
                Maps = Map.ConvertFromBig(mapToLoad.Item1, MAP_SIZE);
                PlayerPosition = mapToLoad.Item2;
                Cam.Position = PlayerPosition.ToVector2( ) - new Vector2(
                    Graphics.PreferredBackBufferWidth / Cam.Scale.X / 2,
                    Graphics.PreferredBackBufferHeight / Cam.Scale.Y / 2);
            }

            PreviousKeyboardState = KeyboardState;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);
            SpriteBatch.Begin( );
            SpriteBatch.Draw(Back, new Rectangle(Cam.WorldToWindow(new Point(-26, 3).ToVector2()), (new Point(429, 86).ToVector2() * Cam.Scale).ToPoint()), Color.White);

            for (uint i = 0; i < Maps.Length; i++) {
                for (uint j = 0; j < Maps[i].Length; j++) {
                    Maps[i][j].Draw(SpriteBatch, new Camera(Cam.Position - new Vector2(MAP_SIZE.X * i, MAP_SIZE.Y * j), Cam.Scale));
                }
            }
            SpriteBatch.Draw(SimpleUtils.WhiteRect, new Rectangle(Cam.WorldToWindow(SelectedPoint.Vector2( ).Add(0.25f) + 
                new Vector2(MAP_SIZE.X * SelectedMap.X, MAP_SIZE.Y * SelectedMap.Y)), 
                (Cam.Scale / 2f).RoundToPoint( )), new Color(0, 0, 255, 127));
            SpriteBatch.Draw(SimpleUtils.WhiteRect, new Rectangle(Cam.WorldToWindow(PlayerPosition.Vector2( ) + new Vector2(0.25f, 0.5f)), (Cam.Scale + new Vector2(-Cam.Scale.X / 2f, Cam.Scale.Y / 2f)).RoundToPoint( )), Color.Green);
            SpriteBatch.End( );

            base.Draw(gameTime);
        }
    }
}
