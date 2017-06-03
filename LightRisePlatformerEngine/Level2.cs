using LightRise.BaseClasses;
using LightRise.WinUtilsLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Spine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightRise.Main
{
    class Level2 : Level
    {
        Texture2D Back_2;
        Texture2D Back_1;
        Texture2D Back_0;
        Texture2D door0Tex;
        Texture2D door1Tex;
        public Player Player;
        GraphicsDevice GraphicsDevice;
        GraphicsDeviceManager Graphics;
        InfoScreen screen;
        SpriteFont font;
        Texture2D terminal;
        SpineObject CutScene;
        public event Action NextLevel;

        class Door : GameObject
        {
            Texture2D Texture;
            bool opened;

            public Door(Texture2D texture, Level World, Vector2 Position, Vector2 Size) : base(World, Position, Size)
            {
                Texture = texture;
            }

            public override void Draw(SpriteBatch surface, Camera camera)
            {
                if (!opened)
                {
                    surface.Begin();
                    surface.Draw(Texture, new Rectangle(camera.WorldToWindow(Position), (Size * camera.Scale.X).ToPoint()), Color.White);
                    surface.End();
                }
            }

            public void Open()
            {
                opened = true;
            }
        }

        public Level2(ContentManager Content, GraphicsDeviceManager Graphics) : base()
        {
            this.Graphics = Graphics;
            GraphicsDevice = Graphics.GraphicsDevice;
#if DEBUG
            Cam = new Camera(new Vector2(0, 0), new Vector2(16f, 16f));
#else
            Cam = new Camera(new Vector2(0, 0), new Vector2(40f, 40f));
#endif
            Tuple<Map, Point> tuple = WinUtils.LoadMap("Content/L2.lrmap");
            Map = tuple.Item1;
            Player = new Player(this, new Point(2, 63), GraphicsDevice);
            Objects.Add("door0", new Door(door0Tex, this, new Vector2(71, 36), new Vector2(4, 2)));
            Objects.Add("door1", new Door(door1Tex, this, new Vector2(89, 66), new Vector2(2, 10)));
            Objects.Add("door2", new Door(door1Tex, this, new Vector2(114, 43), new Vector2(2, 9)));
            Objects.Add("door3", new Door(door1Tex, this, new Vector2(136, 66), new Vector2(2, 10)));
            Objects.Add("Player", Player);
            Cam.Position = Player.Position - Player.Size / Cam.Scale / 2f - new Vector2(GraphicsDevice.Viewport.Width / Cam.Scale.X / 2, GraphicsDevice.Viewport.Height / Cam.Scale.Y / 2);
            SimpleUtils.Init(GraphicsDevice);
            Renders = new RenderTarget2D[4];
            for (uint i = 0; i < Renders.Length; i++)
            {
                Renders[i] = new RenderTarget2D(GraphicsDevice, Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight);
            }
            font = Content.Load<SpriteFont>("HackFont");
            terminal = Content.Load<Texture2D>("Terminal");
            Back_0 = Content.Load<Texture2D>("L2");
            Back_1 = Content.Load<Texture2D>("BG_1");
            Back_2 = Content.Load<Texture2D>("BG_2");
            door0Tex = Content.Load<Texture2D>("door0");
            door1Tex = Content.Load<Texture2D>("door1");
            FirstHack.Items = Player.Items;
            SecondHack.Items = Player.Items;
            Interactives.Add("Console0", new InteractivePoint(new Vector2(92 - 26, 28 + 3), 6f));
            Map[98 - 26, 34 + 3] = Map.WALL;
            Map[99 - 26, 34 + 3] = Map.WALL;
            Interactives["Console0"].ToInteract += delegate
            {
                Map[98 - 26, 34 + 3] = Map.LADDER;
                Map[99 - 26, 34 + 3] = Map.LADDER;
                screen = new InfoScreen(font, "Класс - это элемент\nпрограммного  обеспечения,\nописывающий абстрактный тип\nданных и его\nчастичную или полную\nреализацию", GraphicsDevice, terminal);
                screen.close = () =>
                {
                    screen = null;
                    Interactives["Console0"].BreakInteraction();
                    (Objects["door0"] as Door).Open();
                };
            };
            Interactives.Add("Console1", new InteractivePoint(new Vector2(100 - 26, 70 + 3), 4f));
            Interactives["Console1"].ToInteract += delegate
            {
                for (uint Y = 63 + 3; Y < 63 + 3 + 10; Y++)
                    Map[115 - 26, Y] = Map.EMPTY;
                screen = new InfoScreen(font, "Инкапсуляция - упаковка\nданных и функций\nв единый компонент", GraphicsDevice, terminal);
                screen.close = () =>
                {
                    screen = null;
                    Interactives["Console1"].BreakInteraction();
                    (Objects["door1"] as Door).Open();
                };
            };
            Interactives.Add("Console2", new InteractivePoint(new Vector2(155 - 26, 46 + 3), 4f));
            Interactives["Console2"].ToInteract += delegate
            {
                for (uint Y = 40 + 3; Y < 40 + 3 + 9; Y++)
                    Map[140 - 26, Y] = Map.EMPTY;
                Map[142 - 26, 40 + 3 + 9] = Map.LEFT_SHELF;
                screen = new InfoScreen(font, "Наследование - концепция\nобъектно-ориентированного\nпрограммирования,\nпозволяющая наследовать\nданные и\nфункциональность некоторого\nсуществующего типа", GraphicsDevice, terminal);
                screen.close = () =>
                {
                    screen = null;
                    Interactives["Console2"].BreakInteraction();
                    (Objects["door2"] as Door).Open();
                };
            };
            Interactives.Add("Console3", new InteractivePoint(new Vector2(148 - 26, 37 + 3), 4f));
            Interactives["Console3"].ToInteract += delegate
            {
                for (uint Y = 63 + 3; Y < 63 + 3 + 10; Y++)
                    Map[162 - 26, Y] = Map.EMPTY;
                screen = new InfoScreen(font, "Полиморфизм - способность\nфункции обрабатывать\nданные разных типов", GraphicsDevice, terminal);
                screen.close = () =>
                {
                    screen = null;
                    Interactives["Console3"].BreakInteraction();
                    (Objects["door3"] as Door).Open();
                };
            };
            Interactives.Add("Console4", new InteractivePoint(new Vector2(260 - 26, 44 + 3), 4f));
            Interactives["Console4"].ToInteract += delegate
            {
                screen = new InfoScreen(font, "Метод - это функция или\nпроцедура, принадлежащая\nкакому-то классу или объекту.", GraphicsDevice, terminal);
                screen.close = () =>
                {
                    screen = null;
                    Interactives["Console4"].BreakInteraction();
                };
            };
            ActiveZones.Add("CutScene",new InteractiveArea(this, new Rectangle(308, 15, 7, 22)));
            ActiveZones["CutScene"].Impact += (KeyValuePair<string, GameObject> a) =>
            {
                if (a.Key == "Player")
                {
                    CutScene = new SpineObject(GraphicsDevice, "Animations/Prologue/CutScene", 0.007f, new Vector2(25, 15));
                    CutScene.State.SetAnimation(0, "аnimation", false);
                    CutScene.State.Complete += delegate (TrackEntry e)
                    {
                        NextLevel();
                    };
                    Cam.Position = Vector2.Zero;
                }
            };
        }

        public override void Update(GameTime gameTime)
        {
            if (CutScene == null)
            {
                if (screen == null)
                {
                    foreach (var obj in Objects)
                        obj.Value.Update(gameTime);
                    foreach (var zone in ActiveZones)
                        zone.Value.Update(gameTime);
                }
                else
                    screen.Update(gameTime, new StepState(gameTime, Keyboard.GetState(), Mouse.GetState()));
            }
            else
            {
                CutScene.offset = CutScene.pos;
                CutScene.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch SpriteBatch)
        {
            if (CutScene != null)
                CutScene.Draw(Cam);
            else
            {
                SpriteBatch.Begin();
                SpriteBatch.Draw(Back_2, new Rectangle(new Point(0 - 20), (new Vector2(1, 17f / 30f) * Graphics.PreferredBackBufferWidth * 1.3f).ToPoint()), Color.White);
                SpriteBatch.Draw(Back_1, new Rectangle(Cam.WorldToWindow(Cam.Position + Vector2.UnitX * Cam.Position.X * -0.2f + Vector2.UnitX * -3f), (new Vector2(1, 10f / 25f) * Graphics.PreferredBackBufferWidth * 2f).ToPoint()), Color.White);
                SpriteBatch.Draw(Back_0, new Rectangle(Cam.WorldToWindow(new Vector2(-26.3f, 2.5f)), (new Point(429, 86).ToVector2() * Cam.Scale).ToPoint()), Color.White);
#if DEBUG
                Map.Draw(SpriteBatch, Cam);
#endif
                SpriteBatch.End();

                foreach (var obj in Objects)
                    obj.Value.Draw(SpriteBatch, Cam);
                foreach (var zone in ActiveZones)
                    zone.Value.Draw(SpriteBatch, Cam);
                if (screen != null)
                    screen.Draw(Cam, SpriteBatch);
            }
        }
    }
}
