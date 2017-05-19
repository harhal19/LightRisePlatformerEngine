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

        //public override void Left()
        //{
        //    if (CharacterHorisontalState != HorisontalStates.GO_TO_THE_LEFT)
        //    {
        //        if (CharacterVerticalState == VerticalStates.STANDING)
        //            if (Hero != null)
        //                Hero.State.SetAnimation(0, RUN_ANIMATION, true);
        //        /*else
        //            Hero.State.SetAnimation(0, "side", true);*/
        //    }
        //    if (Hero != null)
        //        Hero.Skeleton.FlipX = LEFT;
        //    base.Left();
        //}

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

        //public override void Right()
        //{
        //    if (CharacterHorisontalState != HorisontalStates.GO_TO_THE_RIGHT)
        //    {
        //        if (CharacterVerticalState == VerticalStates.STANDING)
        //            if (Hero != null)
        //                Hero.State.SetAnimation(0, RUN_ANIMATION, true);
        //        /*else
        //            Hero.State.SetAnimation(0, "side", true);*/
        //    }
        //    if (Hero != null)
        //        Hero.Skeleton.FlipX = RIGHT;
        //    base.Right();
        //}

        //public override void Stop()
        //{
        //    //if (LinearSpeed != 0)
        //    if (CharacterHorisontalState != HorisontalStates.NOT_MOVING)
        //    {
        //        if (CharacterVerticalState == VerticalStates.STANDING)
        //            if (Hero != null)
        //                Hero.State.SetAnimation(0, STAND_ANIMATION, true);
        //        /*else
        //            Hero.State.SetAnimation(0, "side", true);*/
        //    }
        //    base.Stop();
        //}

        //public override void Fall()
        //{
        //    if (CharacterVerticalState != VerticalStates.FALLING)
        //    {
        //        if (Hero != null)
        //            Hero.State.SetAnimation(0, STAND_ANIMATION, true);
        //    }
        //    /*else
        //        Hero.State.SetAnimation(0, "side", true);*/
        //    base.Fall();
        //}

        //public override void Jump()
        //{
        //    base.Jump();
        //}

        //public override void TouchDown()
        //{
        //    if (CharacterHorisontalState != HorisontalStates.NOT_MOVING)
        //        if (Hero != null)
        //            Hero.State.SetAnimation(0, RUN_ANIMATION, true);
        //    base.TouchDown();
        //}

        //public override void Crouch()
        //{
        //    VerticalStates lastCharacterVerticalState = CharacterVerticalState;
        //    base.Crouch();
        //    if (CharacterVerticalState == VerticalStates.CROUCHING && lastCharacterVerticalState == VerticalStates.STANDING)
        //    {
        //        if (Hero != null)
        //            Hero.State.SetAnimation(0, CROUCH_ANIMATION, false);
        //    }
        //}

        //public override void StandUp()
        //{
        //    VerticalStates lastCharacterVerticalState = CharacterVerticalState;
        //    base.StandUp();
        //    if (CharacterVerticalState == VerticalStates.STANDING_UP && lastCharacterVerticalState == VerticalStates.STEALING)
        //    {
        //        if (CharacterHorisontalState == HorisontalStates.NOT_MOVING)
        //        {
        //            if (Hero != null)
        //                Hero.State.SetAnimation(0, STAND_ANIMATION, true);
        //        }
        //        else
        //        {
        //            if (Hero != null)
        //                Hero.State.SetAnimation(0, RUN_ANIMATION, true);
        //        }
        //    }
        //}

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

        /*public override bool BlockIsFallable(uint Block)
        {
            return Block != Map.WALL && (FallingSpeed < 0 || (Block != Map.LEFT_SHELF && Block != Map.RIGHT_SHELF) || isJumpDown);
        }*/

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
                /*if (!state.Keyboard.IsKeyDown(Keys.Left) && !state.Keyboard.IsKeyDown(Keys.A) && !state.Keyboard.IsKeyDown(Keys.Right) && !state.Keyboard.IsKeyDown(Keys.D))
                    Stop();*/
                if (state.Keyboard.IsKeyDown(Keys.Space))
                    Jump();
                if (state.Keyboard.IsKeyDown(Keys.LeftControl) && !lastState.Keyboard.IsKeyDown(Keys.LeftControl))
                    Crouch();
                /*else
                    StandUp();*/
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
    /*IInstance
{
    public List<TextObject> Items;
    public SoundEffect pick;
    public const bool LEFT = false;
    public const bool RIGHT = true;

    public const float WIDTH = 0.8f;
    public const float HEIGHT = 1.6f;
    public const float GRAVITY = 0.02f;
    public const float SIT_HEIGHT = 0.9f;

    public bool Locked = false;

    public const uint WALK_TIME = 20;
    public const uint TO_SIT_TIME = 10;
    public const uint FROM_SIT_TIME = 15;
    public const uint SQUAT_TIME = 40;
    public const uint GET_DOWN_TIME = 30;
    public const uint GET_UP_TIME = 60;
    public enum ACTIONS {
        STAND_LEFT,
        STAND_RIGHT,
        WALK_LEFT,
        WALK_RIGHT,
        FALL,
        TO_SIT,
        SIT,
        FROM_SIT,
        SQUAT_LEFT,
        SQUAT_RIGHT,
        GET_DOWN,
        HANG_DOWN,
        GET_UP,
        JUMP_ON,
        JUMP_LEFT,
        JUMP_RIGHT,
    }
    public ACTIONS ActionPrevious = ACTIONS.STAND_LEFT;
    public ACTIONS Action = ACTIONS.STAND_LEFT;

    public uint Alarm = 0;
    public float VSpeed = 0f;
    public float VSize = HEIGHT;

    private Point _gridPosition;
    public Point GridPosition {
        get { return _gridPosition; }
        set {
            if (value == null) {
                throw new NullReferenceException( );
            }
            _gridPosition = value;
            Position = GridPosition.ToVector2( );
        }
    }

    public Vector2 Position;

    public SpineObject Hero;

    public Player(Point position) {
        if (Collision(position)) {
            throw new Exception("Incorrect initial coordinates of the player");
        }
        GridPosition = position;
        Items = new List<TextObject>();
    }

    public void SetHero(GraphicsDevice graphicDevice, float baseScale) {
        Hero = new SpineObject(graphicDevice, "Hero", baseScale, Position);
        Hero.State.SetAnimation(0, "stand", true);
    }

    public static Map Map { get { return Program.MainThread.Map; } }

    public static bool Collision(Point position) {
        return
            Map[position] == Map.WALL;
    }

    public void Draw(SpriteBatch surface, Camera camera) {
        if (Hero == null) {
            surface.Draw(SimpleUtils.WhiteRect, new Rectangle(camera.WorldToWindow(Position.Add(0.5f) - new Vector2(WIDTH / 2f, VSize - 1.5f)), (new Vector2(WIDTH, VSize) * camera.Scale).ToPoint( )), Color.Green);
        } else {
            Hero.offset = new Vector2(camera.Scale.X * 0.5f, camera.Scale.Y * 0.8f);
            Hero.Draw(camera);
        }
    }

    /*public void OnAlarm(StepState state) {
        switch (Action) {
        case ACTIONS.WALK_LEFT:
            Action = ACTIONS.STAND_LEFT;
            GridPosition += new Point(-1, 0);
            break;
        case ACTIONS.WALK_RIGHT:
            Action = ACTIONS.STAND_LEFT;
            GridPosition += new Point(1, 0);
            break;
        case ACTIONS.TO_SIT:
            Action = ACTIONS.SIT;
            VSize = SIT_HEIGHT;
            break;
        case ACTIONS.FROM_SIT:
            Action = ACTIONS.STAND_LEFT;
            VSize = HEIGHT;
            break;
        case ACTIONS.SQUAT_LEFT:
            Action = ACTIONS.SIT;
            GridPosition += new Point(-1, 0);
            break;
        case ACTIONS.SQUAT_RIGHT:
            Action = ACTIONS.SIT;
            GridPosition += new Point(1, 0);
            break;
        case ACTIONS.GET_DOWN:
            Action = ACTIONS.HANG_DOWN;
            GridPosition += new Point(0, 2);
            break;
        case ACTIONS.GET_UP:
            Action = ACTIONS.STAND_LEFT;
            GridPosition += new Point(0, -2);
            break;
        case ACTIONS.JUMP_ON:
            Action = ACTIONS.HANG_DOWN;
            GridPosition += new Point(0, -2);
            VSpeed = 0f;
            break;
        }
    }

    public void Step(StepState state) {
        if (!Locked) {
            /*ActionPrevious = Action;
            if (Alarm > 0) {
                Alarm--;
                if (Alarm == 0) {
                    OnAlarm(state);
                }
            }
            if (Action == ACTIONS.WALK_LEFT) {
                Position += new Vector2(-1f / WALK_TIME, 0);
            }
            if (Action == ACTIONS.WALK_RIGHT) {
                Position += new Vector2(1f / WALK_TIME, 0);
            }
            if (Action == ACTIONS.STAND_LEFT) {
                if (Map[GridPosition + new Point(0, 2)] == Map.EMPTY) {
                    Action = ACTIONS.FALL;
                } else if (state.Keyboard.IsKeyDown(Keys.A) && !Collision(GridPosition + new Point(-1, 0))) {
                    Action = ACTIONS.WALK_LEFT;
                    if (ActionPrevious != ACTIONS.WALK_LEFT) {
                        Hero.State.SetAnimation(0, "run", true);
                        Hero.Skeleton.FlipX = LEFT;
                    }
                    Alarm = WALK_TIME;
                } else if (state.Keyboard.IsKeyDown(Keys.D) && !Collision(GridPosition + new Point(1, 0))) {
                    Action = ACTIONS.WALK_RIGHT;
                    if (ActionPrevious != ACTIONS.WALK_RIGHT) {
                        Hero.State.SetAnimation(0, "run", true);
                        Hero.Skeleton.FlipX = RIGHT;
                    }
                    Alarm = WALK_TIME;
                } else {
                    //Hero.State.SetAnimation(0, "стоять ", true);
                    Hero.State.SetAnimation(0, "stand", true);

                    if (state.Keyboard.IsKeyDown(Keys.S)) {
                        Point temp = GridPosition + new Point(0, 2);
                        if (Map[temp] == Map.LEFT_SHELF ||
                            Map[temp] == Map.RIGHT_SHELF) {
                            Action = ACTIONS.GET_DOWN;
                            Alarm = GET_DOWN_TIME;
                            Hero.State.SetAnimation(0, "side", false);
                        } else {
                            Action = ACTIONS.TO_SIT;
                            Alarm = TO_SIT_TIME;
                            Hero.State.SetAnimation(0, "side", false);
                        }
                    } else if (state.Keyboard.IsKeyDown(Keys.W)) {
                        Point temp = GridPosition + new Point(0, -2);
                        if (Map[temp] == Map.LEFT_SHELF || Map[temp] == Map.RIGHT_SHELF) {
                            Action = ACTIONS.JUMP_ON;
                            uint h = 2u;
                            float t = (float)Math.Sqrt(2f * h / GRAVITY);
                            VSpeed = -GRAVITY * t;
                            Alarm = (uint)Math.Floor(t);
                        }
                    } else if (state.Keyboard.IsKeyDown(Keys.E)) {
                        foreach (var a in Program.MainThread.Instances) {
                            Point diff = a.GridPosition - GridPosition;
                            if (diff.Y == 0 && Math.Abs(diff.X) <= 1) {
                                if (a is Comp && Comp.inUse == false) {
                                    (a as Comp).Connect( );
                                    Locked = true;
                                    if (pick != null)
                                        pick.Play();
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            if (Action == ACTIONS.JUMP_ON) {
                Position += new Vector2(0, VSpeed);
                VSpeed += GRAVITY;
            }
            if (Action == ACTIONS.GET_DOWN) {
                Position += new Vector2(0, 2f / GET_DOWN_TIME);
            }
            if (Action == ACTIONS.TO_SIT) {
                VSize -= (HEIGHT - SIT_HEIGHT) / TO_SIT_TIME;
            }
            if (Action == ACTIONS.SIT) {
                if (Map[GridPosition + new Point(0, 2)] == Map.EMPTY) {
                    Action = ACTIONS.FALL;
                } else if (state.Keyboard.IsKeyDown(Keys.W) && !Collision(GridPosition)) {
                    Action = ACTIONS.FROM_SIT;
                    Alarm = FROM_SIT_TIME;
                } else if (state.Keyboard.IsKeyDown(Keys.A) && !Collision(GridPosition + new Point(-1, 1))) {
                    Action = ACTIONS.SQUAT_LEFT;
                    Alarm = SQUAT_TIME;
                } else if (state.Keyboard.IsKeyDown(Keys.D) && !Collision(GridPosition + new Point(1, 1))) {
                    Action = ACTIONS.SQUAT_RIGHT;
                    Alarm = SQUAT_TIME;
                }
            }
            if (Action == ACTIONS.HANG_DOWN) {
                if (state.Keyboard.IsKeyDown(Keys.S)) {
                    Action = ACTIONS.FALL;
                } else if (state.Keyboard.IsKeyDown(Keys.W)) {
                    Action = ACTIONS.GET_UP;
                    Alarm = GET_UP_TIME;
                }
            }
            if (Action == ACTIONS.GET_UP) {
                Position += new Vector2(0, -2f / GET_UP_TIME);
            }
            if (Action == ACTIONS.FALL) {
                VSpeed = Math.Min(2, VSpeed + GRAVITY);
                Position += new Vector2(0f, VSpeed);
                if (Position.Y - GridPosition.Y > 1f) {
                    Vector2 pos = Position;
                    GridPosition += new Point(0, 1);
                    if (Map[GridPosition + new Point(0, 2)] != Map.EMPTY) {
                        VSize = HEIGHT;
                        Action = ACTIONS.STAND_LEFT;
                        VSpeed = 0f;
                    } else {
                        Position = pos;
                    }
                }
            }
            if (Action == ACTIONS.SQUAT_LEFT) {
                Position += new Vector2(-1f / SQUAT_TIME, 0);
                Hero.Skeleton.FlipX = LEFT;
            }
            if (Action == ACTIONS.SQUAT_RIGHT) {
                Position += new Vector2(1f / SQUAT_TIME, 0);
                Hero.Skeleton.FlipX = RIGHT;
            }
            if (Action == ACTIONS.FROM_SIT) {
                VSize += (HEIGHT - SIT_HEIGHT) / FROM_SIT_TIME;
            }
            if (Hero != null) {
                Hero.pos = Position;
            }
}
        }
    }
}*/
