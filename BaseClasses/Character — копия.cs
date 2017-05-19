using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightRise.BaseClasses
{
    public class Character
    {
        Map World;
        Vector2 size;
        public Vector2 Size
        {
            get
            {
                if (CharacterVerticalState == VerticalStates.STEALING)
                    return new Vector2(size.X, size.Y * 0.5f);
                else
                    return size;
            }
            protected set { size = value; }
        }
        public Vector2 Position { get; protected set; }
        public float LinearSpeed { get; protected set; }
        public Vector2 ClimpSpeed { get; protected set; }
        Vector2 ClimpAxeleration;
        public enum HorisontalStates { GO_TO_THE_LEFT, NOT_MOVING, GO_TO_THE_RIGHT }
        public HorisontalStates CharacterHorisontalState { get; protected set; }
        public enum VerticalStates { STANDING, CROUCHING, STEALING, STANDING_UP, START_CLIMBING, CLIMBING, FALLING, HANGING }
        public VerticalStates CharacterVerticalState { get; protected set; }
        float LinearAxeleration;
        float LinearMaxSpeed
        {
            get
            {
                if (CharacterVerticalState == VerticalStates.CROUCHING ||
                    CharacterVerticalState == VerticalStates.STEALING ||
                        CharacterVerticalState == VerticalStates.STANDING_UP)
                    return linearMaxSpeed * 0.5f;
                else
                    return linearMaxSpeed;
            }
            set { linearMaxSpeed = value; }
        }
        float linearMaxSpeed;
        float Gravity;
        public float FallingSpeed { get; protected set; }
        float FallingMaxSpeed;
        float JumpAxeleration;
        public bool UseGravity = true;
        const float COLLIZION_RANGE = 1f;
        protected float CrouchingDelay;
        protected float UsingDelay;
        float Delay;

        public Character(Map world, Vector2 position, Vector2 size, float linearAxeleration, float linearMaxSpeed, float gravity, float fallingMaxSpeed, float jumpAxeleration)
        {
            World = world;
            Position = position;
            Size = size;
            LinearAxeleration = linearAxeleration;
            LinearMaxSpeed = linearMaxSpeed;
            Gravity = gravity;
            FallingMaxSpeed = fallingMaxSpeed;
            JumpAxeleration = jumpAxeleration;
        }

        public virtual void Stop()
        {
            CharacterHorisontalState = HorisontalStates.NOT_MOVING;
        }

        public virtual void Left()
        {
            CharacterHorisontalState = HorisontalStates.GO_TO_THE_LEFT;
        }

        public virtual void Right()
        {
            CharacterHorisontalState = HorisontalStates.GO_TO_THE_RIGHT;
        }

        public virtual void Up()
        {
            if (CharacterVerticalState != VerticalStates.HANGING)
            {
                for (int i = 0; i < Size.Y; i++)
                    if (BlockOn(-i, 0.1f) == Map.LADDER)
                    {
                        CharacterVerticalState = VerticalStates.CLIMBING;
                        ClimpAxeleration.Y = -LinearAxeleration;
                        if (ClimpAxeleration.Y < -LinearMaxSpeed)
                            ClimpAxeleration.Y = -LinearMaxSpeed;
                        return;
                    }
                CharacterVerticalState = VerticalStates.STANDING;
                Position = new Vector2(Position.X, Map.Round(Position.Y));
            }
            /*else
            {
                for (int i = 0; i < Size.Y; i++)
                {
                    uint block = BlockOn(-i, 0.1f);
                    if (block == Map.LEFT_SHELF || block == Map.RIGHT_SHELF)
                    {
                        CharacterVerticalState = VerticalStates.CLIMBING;
                        ClimpAxeleration.Y = -LinearAxeleration;
                        if (ClimpAxeleration.Y < -LinearMaxSpeed)
                            ClimpAxeleration.Y = -LinearMaxSpeed;
                        return;
                    }
                }
                CharacterVerticalState = VerticalStates.STANDING;
                Position = new Vector2(Position.X, Map.Round(Position.Y));
            }*/
        }

        public virtual void Down()
        {
            if (CharacterVerticalState != VerticalStates.HANGING)
            {
                if (BlockOn(1, 0.1f) == Map.WALL || BlockOn(1, 0.1f) == Map.LEFT_SHELF || BlockOn(1, 0.1f) == Map.RIGHT_SHELF)
                {
                    CharacterVerticalState = VerticalStates.STANDING;
                    Position = new Vector2(Position.X, Map.Round(Position.Y));
                }
                else
                {
                    for (int i = -1; i < Size.Y; i++)
                        if (BlockOn(-i, 0.1f) == Map.LADDER)
                        {
                            CharacterVerticalState = VerticalStates.CLIMBING;
                            ClimpAxeleration.Y = LinearAxeleration;
                            if (ClimpAxeleration.Y > LinearMaxSpeed)
                                ClimpAxeleration.Y = LinearMaxSpeed;
                        }
                }
            }
            /*else
            {
                if (BlockOn(1, 0.1f) == Map.WALL || BlockOn(1, 0.1f) == Map.LEFT_SHELF || BlockOn(1, 0.1f) == Map.RIGHT_SHELF)
                {
                    CharacterVerticalState = VerticalStates.STANDING;
                    Position = new Vector2(Position.X, Map.Round(Position.Y));
                }
                else
                {
                    for (int i = -1; i < Size.Y; i++)
                    {
                        uint block = BlockOn(-i, 0.1f);
                        if (block == Map.LEFT_SHELF || block == Map.RIGHT_SHELF)
                        {
                            CharacterVerticalState = VerticalStates.HANGING;
                            ClimpAxeleration.Y = LinearAxeleration;
                        }
                    }
                }
            }*/
        }

        public virtual void Crouch()
        {
            if (CharacterVerticalState == VerticalStates.STANDING || CharacterVerticalState == VerticalStates.STANDING_UP)
            {
                CharacterVerticalState = VerticalStates.CROUCHING;
                Delay = CrouchingDelay;
            }
        }

        public virtual void StandUp()
        {
            if ((CharacterVerticalState == VerticalStates.STEALING || CharacterVerticalState == VerticalStates.CROUCHING) && BlockOn(- 1 - (int)Size.Y, 0.4f) != Map.WALL)
            {
                CharacterVerticalState = VerticalStates.STANDING_UP;
                Delay = CrouchingDelay;
            }
        }

        public virtual void Jump()
        {
            if (CharacterVerticalState != VerticalStates.FALLING)
            {
                CharacterVerticalState = VerticalStates.FALLING;
                FallingSpeed = -JumpAxeleration;
            }
        }

        public virtual void TouchDown()
        {
            CharacterVerticalState = VerticalStates.STANDING;
            FallingSpeed = 0;
        }

        public virtual void Hang()
        {
            CharacterVerticalState = VerticalStates.HANGING;
            FallingSpeed = 0;
        }

        public virtual bool OnFloor()
        {
            uint block = BlockOn(1, 0.4f);
            return (block == Map.WALL || block == Map.LEFT_SHELF || block == Map.RIGHT_SHELF);
        }

        public virtual uint BlockOn(int verticalOffset, float CollisionFactor)
        {
            for (uint i = (uint)Map.Round(Position.X - COLLIZION_RANGE * 0.4f); i <= (uint)Map.Round(Position.X + Size.X - 1 + COLLIZION_RANGE * CollisionFactor); i++)
                if (World[i, (uint)(Map.Round(Position.Y) + verticalOffset)] == Map.WALL)
                    return Map.WALL;
            for (uint i = (uint)Map.Round(Position.X - COLLIZION_RANGE * 0.4f); i <= (uint)Map.Round(Position.X + Size.X - 1 + COLLIZION_RANGE * CollisionFactor); i++)
                if (World[i, (uint)(Map.Round(Position.Y) + verticalOffset)] != Map.EMPTY)
                    return World[i, (uint)(Map.Round(Position.Y) + verticalOffset)];
            return Map.EMPTY;
        }

        public virtual void Use()
        {
        }

        public virtual void Fall()
        {
            CharacterVerticalState = VerticalStates.FALLING;
            if (UseGravity && FallingSpeed < FallingMaxSpeed)
            {
                FallingSpeed += Gravity;
                if (FallingSpeed > FallingMaxSpeed) FallingSpeed = FallingMaxSpeed;
            }
        }

        public virtual bool BlockIsSolid(uint Block)
        {
            return Block == Map.WALL || Block == Map.LEFT_SHELF || Block == Map.RIGHT_SHELF || Block == Map.LADDER;
        }

        public virtual bool BlockIsPassable(uint Block)
        {
            return Block != Map.WALL;
        }

        public virtual void StartClimbing()
        {

        }

        public virtual void StartHanging()
        {

        }

        void UpdatePhisics(float ms)
        {
            float realLinearSpeed = 0;
            float realFallingSpeed = 0;
            if (CharacterVerticalState == VerticalStates.CLIMBING || CharacterVerticalState == VerticalStates.HANGING)
            #region Speed update on climbing or hanging 
            {
                if (CharacterHorisontalState == HorisontalStates.NOT_MOVING)
                {
                    ClimpSpeed += ClimpAxeleration * ms;
                    if (ClimpSpeed.Length() > LinearMaxSpeed)
                    {
                        ClimpSpeed.Normalize();
                        ClimpSpeed *= LinearMaxSpeed;
                    }
                    realLinearSpeed = ClimpSpeed.X;
                    realFallingSpeed = ClimpSpeed.Y;
                }
                else
                    Fall();
            }
            #endregion
            if (CharacterVerticalState != VerticalStates.CLIMBING && CharacterVerticalState != VerticalStates.HANGING)
            #region Speed update on walking
            {
                if (CharacterHorisontalState == HorisontalStates.NOT_MOVING)
                {
                    if (LinearSpeed < 0)
                    {
                        LinearSpeed += LinearAxeleration * ms * 5f;
                        if (LinearSpeed > 0)
                        {
                            LinearSpeed = 0;
                        }
                    }
                    if (LinearSpeed > 0)
                    {
                        LinearSpeed -= LinearAxeleration * ms * 5f;
                        if (LinearSpeed < 0)
                        {
                            LinearSpeed = 0;
                        }
                    }
                }
                if (CharacterHorisontalState == HorisontalStates.GO_TO_THE_LEFT)
                {
                    LinearSpeed -= LinearAxeleration * ms;
                    if (LinearSpeed < -LinearMaxSpeed)
                        LinearSpeed = -LinearMaxSpeed;
                }
                if (CharacterHorisontalState == HorisontalStates.GO_TO_THE_RIGHT)
                {
                    LinearSpeed += LinearAxeleration * ms;
                    if (LinearSpeed > LinearMaxSpeed)
                        LinearSpeed = LinearMaxSpeed;
                }
                realLinearSpeed = LinearSpeed * ms;
                realFallingSpeed = FallingSpeed * ms;
            }
            #endregion
            uint Obstacle = Map.EMPTY;
            if (CharacterHorisontalState != HorisontalStates.NOT_MOVING)
            {
                uint startX = realLinearSpeed < 0 ? (uint)Map.Round(Position.X) : (uint)Map.Round(Position.X + Size.X - 1);
                uint endX = realLinearSpeed < 0 ? (uint)Map.Round(Position.X + realLinearSpeed - COLLIZION_RANGE * 0.5f) : (uint)Map.Round(Position.X + Size.X - 1 + realLinearSpeed + COLLIZION_RANGE * 0.5f);
                uint startY = realFallingSpeed < 0 ? (uint)Map.Round(Position.Y) : (uint)Map.Round(Position.Y - (Size.Y - 1));
                uint endY = realFallingSpeed < 0 ? (uint)Map.Round(Position.Y - (Size.Y - 1) + realFallingSpeed) : (uint)Map.Round(Position.Y + realFallingSpeed);
                for (uint i = startX; (realLinearSpeed < 0 ? i >= endX : i <= endX) && Obstacle == Map.EMPTY; i = realLinearSpeed < 0 ? i - 1 : i + 1)
                    for (uint j = startY; (realFallingSpeed < 0 ? j >= endY : j <= endY) && Obstacle == Map.EMPTY; j = realFallingSpeed < 0 ? j - 1 : j + 1)
                        if (!BlockIsPassable(World[i, j]))
                        {
                            Obstacle = Map.WALL;
                            Position = new Vector2(realLinearSpeed < 0 ? i + COLLIZION_RANGE : i + Size.X - 1 - COLLIZION_RANGE, Position.Y);
                            LinearSpeed = 0;
                        }
            }
            Obstacle = Map.EMPTY;
            if (CharacterVerticalState == VerticalStates.FALLING)
            #region Falling
            {
                uint startX = realLinearSpeed < 0 ? (uint)Map.Round(Position.X) : (uint)Map.Round(Position.X + Size.X - 1);
                uint endX = realLinearSpeed < 0 ? (uint)Map.Round(Position.X + realLinearSpeed) : (uint)Map.Round(Position.X + Size.X - 1 + realLinearSpeed);
                uint startY = /*realFallingSpeed < 0 ?*/ (uint)Map.Round(Position.Y - (Size.Y - 1)) /*: (uint)Map.Round(Position.Y)*/;
                uint endY = realFallingSpeed < 0 ? (uint)Map.Round(Position.Y - (Size.Y - 1) + realFallingSpeed - COLLIZION_RANGE * 0.5f) : (uint)Map.Round(Position.Y + realFallingSpeed + COLLIZION_RANGE * 0.5f);
                for (uint j = startY; (realFallingSpeed < 0 ? j >= endY : j <= endY) && Obstacle == Map.EMPTY; j = realFallingSpeed < 0 ? j - 1 : j + 1)
                    for (uint i = startX; (realLinearSpeed < 0 ? i >= endX : i <= endX) && Obstacle == Map.EMPTY; i = realLinearSpeed < 0 ? i - 1 : i + 1)
                        if (CharacterVerticalState == VerticalStates.FALLING)
                        {
                            if (World[i, j] == Map.WALL || World[i, j] == Map.LADDER)
                            {
                                Obstacle = Map.WALL;
                                Position = new Vector2(Position.X, realFallingSpeed < 0 ? j + (Size.Y - 1) + COLLIZION_RANGE : j - COLLIZION_RANGE);
                                TouchDown();
                            }
                        }
            }
            #endregion
            Obstacle = Map.EMPTY;
            if (CharacterVerticalState != VerticalStates.CLIMBING && CharacterVerticalState != VerticalStates.HANGING)
                if (!OnFloor())
                    Fall();
            if (CharacterVerticalState == VerticalStates.CLIMBING)
            {
                uint startX = realLinearSpeed < 0 ? (uint)Map.Round(Position.X) : (uint)Map.Round(Position.X + Size.X - 1);
                uint endX = realLinearSpeed < 0 ? (uint)Map.Round(Position.X + realLinearSpeed) : (uint)Map.Round(Position.X + Size.X - 1 + realLinearSpeed);
                uint startY = /*realFallingSpeed < 0 ?*/ (uint)Map.Round(Position.Y - (Size.Y - 1)) /*: (uint)Map.Round(Position.Y)*/;
                uint endY = realFallingSpeed < 0 ? (uint)Map.Round(Position.Y - (Size.Y - 1) + realFallingSpeed - COLLIZION_RANGE * 0.5f) : (uint)Map.Round(Position.Y + realFallingSpeed + COLLIZION_RANGE * 0.5f);
                for (uint j = startY; (realFallingSpeed < 0 ? j >= endY : j <= endY) && Obstacle == Map.EMPTY; j = realFallingSpeed < 0 ? j - 1 : j + 1)
                    for (uint i = startX; (realLinearSpeed < 0 ? i >= endX : i <= endX) && Obstacle == Map.EMPTY; i = realLinearSpeed < 0 ? i - 1 : i + 1)
                        if (CharacterVerticalState == VerticalStates.FALLING)
                        {
                            if (World[i, j] == Map.WALL || World[i, j] == Map.LADDER)
                            {
                                Obstacle = Map.WALL;
                                Position = new Vector2(Position.X, realFallingSpeed < 0 ? j + (Size.Y - 1) + COLLIZION_RANGE : j - COLLIZION_RANGE);
                            }
                        }
            }
        }

        void UpdateControl(float ms)
        {
            float realLinearSpeed = 0;
            float realFallingSpeed = 0;
            if (CharacterVerticalState == VerticalStates.CLIMBING || CharacterVerticalState == VerticalStates.HANGING)
            {
                realLinearSpeed = ClimpSpeed.X;
                realFallingSpeed = ClimpSpeed.Y;
            }
            else
            {
                realLinearSpeed = LinearSpeed * ms;
                realFallingSpeed = FallingSpeed * ms;
            }
            if (CharacterHorisontalState != HorisontalStates.NOT_MOVING)
            {
                Position += Vector2.UnitX * realLinearSpeed;
            }
            Position += Vector2.UnitY * realFallingSpeed;
            if ((CharacterVerticalState == VerticalStates.CLIMBING || CharacterVerticalState == VerticalStates.HANGING) && ClimpAxeleration.Length() != 0)
            {
                ClimpAxeleration = Vector2.Zero;
                ClimpSpeed = Vector2.Zero;
            }
        }

        void UpdateMovies(float ms)
        {
            if (Delay > 0)
            {
                Delay -= ms;
            }
            else
            {
                Delay = 0;
                if (CharacterVerticalState == VerticalStates.CROUCHING)
                {
                    CharacterVerticalState = VerticalStates.STEALING;
                }
                else if (CharacterVerticalState == VerticalStates.STANDING_UP)
                {
                    CharacterVerticalState = VerticalStates.STANDING;
                }
                else if (CharacterVerticalState == VerticalStates.STANDING_UP)
                {
                    CharacterVerticalState = VerticalStates.STANDING;
                }
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            float ms = gameTime.ElapsedGameTime.Milliseconds / 1000f;
            if (Delay == 0)
            {
                UpdatePhisics(ms);
                UpdateControl(ms);
            }
            else //Scripted Moovies
            {
                UpdateMovies(ms);
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch, Camera camera)
        {

        }
    }
}
