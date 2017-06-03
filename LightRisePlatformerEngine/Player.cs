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
        public static SoundEffect Pick; // ToChange

        StepState lastState;

        public List<TextObject> Items;
        SpineObject Hero;
        SpineObject HeroBackView;
        SpineObject currentHero;
        const bool LEFT = true;
        const bool RIGHT = false;

        const string RUN_ANIMATION = "run";
        const string STAND_ANIMATION = "pasiv";
        const string STAND_CROUCH_ANIMATION = "pasiv2";
        const string STOP_ANIMATION = "run_end";
        const string CLING_ANIMATION = "jump_on_the_wall";
        const string RISE_ANIMATION = "climbs";
        const string STAND_AT_WALL_ANIMATION = "wall_pasiv";
        const string LAND_ANIMATION = "jump_end";
        const string STOP_CLING_ANIMATION = "fouk";
        const string CROUCH_DOWN_ANIMATION = "squat";
        const string START_ANIMATION = "run_begin";
        const string LOOSE_ANIMATION = "game_over_v2";
        const string STAND_UP_ANIMATION = "squat_duplication1";
        const string FALL_ANIMATION = "flight";
        const string JUMP_ANIMATION = "jump_begin";
        const string TOUCH_WALL_ANIMATION = "wall_touch";
        const string CROUCH_GO_ANIMATION = "crawl";
        const string HANG_ANIMATION = "hanging";
        const string LADDER_LEAVE_UP_ANIMATION = "up";
        const string LADDER_ANIMATION = "climb_the_stairs";
        const string LADDER_LEAVE_DOWN_ANIMATION = "spusk";
        const string LADDER_ENTER_DOWN_ANIMATION = "down";
        const string USE_ANIMATION = "hack";

        public Player(Level world, Point position, GraphicsDevice graphicDevice) : base(world, position.ToVector2(), new Vector2(2, 5), 70f, 15f, 7f, 20f, 70f)
        {
            Items = new List<TextObject>();
            Hero = new SpineObject(graphicDevice, "Animations/Hero/Hero", 1/200f /*1 / 250f*/, Position);
            HeroBackView = new SpineObject(graphicDevice, "Animations/Hero/HeroBackView", 1 / 200f , Position);
            //currentHero.Skeleton.FindSlot("girl_sword").Attachment = null;
            Hero.Skeleton.FindSlot("planca1").Attachment = null;
            Hero.Skeleton.FindSlot("planca2").Attachment = null;
            Hero.Skeleton.FindSlot("planca3").Attachment = null;
            HeroBackView.Skeleton.FindSlot("123").Attachment = null;
            Hero.State.SetAnimation(0, STAND_ANIMATION, true);
            currentHero = Hero;
        }

        float getAniamtionLength(SpineObject spine, string animationName)
        {
            return spine.State.Data.SkeletonData.Animations.Find(
                               delegate (Spine.Animation animation)
                               {
                                   return animation.Name == animationName;
                               }).Duration;
        }

        protected override void GoUpdater(float ms)
        {
            float lastLinearSpeed = LinearSpeed;
            base.GoUpdater(ms);
            if (FallingSpeed == 0)
            {
                if ((lastLinearSpeed == 0 && LinearSpeed != 0) ||
                    (lastLinearSpeed > 0 && LinearSpeed < 0) ||
                    (lastLinearSpeed < 0 && LinearSpeed > 0))
                    if (!crouching)
                    {
                        currentHero.State.ClearTracks();
                        currentHero.State.SetAnimation(0, START_ANIMATION, false);
                        currentHero.State.AddAnimation(0, RUN_ANIMATION, true, getAniamtionLength(currentHero, START_ANIMATION));
                    }
                else
                    {
                        currentHero.State.ClearTracks();
                        currentHero.State.SetAnimation(0, CROUCH_GO_ANIMATION, true);
                    }
            }
            else
            {
                /*if (lastLinearSpeed == 0 && LinearSpeed != 0)
                    if (!crouching)
                        currentHero.State.SetAnimation(0, RUN_ANIMATION, true);*/
            }
            if (LinearSpeed > 0)
                currentHero.Skeleton.FlipX = RIGHT;
            if (LinearSpeed < 0)
                currentHero.Skeleton.FlipX = LEFT;
        }

        protected override void LeaveTheWallUpdater(float ms)
        {
            base.LeaveTheWallUpdater(ms);
            if (FallingSpeed != 0)
            {
                currentHero.State.ClearTracks();
                currentHero.State.SetAnimation(0, RUN_ANIMATION, true);
            }
            else if (crouching)
            {
            }
            else
            {
                currentHero.State.ClearTracks();
                currentHero.State.SetAnimation(0, RUN_ANIMATION, true);
            }
        }

        protected override void TouchTheWallUpdater(float ms)
        {
            base.TouchTheWallUpdater(ms);
            if (FallingSpeed != 0)
            {
            }
            else if (crouching)
            {
            }
            else
            {
                currentHero.State.ClearTracks();
                currentHero.State.SetAnimation(0, TOUCH_WALL_ANIMATION, false);
                currentHero.State.AddAnimation(1, STAND_AT_WALL_ANIMATION, true, getAniamtionLength(currentHero, TOUCH_WALL_ANIMATION));
            }
        }

        protected override void StopUpdater(float ms)
        {
            base.StopUpdater(ms);
            /*if (FallingSpeed != 0)
            {
                currentHero.State.SetAnimation(0, STAND_ANIMATION, true);
            }
            else*/
            if (crouching)
            {
                currentHero.State.ClearTracks();
                currentHero.State.SetAnimation(0, STAND_CROUCH_ANIMATION, true);
            }
            else
            {
                currentHero.State.ClearTracks();
                currentHero.State.SetAnimation(0, STOP_ANIMATION, false);
                currentHero.State.AddAnimation(1, STAND_ANIMATION, true, getAniamtionLength(currentHero, STOP_ANIMATION));
            }
        }

        protected override void CrouchUpdater(float ms)
        {
            if (delay == 0)
                if (!crouching)
                {
                    if (currentHero != null)
                    {
                        currentHero.State.ClearTracks();
                        currentHero.State.SetAnimation(0, CROUCH_DOWN_ANIMATION, false);
                        delay = getAniamtionLength(currentHero, CROUCH_DOWN_ANIMATION);
                    }
                }
                else
                {
                    if (currentHero != null)
                    {
                        currentHero.State.ClearTracks();
                        currentHero.State.SetAnimation(0, STAND_UP_ANIMATION, false);
                        delay = getAniamtionLength(currentHero, STAND_UP_ANIMATION);
                    }
                }
            base.CrouchUpdater(ms);
        }

        protected override void ClingUpdater(float ms)
        {
            if (delay == 0)
            {
                if (currentHero != null)
                {
                    currentHero.State.ClearTracks();
                    currentHero.State.SetAnimation(0, CLING_ANIMATION, false);
                    delay = getAniamtionLength(currentHero, CLING_ANIMATION);
                    currentHero.State.AddAnimation(1, HANG_ANIMATION, true, delay);
                }
            }
            base.ClingUpdater(ms);
        }

        protected override void RiseUpdater(float ms)
        {
            if (delay == 0)
            {
                if (currentHero != null)
                {
                    currentHero.State.ClearTracks();
                    currentHero.State.SetAnimation(0, RISE_ANIMATION, false);
                    delay = getAniamtionLength(currentHero, RISE_ANIMATION);
                    currentHero.State.AddAnimation(1, STAND_ANIMATION, true, delay);
                }
            }
            base.RiseUpdater(ms);
        }

        protected override void JumpUpdater(float ms)
        {
            if (currentHero != null)
            {
                currentHero.State.ClearTracks();
                currentHero.State.SetAnimation(0, JUMP_ANIMATION, false);
                currentHero.State.AddAnimation(1, FALL_ANIMATION, true, getAniamtionLength(currentHero, JUMP_ANIMATION));
            }
            base.JumpUpdater(ms);
        }

        protected override void ComeDownUpdater(float ms)
        {
            if (currentHero != null)
            {
                if (currentHero == HeroBackView)
                    currentHero = Hero;
                currentHero.State.ClearTracks();
                currentHero.State.SetAnimation(0, FALL_ANIMATION, true);
            }
            base.ComeDownUpdater(ms);
        }

        protected override void LandUpdater(float ms)
        {
            if (delay == 0)
            {
                if (currentHero != null)
                {
                    currentHero.State.ClearTracks();
                    currentHero.State.SetAnimation(0, LAND_ANIMATION, false);
                    delay = getAniamtionLength(currentHero, LAND_ANIMATION);
                    if (toLeft != toRight)
                        currentHero.State.AddAnimation(1, RUN_ANIMATION, true, delay);
                    else
                        currentHero.State.AddAnimation(1, STAND_ANIMATION, true, delay);
                }
            }
            base.LandUpdater(ms);
        }

        protected override void LadderEnterDownUpdater(float ms)
        {
            if (delay == 0)
            {
                currentHero = HeroBackView;
                if (currentHero != null)
                {
                    currentHero.State.ClearTracks();
                    currentHero.State.SetAnimation(0, LADDER_ENTER_DOWN_ANIMATION, false);
                    delay = getAniamtionLength(currentHero, LADDER_ENTER_DOWN_ANIMATION);
                    //currentHero.State.AddAnimation(1, LADDER_ANIMATION, true, delay);
                }
            }
            base.LadderEnterDownUpdater(ms);
        }

        protected override void LadderEnterUpUpdater(float ms)
        {
            base.LadderEnterUpUpdater(ms);
            currentHero = HeroBackView;
        }

        protected override void LadderCatchUpdater(float ms)
        {
            base.LadderCatchUpdater(ms);
            currentHero = HeroBackView;
        }

        protected override void LadderLeaveUpUpdater(float ms)
        {
            if (delay == 0)
            {
                if (currentHero != null)
                {
                    currentHero.State.ClearTracks();
                    currentHero.State.SetAnimation(0, LADDER_LEAVE_UP_ANIMATION, false);
                    delay = getAniamtionLength(currentHero, LADDER_LEAVE_UP_ANIMATION);
                }
            }
            base.LadderLeaveUpUpdater(ms);
            if (delayCounter == 0)
            {
                currentHero = Hero;
                currentHero.State.SetAnimation(0, STAND_ANIMATION, true);
            }
        }

        protected override void LadderLeaveDownUpdater(float ms)
        {
            if (delay == 0)
            {
                if (currentHero != null)
                {
                    currentHero.State.ClearTracks();
                    currentHero.State.SetAnimation(0, LADDER_LEAVE_DOWN_ANIMATION, false);
                    delay = getAniamtionLength(currentHero, LADDER_LEAVE_DOWN_ANIMATION);
                }
            }
            base.LadderLeaveDownUpdater(ms);
            if (delayCounter == 0)
            {
                currentHero = Hero;
                currentHero.State.SetAnimation(0, STAND_ANIMATION, true);
            }
        }

        protected override void LadderGoUpdater(float ms)
        {
            float lastSpeed = FallingSpeed;
            base.LadderGoUpdater(ms);
            if (FallingSpeed != 0)
            {
                if (lastSpeed == 0 ||
                    lastSpeed > 0 && FallingSpeed < 0 ||
                    lastSpeed < 0 && FallingSpeed > 0)
                {
                    currentHero.State.ClearTracks();
                    currentHero.State.SetAnimation(0, LADDER_ANIMATION, true);
                }
            }
            else
            {
                if (lastSpeed == 0)
                {
                    currentHero.State.ClearTracks();
                    currentHero.State.SetAnimation(0, LADDER_ANIMATION, true);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (currentHero != null)
            {
                currentHero.pos = Position;
                currentHero.Update(gameTime);
            }
        }

        public override void Draw(SpriteBatch surface, Camera camera)
        {
            Vector2 cameraOffset = (camera.Position - (Position - Size / camera.Scale / 2f - new Vector2(surface.GraphicsDevice.Viewport.Width / camera.Scale.X / 2, surface.GraphicsDevice.Viewport.Height / camera.Scale.Y / 2)));
           
            if (cameraOffset.Length() > 0.2f)
            {
                cameraOffset.Normalize();
                cameraOffset *= 0.2f;
            }
            camera.Position -= cameraOffset;
            if (currentHero == null)
            {
                surface.Begin();
                Color color = Color.OrangeRed;
                surface.Draw(SimpleUtils.WhiteRect, new Rectangle(camera.WorldToWindow(Position.Add(0.5f) - new Vector2(Size.X / 2f, Size.Y - 0.5f)), (Size * camera.Scale.X).ToPoint()), color);
                surface.End();
            }
            else
            {
#if DEBUG
                surface.Begin();
                Color color = Color.OrangeRed;
                surface.Draw(SimpleUtils.WhiteRect, new Rectangle(camera.WorldToWindow(Position.Add(0.5f) - new Vector2(Size.X / 2f, Size.Y - 0.5f)), (Size * camera.Scale.X).ToPoint()), color);
                surface.End();
#endif
                currentHero.offset = new Vector2(camera.Scale.X * 0.5f, camera.Scale.Y * -1.5f);
                currentHero.Draw(camera);
            }
        }

        public void Step(StepState state)
        {
            if (lastState == null)
                lastState = state;
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
            if (state.Keyboard.IsKeyDown(Keys.E))
                Use();
            lastState = state;
        }
    }
}