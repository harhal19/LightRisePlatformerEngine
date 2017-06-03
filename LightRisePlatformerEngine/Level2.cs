using LightRise.BaseClasses;
using LightRise.WinUtilsLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        public Player Player;
        GraphicsDevice GraphicsDevice;
        GraphicsDeviceManager Graphics;
        InfoScreen screen;
        SpriteFont font;
        Texture2D terminal;
        SpineObject CutScene;
        public event Action NextLevel;

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
            Objects.Add("Player", Player);
            Cam.Position = Player.Position - Player.Size / Cam.Scale / 2f - new Vector2(GraphicsDevice.Viewport.Width / Cam.Scale.X / 2, GraphicsDevice.Viewport.Height / Cam.Scale.Y / 2);
            /*Instances.Add(new FirstComp(Player.Position.ToPoint() + new Point(25, -1), GraphicsDevice));
            Instances.Add(new SecondComp(Player.Position.ToPoint() + new Point(1, -1), GraphicsDevice));*/
            SimpleUtils.Init(GraphicsDevice);
            // TODO: Renders will be used for more fust drawing of the background... Later
            Renders = new RenderTarget2D[4];
            for (uint i = 0; i < Renders.Length; i++)
            {
                Renders[i] = new RenderTarget2D(GraphicsDevice, Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight);
            }
            //SpineInstance = new SpineObject(GraphicsDevice, "Sample", 1, new Vector2(20, 10));
            //HackFont = Content.Load<SpriteFont>("HackFont");
            font = Content.Load<SpriteFont>("HackFont");
            //Terminal = Content.Load<Texture2D>("Terminal");
            terminal = Content.Load<Texture2D>("Terminal");
            Back_0 = Content.Load<Texture2D>("L2");
            Back_1 = Content.Load<Texture2D>("BG_1");
            Back_2 = Content.Load<Texture2D>("BG_2");
            //FirstHackScreen = new FirstHack(HackFont, SpriteBatch, Terminal, Instances[0] as Comp);
            FirstHack.Items = Player.Items;
            //SecondHackScreen = new SecondHack(HackFont, SpriteBatch, Terminal, Instances[1] as Comp);
            SecondHack.Items = Player.Items;
            Texture2D doorText = Content.Load<Texture2D>("door");
            Texture2D computerTex = Content.Load<Texture2D>("Computer");
            //(Instances[0] as Comp).texture = computerTex;
            //(Instances[1] as Comp).texture = computerTex;
            //Door1 = new Door(doorText, Player.Position.ToPoint() + new Point(22, -2), Instances[0] as Comp, Map);
            //Door2 = new Door(doorText, Player.Position.ToPoint() + new Point(-1, -2), Instances[1] as Comp, Map);
            //Player.Pick = Content.Load<SoundEffect>("Pick");
            //(Instances[0] as Comp).Allowed = true;
            //script1 = delegate ()
            //{
            //    (Instances[0] as Comp).Allowed = false;
            //};
            //script2 = delegate ()
            //{
            //    BigDoor = Content.Load<Texture2D>("BigDoor");
            //};
            //script3 = delegate ()
            //{
            //    Finish = true;
            //    finishColor.A = 0;
            //};
            //Player.GridPosition += new Point(4, 0);
            //MediaPlayer.Play(Content.Load<Song>("Music"));
            //MediaPlayer.Volume = 0.6f;
            //MediaPlayer.IsRepeating = true;
            // TODO: use this.Content to load your game content here*/
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
                    Cam.Position = Vector2.Zero;
                }
            };
        }

        public override void Update(GameTime gameTime)
        {

            //float cam_spd = 0.1f;
            /*float dx = (State.Keyboard.IsKeyDown(Keys.Right) ? cam_spd : 0) - (State.Keyboard.IsKeyDown(Keys.Left) ? cam_spd : 0);
            float dy = (State.Keyboard.IsKeyDown(Keys.Down) ? cam_spd : 0) - (State.Keyboard.IsKeyDown(Keys.Up) ? cam_spd : 0);*/
            //Cam.Position = new Vector2(Cam.Position.X + dx, Cam.Position.Y + dy);
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
            /*Player.Hero.Update(gameTime);

            Player.Step(State);*/
            /*if (Player.Position.X > 40 && script1 != null)
            {
                script1();
                script1 = null;
            }
            if (Player.Position.X < 12 && Player.Position.Y < 11 && script3 != null)
            {
                script3();
                script3 = null;
            }*/
            //Cam.Position = Player.Position - Size.ToVector2() / Cam.Scale / 2f;
            /*if (HackScreen != null)
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
            catch { }*/
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
                //SpriteBatch.Draw(Back_0, new Rectangle(Cam.WorldToWindow(new Vector2(-28.7f, -15.2f)), (new Point(196, 68).ToVector2() * Cam.Scale).ToPoint()), Color.White);
                SpriteBatch.Draw(Back_0, new Rectangle(Cam.WorldToWindow(new Vector2(-26.3f, 2.5f)), (new Point(429, 86).ToVector2() * Cam.Scale).ToPoint()), Color.White);
#if DEBUG
                Map.Draw(SpriteBatch, Cam);
#endif
                SpriteBatch.End();
                /*Door1.Draw(SpriteBatch, Cam);
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
                catch (InvalidOperationException) { }*/

                foreach (var obj in Objects)
                    obj.Value.Draw(SpriteBatch, Cam);
                foreach (var zone in ActiveZones)
                    zone.Value.Draw(SpriteBatch, Cam);
                if (screen != null)
                    screen.Draw(Cam, SpriteBatch);
            }

            /*SpriteBatch.Begin();
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
            }*/
        }
    }
}
