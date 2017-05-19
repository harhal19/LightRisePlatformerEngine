using LightRise.BaseClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework.Input;
using LightRise.WinUtilsLib;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace LightRise.Main
{
    public class Player : Character
    {
        public bool Locked; // ToDelete
        public static SoundEffect Pick; // ToChange

        StepState lastState;

        public List<TextObject> Items;
        public SpineObject Hero;
        const bool LEFT = false;
        const bool RIGHT = true;

        const string RUN_ANIMATION = "run";
        const string CROUCH_ANIMATION = "side";
        const string STAND_ANIMATION = "stand";

        public Player(Map world, Point position, GraphicsDevice graphicDevice) : base(world, position.ToVector2(), new Vector2(1, 2), 200f, 7f, 7f, 10f, 40f)
        {
            Items = new List<TextObject>();
            Hero = new SpineObject(graphicDevice, "Hero", 1 / 250f, Position);
            Hero.Skeleton.FindSlot("girl_sword").Attachment = null;
            Hero.State.SetAnimation(0, "stand", true);
        }

        protected override void GoUpdater(float ms)
        {
            float lastLinearSpeed = LinearSpeed;
            base.GoUpdater(ms);
            if (FallingSpeed == 0)
            {
                if (lastLinearSpeed == 0 && LinearSpeed != 0)
                    if (!crouching)
                        Hero.State.SetAnimation(0, RUN_ANIMATION, true);
            }
            else
            {
                if (lastLinearSpeed == 0 && LinearSpeed != 0)
                    if (!crouching)
                        Hero.State.SetAnimation(0, RUN_ANIMATION, true);
            }
            if (LinearSpeed > 0)
                Hero.Skeleton.FlipX = RIGHT;
            if (LinearSpeed < 0)
                Hero.Skeleton.FlipX = LEFT;
        }

        protected override void LeaveTheWallUpdater(float ms)
        {
            base.LeaveTheWallUpdater(ms);
            if (FallingSpeed != 0)
            {
                Hero.State.SetAnimation(0, RUN_ANIMATION, true);
            }
            else if (crouching)
            {
            }
            else
            {
                Hero.State.SetAnimation(0, RUN_ANIMATION, true);
            }
        }

        protected override void TouchTheWallUpdater(float ms)
        {
            base.TouchTheWallUpdater(ms);
            if (FallingSpeed != 0)
            {
                Hero.State.SetAnimation(0, STAND_ANIMATION, true);
            }
            else if (crouching)
            {
            }
            else
            {
                Hero.State.SetAnimation(0, STAND_ANIMATION, true);
            }
        }

        protected override void StopUpdater(float ms)
        {
            base.StopUpdater(ms);
            if (FallingSpeed != 0)
            {
                Hero.State.SetAnimation(0, STAND_ANIMATION, true);
            }
            else if (crouching)
            {
            }
            else
            {
                Hero.State.SetAnimation(0, STAND_ANIMATION, true);
            }
        }

        protected override void CrouchUpdater(float ms)
        {
            base.CrouchUpdater(ms);
            if (crouching)
            {
                if (Hero != null)
                    Hero.State.SetAnimation(0, CROUCH_ANIMATION, false);
            }
            else
            {
                if (Hero != null)
                    if (LinearSpeed == 0)
                        Hero.State.SetAnimation(0, STAND_ANIMATION, false);
                    else
                        Hero.State.SetAnimation(0, RUN_ANIMATION, false);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Hero != null)
            {
                Hero.pos = Position;
                Hero.Update(gameTime);
            }
        }

        public override void Draw(SpriteBatch surface, Camera camera)
        {
            camera.Position = Position - Size / camera.Scale / 2f - new Vector2(15, 8);
            if (Hero == null)
            {
                surface.Begin();
                Color color = Color.OrangeRed;
                surface.Draw(SimpleUtils.WhiteRect, new Rectangle(camera.WorldToWindow(Position.Add(0.5f) - new Vector2(Size.X / 2f, Size.Y - 0.5f)), (Size * camera.Scale.X).ToPoint()), color);
                surface.End();
            }
            else
            {
                Hero.offset = new Vector2(camera.Scale.X * 0.5f, camera.Scale.Y * -0.2f);
                Hero.Draw(camera);
            }
        }

        public void Step(StepState state)
        {
            if (lastState == null)
                lastState = state;
            if (!Locked)
            {
                if (state.Keyboard.IsKeyDown(Keys.Left) || state.Keyboard.IsKeyDown(Keys.A))
                    Left();
                if (state.Keyboard.IsKeyDown(Keys.Right) || state.Keyboard.IsKeyDown(Keys.D))
                    Right();
                if (state.Keyboard.IsKeyDown(Keys.Space))
                    Jump();
                if (state.Keyboard.IsKeyDown(Keys.LeftControl) && !lastState.Keyboard.IsKeyDown(Keys.LeftControl))
                    Crouch();
                if (state.Keyboard.IsKeyDown(Keys.Up) || state.Keyboard.IsKeyDown(Keys.W))
                    Up();
                if (state.Keyboard.IsKeyDown(Keys.Down) || state.Keyboard.IsKeyDown(Keys.S))
                    Down();
                {
                    if (state.Keyboard.IsKeyDown(Keys.E))
                        foreach (var a in Program.MainThread.Instances)
                        {
                            Vector2 diff = a.GridPosition.ToVector2() - Position;
                            if (diff.Length() <= 1.5f)
                            {
                                (a as Comp).Connect();
                                Locked = true;
                                if (Pick != null)
                                    Pick.Play();
                                break;
                            }
                        }
                }
            }
            lastState = state;
        }
    }
}