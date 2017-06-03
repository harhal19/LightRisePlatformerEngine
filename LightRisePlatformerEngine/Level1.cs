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
    class Level1 : Level
    {
        Texture2D Back_2;
        Texture2D Back_1;
        Texture2D Back_0;
        public Player Player;
        Bridge bridge;
        GraphicsDevice GraphicsDevice;
        GraphicsDeviceManager Graphics;
        public event Action NextLevel;
        public event Action Restart;

        class Bridge : GameObject
        {
            SpineObject spine;
            bool Triggered = false;

            public Bridge(Level world, Vector2 position, Vector2 size, GraphicsDevice graphicDevice) : base(world, position, size)
            {
                spine = new SpineObject(graphicDevice, "Animations/L1/Bridge", 1 / 60f /*1 / 250f*/, Position);
                spine.State.Event += delegate (TrackEntry entry, Event e)
                {
                    e.Int = e.Data.Name[3] - '1';
                    for (int i = e.Int * 6; i < (e.Int + 1) * 6; i ++)
                        world.Map[(uint)(position.X - Size.X / 2 + i + 2), (uint)(position.Y)] = Map.EMPTY;
                };
            }

            public override void Update(GameTime gameTime)
            {
                base.Update(gameTime);
                if (spine != null)
                {
                    spine.pos = Position;
                    spine.Update(gameTime);
                }
                Rectangle r1 = new Rectangle((Position - new Vector2(0, Size.Y)).ToPoint(), Size.ToPoint());
                foreach (var obj in World.Objects)
                {
                    if (r1.Contains(obj.Value.Position))
                        if (obj.Key == "Player" && !Triggered)
                        {
                            spine.State.SetAnimation(0, "anim", false);
                            Triggered = true;
                        }
                }
            }

            public override void Draw(SpriteBatch surface, Camera camera)
            {
                surface.Begin();
                Color color = Color.OrangeRed;
                surface.End();
                spine.Draw(camera);
            }
        }

        public Level1(ContentManager Content, GraphicsDeviceManager Graphics) : base()
        {
            this.Graphics = Graphics;
            GraphicsDevice = Graphics.GraphicsDevice;
#if DEBUG
            Cam = new Camera(new Vector2(0, 0), new Vector2(16f, 16f));
#else
            Cam = new Camera(new Vector2(0, 0), new Vector2(40f, 40f));
#endif
            Tuple<Map, Point> tuple = WinUtils.LoadMap("Content/L1.lrmap");
            Map = tuple.Item1;
            Player = new Player(this, new Point(3, 22), GraphicsDevice);
            Cam.Position = Player.Position - Player.Size / Cam.Scale / 2f - new Vector2(GraphicsDevice.Viewport.Width / Cam.Scale.X / 2, GraphicsDevice.Viewport.Height / Cam.Scale.Y / 2);
            bridge = new Bridge(this, new Vector2(102, 14), new Vector2(31, 10), GraphicsDevice);
            Objects.Add("Player", Player);
            Objects.Add("bridge", bridge);
            SimpleUtils.Init(GraphicsDevice);
            Renders = new RenderTarget2D[4];
            for (uint i = 0; i < Renders.Length; i++)
            {
                Renders[i] = new RenderTarget2D(GraphicsDevice, Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight);
            }
            Back_0 = Content.Load<Texture2D>("L1");
            Back_1 = Content.Load<Texture2D>("BG_1");
            Back_2 = Content.Load<Texture2D>("BG_2");
            ActiveZones.Add("nextLevel", new InteractiveArea(this, new Rectangle(134, 7, 4, 7)));
            ActiveZones["nextLevel"].Impact += (KeyValuePair<string, GameObject> a) => NextLevel();
            ActiveZones.Add("fail", new InteractiveArea(this, new Rectangle(87, 35, 31, 10)));
            ActiveZones["fail"].Impact += (KeyValuePair<string, GameObject> a) => Restart();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var obj in Objects)
                obj.Value.Update(gameTime);
            foreach (var zone in ActiveZones)
                zone.Value.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch SpriteBatch)
        {
            SpriteBatch.Begin();
            SpriteBatch.Draw(Back_2, new Rectangle(new Point(0 - 20), (new Vector2(1, 17f / 30f) * Graphics.PreferredBackBufferWidth * 1.3f).ToPoint()), Color.White);
            SpriteBatch.Draw(Back_1, new Rectangle(Cam.WorldToWindow(Cam.Position + Vector2.UnitX * Cam.Position.X * -0.2f + Vector2.UnitX * -3f), (new Vector2(1, 10f / 25f) * Graphics.PreferredBackBufferWidth * 2f).ToPoint()), Color.White);
            SpriteBatch.Draw(Back_0, new Rectangle(Cam.WorldToWindow(new Vector2(-28.7f, -15.7f)), (new Point(196, 68).ToVector2() * Cam.Scale).ToPoint()), Color.White);
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
