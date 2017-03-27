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
        public Vector2 Size { get; protected set; }
        public Vector2 Position { get; protected set; }
        /*public Rectangle GridRectangle
        {
            get {
                return new Rectangle((Position - new Vector2(0, Size.Y - 1)).ToPoint(), (Size - Vector2.One).ToPoint());
            }
        }
        public Rectangle GetCollisionRectangle(GameTime gameTime)
        {
            float realLinearSpeed = LinearSpeed * gameTime.ElapsedGameTime.Milliseconds / 1000f;
            float realFallingSpeed = FallingSpeed * gameTime.ElapsedGameTime.Milliseconds / 1000f;
            Vector2 collisionPosition = Position - new Vector2(0, Size.Y) + new Vector2(realLinearSpeed < 0 ? realLinearSpeed : 0, realFallingSpeed < 0 ? -realFallingSpeed : 0);
            /*if (realLinearSpeed > 0 && (realLinearSpeed - (int)realLinearSpeed != 0)) realLinearSpeed++;
            if (realLinearSpeed > 0 && (realLinearSpeed - (int)realLinearSpeed != 0)) realLinearSpeed = -realLinearSpeed + 1;
            if (realFallingSpeed > 0 && (realFallingSpeed - (int)realFallingSpeed != 0)) realFallingSpeed = realFallingSpeed + 1;
            if (realLinearSpeed > 0 && (realLinearSpeed - (int)realLinearSpeed != 0)) realLinearSpeed++;*//*
            Vector2 collisionSize = Size - Vector2.One + new Vector2(Math.Abs(realLinearSpeed), Math.Abs(realFallingSpeed));
            return new Rectangle(collisionPosition.ToPoint(), collisionSize.ToPoint());
        }*/
        public float LinearSpeed { get; protected set; }
        float LinearAxeleration;
        float LinearMaxSpeed;
        float Gravity;
        public float FallingSpeed { get; protected set; }
        float FallingMaxSpeed;
        float JumpAxeleration;
        public bool UseGravity = true;
        const float COLLIZION_RANGE = 1f;

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
            //if (LinearSpeed < 0)
            {
                LinearSpeed += LinearAxeleration;
                if (LinearSpeed > 0)
                    LinearSpeed = 0;
            }
            //if (LinearSpeed > 0)
            {
                LinearSpeed -= LinearAxeleration;
                if (LinearSpeed < 0)
                    LinearSpeed = 0;
            }
        }

        public virtual void Left()
        {
            //if (FallingSpeed == 0)
            {
                LinearSpeed -= LinearAxeleration;
                if (LinearSpeed < -LinearMaxSpeed)
                    LinearSpeed = -LinearMaxSpeed;
            }
        }

        public virtual void Right()
        {
            //if (FallingSpeed == 0)
            {
                LinearSpeed += LinearAxeleration;
                if (LinearSpeed > LinearMaxSpeed)
                    LinearSpeed = LinearMaxSpeed;
            }
        }

        public virtual void Jump()
        {
            if (OnFloor())
            {
                FallingSpeed = -JumpAxeleration;
            }
        }

        public virtual void TouchDown()
        {
            FallingSpeed = 0;
        }

        public virtual bool BrickOnTop()
        {
            for (uint i = (uint)Map.Round(Position.X - COLLIZION_RANGE * 0.4f); i <= (uint)Map.Round(Position.X + Size.X - 1 + COLLIZION_RANGE * 0.4f); i++)
                if (World[i, (uint)Map.Round(Position.Y - (Size.Y - 1)) - 1] != Map.EMPTY)
                    return true;
            return false;
        }

        public virtual uint FloorBottom()
        {
            for (uint i = (uint)Map.Round(Position.X - COLLIZION_RANGE * 0.4f); i <= (uint)Map.Round(Position.X + Size.X - 1 + COLLIZION_RANGE * 0.4f); i++)
                if (World[i, (uint)Map.Round(Position.Y) + 1] == Map.WALL)
                    return Map.WALL;
            for (uint i = (uint)Map.Round(Position.X - COLLIZION_RANGE * 0.4f); i <= (uint)Map.Round(Position.X + Size.X - 1 + COLLIZION_RANGE * 0.4f); i++)
                if (World[i, (uint)Map.Round(Position.Y) + 1] != Map.EMPTY)
                    return World[i, (uint)Map.Round(Position.Y) + 1];
            return Map.EMPTY;
        }

        public virtual bool OnFloor()
        {
            return FloorBottom() != Map.EMPTY;
        }

        public virtual void Use()
        {
        }

        public virtual void Fall()
        {
            if (UseGravity && FallingSpeed < FallingMaxSpeed)
            {
                FallingSpeed += Gravity;
                if (FallingSpeed > FallingMaxSpeed) FallingSpeed = FallingMaxSpeed;
            }
        }

        public virtual bool BlockIsFallable(uint Block)
        {
            return Block != Map.WALL && (FallingSpeed < 0 || (Block != Map.LEFT_SHELF && Block != Map.RIGHT_SHELF));
        }

        public virtual bool BlockIsPassable(uint Block)
        {
            return Block != Map.WALL;
        }

        public virtual void Update(GameTime gameTime)
        {
            float realLinearSpeed = LinearSpeed * gameTime.ElapsedGameTime.Milliseconds / 1000f;
            float realFallingSpeed = FallingSpeed * gameTime.ElapsedGameTime.Milliseconds / 1000f;
            uint Obstacle = Map.EMPTY;/*
            for (uint i = (uint)GridRectangle.Bottom; i < (uint)CollisionRectangle.Bottom && Obstacle == Map.EMPTY; i++)
            {
                float diagFactor = (int)FallingSpeed / (float)(i - GridRectangle.Bottom);
                for (uint j = (uint)CollisionRectangle.Left; i < (uint)CollisionRectangle.Right && Obstacle == Map.EMPTY; j++)
                {
                    if (World[j, i] == Map.WALL) Obstacle = Map.WALL;
                    Position += Vector2.UnitX * (LinearSpeed * gameTime.ElapsedGameTime.Milliseconds / 1000f);
                }
                for (uint j = (uint)Position.X; i < (uint)(Position.X + Size.X) && Obstacle == Map.EMPTY; j++)
                    if (World[i, j] == Map.RIGHT_SHELF || World[i, j] == Map.LEFT_SHELF) Obstacle = World[i, j];
            }
            if (Obstacle != Map.EMPTY)
                FallingSpeed = 0;
        */
            if (realLinearSpeed != 0)
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
                if (Obstacle == Map.EMPTY)
                    Position += Vector2.UnitX * realLinearSpeed;
            }
            Obstacle = Map.EMPTY;
            if (!OnFloor())
                Fall();
            Obstacle = Map.EMPTY;
            if (realFallingSpeed != 0)
            {
                uint startX = realLinearSpeed < 0 ? (uint)Map.Round(Position.X) : (uint)Map.Round(Position.X + Size.X - 1);
                uint endX = realLinearSpeed < 0 ? (uint)Map.Round(Position.X + realLinearSpeed) : (uint)Map.Round(Position.X + Size.X - 1 + realLinearSpeed);
                uint startY = realFallingSpeed < 0 ? (uint)Map.Round(Position.Y - (Size.Y - 1)) : (uint)Map.Round(Position.Y);
                uint endY = realFallingSpeed < 0 ? (uint)Map.Round(Position.Y - (Size.Y - 1) + realFallingSpeed - COLLIZION_RANGE * 0.5f) : (uint)Map.Round(Position.Y + realFallingSpeed + COLLIZION_RANGE * 0.5f);
                for (uint j = startY; (realFallingSpeed < 0 ? j >= endY : j <= endY) && Obstacle == Map.EMPTY; j = realFallingSpeed < 0 ? j - 1 : j + 1)
                    for (uint i = startX; (realLinearSpeed < 0 ? i >= endX : i <= endX) && Obstacle == Map.EMPTY; i = realLinearSpeed < 0 ? i - 1 : i + 1)
                        if (!BlockIsFallable(World[i, j]))
                        {
                            Obstacle = Map.WALL;
                            Position = new Vector2(Position.X, realFallingSpeed < 0 ? j + (Size.Y - 1) + COLLIZION_RANGE : j  - COLLIZION_RANGE);
                            TouchDown();
                        }
                if (Obstacle == Map.EMPTY && UseGravity)
                    Position += Vector2.UnitY * realFallingSpeed;
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch, Camera camera)
        {

        }
    }
}
