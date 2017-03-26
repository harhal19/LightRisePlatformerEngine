using LightRise.BaseClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework.Input;
using LightRise.WinUtilsLib;

namespace LightRise.Main
{
    class Player : IInstance
    {

        public const float WIDTH = 0.8f;
        public const float HEIGHT = 1.6f;
        public const float GRAVITY = 0.02f;
        public const float SIT_HEIGHT = 0.9f;

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
        public ACTIONS Action = ACTIONS.STAND_LEFT;

        public uint Alarm = 0;
        public float VSpeed = 0f;
        public float VSize = HEIGHT;

        private Point _gridPosition;
        public Point GridPosition
        {
            get { return _gridPosition; }
            set
            {
                if (value == null)
                {
                    throw new NullReferenceException();
                }
                _gridPosition = value;
                Position = GridPosition.ToVector2();
            }
        }

        public Vector2 Position;

        public SpineObject Hero;

        public Player(Point position)
        {
            if (Collision(position))
            {
                throw new Exception("Incorrect initial coordinates of the player");
            }
            GridPosition = position;
        }

        public void SetHero(GraphicsDevice graphicDevice, float baseScale) {
            Hero = new SpineObject(graphicDevice, "Sample", baseScale, Position);
            Hero.State.SetAnimation(0, "стоять ", true);
        }

        public static Map Map { get { return Program.MainThread.Map; } }

        public static bool Collision(Point position)
        {
            return
                Map[position] == Map.WALL;
        }

        public void Draw(SpriteBatch surface, Camera camera) {
            if (Hero == null) {
                surface.Draw(SimpleUtils.WhiteRect, new Rectangle(camera.WorldToWindow(Position.Add(0.5f) - new Vector2(WIDTH / 2f, VSize - 1.5f)), (new Vector2(WIDTH, VSize) * camera.Scale).ToPoint( )), Color.Green);
            } else {
                Hero.offset = new Vector2(camera.Scale.X / 2, camera.Scale.Y);
                Hero.Draw(camera);
            }
        }

        public void OnAlarm(StepState state) {
            switch (Action) {
            case ACTIONS.WALK_LEFT:
                Action = ACTIONS.STAND_LEFT;
                Hero.State.SetAnimation(0, "стоять ", true);
                GridPosition += new Point(-1, 0);
                break;
            case ACTIONS.WALK_RIGHT:
                Action = ACTIONS.STAND_LEFT;
                Hero.State.SetAnimation(0, "стоять ", true);
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

        public void Step(StepState state)
        {
            if (Alarm > 0)
            {
                Alarm--;
                if (Alarm == 0)
                {
                    OnAlarm(state);
                }
            }
            if (Action == ACTIONS.WALK_LEFT)
            {
                Position += new Vector2(-1f / WALK_TIME, 0);
            }
            if (Action == ACTIONS.WALK_RIGHT)
            {
                Position += new Vector2(1f / WALK_TIME, 0);
            }
            if (Action == ACTIONS.STAND_LEFT) {
                if (Map[GridPosition + new Point(0, 2)] == Map.EMPTY) {
                    Action = ACTIONS.FALL;
                }
                else if (state.Keyboard.IsKeyDown(Keys.A) && !Collision(GridPosition + new Point(-1, 0)))
                {
                    Action = ACTIONS.WALK_LEFT;
                    Alarm = WALK_TIME;
                    Hero.State.SetAnimation(0, "бег", true);
                    Hero.Skeleton.FlipX = false;
                }
                else if (state.Keyboard.IsKeyDown(Keys.D) && !Collision(GridPosition + new Point(1, 0)))
                {
                    Action = ACTIONS.WALK_RIGHT;
                    Alarm = WALK_TIME;
                    Hero.State.SetAnimation(0, "бег", true);
                    Hero.Skeleton.FlipX = true;
                }
                else if (state.Keyboard.IsKeyDown(Keys.S))
                {
                    Point temp = GridPosition + new Point(0, 2);
                    if (Map[temp] == Map.LEFT_SHELF ||
                        Map[temp] == Map.RIGHT_SHELF)
                    {
                        Action = ACTIONS.GET_DOWN;
                        Alarm = GET_DOWN_TIME;
                    }
                    else
                    {
                        Action = ACTIONS.TO_SIT;
                        Alarm = TO_SIT_TIME;
                    }
                }
                else if (state.Keyboard.IsKeyDown(Keys.W))
                {
                    Point temp = GridPosition + new Point(0, -2);
                    if (Map[temp] == Map.LEFT_SHELF || Map[temp] == Map.RIGHT_SHELF)
                    {
                        Action = ACTIONS.JUMP_ON;
                        uint h = 2u;
                        float t = (float)Math.Sqrt(2f * h / GRAVITY);
                        VSpeed = -GRAVITY * t;
                        Alarm = (uint)Math.Floor(t);
                    }
                }
            }
            if (Action == ACTIONS.JUMP_ON)
            {
                Position += new Vector2(0, VSpeed);
                VSpeed += GRAVITY;
            }
            if (Action == ACTIONS.GET_DOWN)
            {
                Position += new Vector2(0, 2f / GET_DOWN_TIME);
            }
            if (Action == ACTIONS.TO_SIT)
            {
                VSize -= (HEIGHT - SIT_HEIGHT) / TO_SIT_TIME;
            }
            if (Action == ACTIONS.SIT)
            {
                if (Map[GridPosition + new Point(0, 2)] == Map.EMPTY)
                {
                    Action = ACTIONS.FALL;
                }
                else if (state.Keyboard.IsKeyDown(Keys.W) && !Collision(GridPosition))
                {
                    Action = ACTIONS.FROM_SIT;
                    Alarm = FROM_SIT_TIME;
                }
                else if (state.Keyboard.IsKeyDown(Keys.A) && !Collision(GridPosition + new Point(-1, 1)))
                {
                    Action = ACTIONS.SQUAT_LEFT;
                    Alarm = SQUAT_TIME;
                }
                else if (state.Keyboard.IsKeyDown(Keys.D) && !Collision(GridPosition + new Point(1, 1)))
                {
                    Action = ACTIONS.SQUAT_RIGHT;
                    Alarm = SQUAT_TIME;
                }
            }
            if (Action == ACTIONS.HANG_DOWN)
            {
                if (state.Keyboard.IsKeyDown(Keys.S))
                {
                    Action = ACTIONS.FALL;
                }
                else if (state.Keyboard.IsKeyDown(Keys.W))
                {
                    Action = ACTIONS.GET_UP;
                    Alarm = GET_UP_TIME;
                }
            }
            if (Action == ACTIONS.GET_UP)
            {
                Position += new Vector2(0, -2f / GET_UP_TIME);
            }
            if (Action == ACTIONS.FALL)
            {
                VSpeed = Math.Min(2, VSpeed + GRAVITY);
                Position += new Vector2(0f, VSpeed);
                if (Position.Y - GridPosition.Y > 1f)
                {
                    Vector2 pos = Position;
                    GridPosition += new Point(0, 1);
                    if (Map[GridPosition + new Point(0, 2)] != Map.EMPTY)
                    {
                        VSize = HEIGHT;
                        Action = ACTIONS.STAND_LEFT;
                        VSpeed = 0f;
                    }
                    else
                    {
                        Position = pos;
                    }
                }
            }
            if (Action == ACTIONS.SQUAT_LEFT)
            {
                Position += new Vector2(-1f / SQUAT_TIME, 0);
            }
            if (Action == ACTIONS.SQUAT_RIGHT)
            {
                Position += new Vector2(1f / SQUAT_TIME, 0);
            }
            if (Action == ACTIONS.FROM_SIT)
            {
                VSize += (HEIGHT - SIT_HEIGHT) / FROM_SIT_TIME;
            }
            if (Hero != null)
            { Hero.pos = Position; }
        }
    }
}
