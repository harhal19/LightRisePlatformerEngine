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
                if (crouching)
                    return new Vector2(size.X, size.Y * 0.5f);
                else
                    return size;
            }
            protected set { size = value; }
        }
        public Vector2 Position { get; protected set; }
        public float LinearSpeed { get; protected set; }
        float LinearAxeleration;
        float LinearMaxSpeed
        {
            get
            {
                if (crouching)
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
            CurrentUpdater = FallUpdater;
            CurrentHorisontalUpdater = WalkUpdater;
        }

        protected bool toJump;
        protected bool toCrouch;
        protected bool toLeft;
        protected bool toRight;
        protected bool toUse;
        protected bool toUp;
        protected bool toDown;

        public virtual void Jump()   { toJump = true; }
        public virtual void Crouch() { toCrouch = true; }
        public virtual void Left()   { toLeft = true; }
        public virtual void Right()  { toRight = true; }
        public virtual void Use()    { toUse = true; }
        public virtual void Up()     { toUp = true; }
        public virtual void Down()   { toDown = true; }

        protected delegate void Updater(float ms);
        Updater CurrentUpdater;
        Updater CurrentHorisontalUpdater;

        protected bool crouching;

        private bool takeRoof(float realFallingSpeed)
        {
            uint startX = (uint)Map.Round(Position.X);
            uint endX = (uint)Map.Round(Position.X + Size.X - 1);
            uint startY = (uint)Map.Round(Position.Y - (Size.Y - 1));
            uint endY = (uint)Map.Round(Position.Y - (Size.Y - 1) + realFallingSpeed - COLLIZION_RANGE * 0.5f);
            for (uint j = startY; j >= endY; j--)
                for (uint i = startX; i <= endX; i = i + 1)
                    if (World[i, j] == Map.WALL)
                        return true;
            return false;
        }

        private bool takeGround(float realFallingSpeed)
        {
            uint startX = (uint)Map.Round(Position.X);
            uint endX = (uint)Map.Round(Position.X + Size.X - 1);
            uint startY = (uint)Map.Round(Position.Y);
            uint endY = (uint)Map.Round(Position.Y + realFallingSpeed + COLLIZION_RANGE * 0.5f);
            for (uint j = startY; j <= endY; j++)
                for (uint i = startX; i <= endX; i = i + 1)
                    if (World[i, j] >= Map.WALL && World[i, j] < Map.LADDER)
                        return true;
            return false;
        }

        private bool takeShelf()
        {
            uint startX = (uint)Map.Round(Position.X);
            uint endX = (uint)Map.Round(Position.X + Size.X - 1);
            uint startY = (uint)Map.Round(Position.Y - (Size.Y / 2 - 1) - COLLIZION_RANGE * 0.5f);
            uint endY = (uint)Map.Round(Position.Y /*- (Size.Y / 2 - 1)*/ + COLLIZION_RANGE * 0.5f);
            for (uint j = startY; j <= endY; j++)
                for (uint i = startX; i <= endX; i = i + 1)
                    if (World[i, j] == Map.LEFT_SHELF || 
                        World[i, j] == Map.RIGHT_SHELF)
                        return true;
            return false;
        }

        private bool takeLadder()
        {
            uint startX = (uint)Map.Round(Position.X - COLLIZION_RANGE * 0.5f);
            uint endX = (uint)Map.Round(Position.X + (Size.X - 1) + COLLIZION_RANGE * 0.5f);
            uint startY = (uint)Map.Round(Position.Y - (Size.Y - 1));
            uint endY = (uint)Map.Round(Position.Y);
            for (uint j = startY; j <= endY; j++)
                for (uint i = startX; i <= endX; i = i + 1)
                    if (World[i, j] == Map.LADDER)
                        return true;
            return false;
        }

        private bool takeLadderBottom()
        {
            uint startX = (uint)Map.Round(Position.X - COLLIZION_RANGE * 0.5f);
            uint endX = (uint)Map.Round(Position.X + (Size.X - 1) + COLLIZION_RANGE * 0.5f);
            uint Y = (uint)Map.Round(Position.Y + 1);
            for (uint i = startX; i <= endX; i = i + 1)
                if (World[i, Y] == Map.LADDER)
                    return true;
            return false;
        }

        //1 Падает
        protected virtual void FallUpdater(float ms)
        {
            float realFallingSpeed = FallingSpeed * ms;
            if (realFallingSpeed < 0) //Если скорость вниз отрицательна (прыжок, взлёт) искать потолок
            {
                if (takeRoof(realFallingSpeed)) //Если сверху потолок - падать
                {
                    CurrentUpdater = TouchRoofUpdater;
                    return;
                }

            }
            else
            {
                if (takeGround(realFallingSpeed)) //Если снизу пол - приземлится
                {
                    CurrentUpdater = LandUpdater;
                    return;
                }
                if (takeShelf()) //Если балкон - зацепится
                {
                    CurrentUpdater = ClingUpdater;
                    return;
                }
                if (takeLadder() && (toUp || toDown)) //Если лестница и [Вверх] или [Вниз] - хвататься за лестницу
                {
                    CurrentUpdater = LadderCatchUpdater;
                    return;
                }
            }
            
            //Иначе падать вниз
            Position += Vector2.UnitY * realFallingSpeed;
            if (FallingSpeed < FallingMaxSpeed)
            {
                FallingSpeed += Gravity;
                if (FallingSpeed > FallingMaxSpeed) FallingSpeed = FallingMaxSpeed;
            }
            CurrentHorisontalUpdater(ms);
        }

        //2 Цепляется (Scripted)
        protected virtual void ClingUpdater(float ms)
        {
            uint startX = (uint)Map.Round(Position.X);
            uint endX = (uint)Map.Round(Position.X + Size.X - 1);
            uint startY = (uint)Map.Round(Position.Y - (Size.Y - 1) - COLLIZION_RANGE * 0.5f);
            uint endY = (uint)Map.Round(Position.Y - (Size.Y / 2 - 1) + COLLIZION_RANGE * 0.5f);
            for (uint j = startY; j <= endY; j++)
                for (uint i = startX; i <= endX; i = i + 1)
                    if (World[i, j] >= Map.LEFT_SHELF && World[i, j] <= Map.RIGHT_SHELF)
                    {
                        Position = new Vector2(World[i, j] == Map.LEFT_SHELF ? i : i - (Size.X - 1), j + (Size.Y - 1) - 1 + COLLIZION_RANGE);
                        CurrentUpdater = HangUpdater;
                        return;
                    }
        }

        //3 Использует (Scripted)
        protected virtual void UseUpdater(float ms)
        {
            CurrentUpdater = GoUpdater;
        }

        //4 Прыгает (Scripted)
        protected virtual void JumpUpdater(float ms)
        {
            FallingSpeed = -JumpAxeleration;
            CurrentUpdater = FallUpdater;
        }

        //5 Висит
        protected virtual void HangUpdater(float ms)
        {
            if (toUp) //Если [Вверх] - залезть
            {
                CurrentUpdater = JumpUpdater;// RiseUpdater;
                return;
            }
            if (toDown) //Если [Вниз] - спрыгнуть
            {
                CurrentUpdater = ComeDownUpdater;
                return;
            }
        }

        //6 Стоит/Крадётся
        protected virtual void GoUpdater(float ms)
        {
            //Если [Использовать] - использовать
            
            if (toJump) //Если [Прыжок] - прыгнуть
            {
                CurrentUpdater = JumpUpdater;
                return;
            }
            if (takeLadderBottom() && toDown) //Если [Вниз] и снизу лестница - спуститься на лестницу
            {
                CurrentUpdater = LadderEnterDownUpdater;
            }
            
            if (takeLadder() && toUp) //Если [Вверх] и рядом лестница - подняться на лестницу
            {
                CurrentUpdater = LadderEnterUpUpdater;
            }
            //Ходьба
            CurrentHorisontalUpdater(ms);
            //Падение
            if (!takeGround(ms) && !takeLadderBottom())
            {
                CurrentUpdater = FallUpdater;
                return;
            }

        }

        //7 Залазит (Scripted)
        protected virtual void RiseUpdater(float ms)
        {
            uint startX = (uint)Map.Round(Position.X);
            uint endX = (uint)Map.Round(Position.X + Size.X - 1);
            uint startY = (uint)Map.Round(Position.Y - (Size.Y - 1));
            uint endY = (uint)Map.Round(Position.Y);
            for (uint j = startY; j <= endY; j++)
                for (uint i = startX; i <= endX; i = i + 1)
                    if (World[i, j] >= Map.LEFT_SHELF && World[i, j] <= Map.RIGHT_SHELF)
                    {
                        Position = new Vector2(Position.X, j - COLLIZION_RANGE);
                        CurrentUpdater = GoUpdater;
                        return;
                    }
        }

        //8 Взбирается на лестницу (Scripted)
        protected virtual void LadderEnterUpUpdater(float ms)
        {
            uint startX = (uint)Map.Round(Position.X - COLLIZION_RANGE * 0.5f);
            uint endX = (uint)Map.Round(Position.X + Size.X - 1 + COLLIZION_RANGE * 0.5f);
            uint startY = (uint)Map.Round(Position.Y - (Size.Y - 1));
            uint endY = (uint)Map.Round(Position.Y);
            for (uint j = startY; j <= endY; j++)
                for (uint i = startX; i <= endX; i = i + 1)
                    if (World[i, j] == Map.LADDER)
                    {
                        while (World[i - 1, j] == Map.LADDER) i--;
                        Position = new Vector2(i, Position.Y);
                        CurrentUpdater = LadderGoUpdater;
                        return;
                    }
            CurrentUpdater = GoUpdater;
        }

        //9 Спускается на лестницу (Scripted)
        protected virtual void LadderEnterDownUpdater(float ms)
        {
            uint startX = (uint)Map.Round(Position.X - COLLIZION_RANGE * 0.5f);
            uint endX = (uint)Map.Round(Position.X + (Size.X - 1) + COLLIZION_RANGE * 0.5f);
            uint Y = (uint)Map.Round(Position.Y + 1);
            for (uint i = startX; i <= endX; i = i + 1)
                if (World[i, Y] == Map.LADDER)
                {
                    Position = new Vector2(i, Y);
                    CurrentUpdater = LadderGoUpdater;
                    return;
                }
        }

        //10 Хватается за лестницу (Scripted)
        protected virtual void LadderCatchUpdater(float ms)
        {
            uint startX = (uint)Map.Round(Position.X - COLLIZION_RANGE * 0.5f);
            uint endX = (uint)Map.Round(Position.X + Size.X - 1 + COLLIZION_RANGE * 0.5f);
            uint startY = (uint)Map.Round(Position.Y - (Size.Y - 1));
            uint endY = (uint)Map.Round(Position.Y);
            for (uint j = startY; j <= endY; j++)
                for (uint i = startX; i <= endX; i = i + 1)
                    if (World[i, j] == Map.LADDER)
                    {
                        while (World[i - 1, j] == Map.LADDER) i--;
                        Position = new Vector2(i, Position.Y);
                        CurrentUpdater = LadderGoUpdater;
                        return;
                    }
        }

        //11 Лезет по лестнице
        protected virtual void LadderGoUpdater(float ms)
        {
            if (toUp) //Если [Вверх]
            {
                FallingSpeed -= LinearAxeleration;
                if (FallingSpeed < LinearMaxSpeed)
                    FallingSpeed = -LinearMaxSpeed;
            }
            else if (toDown) //Если [Низ]
            {
                FallingSpeed += LinearAxeleration;
                if (FallingSpeed > LinearMaxSpeed)
                    FallingSpeed = LinearMaxSpeed;
            }
            else
            {
                FallingSpeed = 0;
            }
            if (toJump) //Если [Прыжок] - падать
            {
                CurrentUpdater = ComeDownUpdater;
                return;
            }
            float realFallingSpeed = FallingSpeed * ms;
            if (realFallingSpeed < 0) //Если скорость вниз отрицательна (прыжок, взлёт) искать потолок
            {
                if (takeRoof(realFallingSpeed)) //Если сверху потолок - остановиться
                {
                    FallingSpeed = 0;
                    return;
                }
                if (!takeLadder()) //Если нет лестницы - падать
                {
                    CurrentUpdater = LadderLeaveUpUpdater;
                    return;
                }
            }
            else
            {
                if (takeGround(realFallingSpeed)) //Если снизу пол - приземлится
                {
                    CurrentUpdater = LadderLeaveDownUpdater;
                    return;
                }
                if (!takeLadder()) //Если нет лестницы - падать
                {
                    CurrentUpdater = ComeDownUpdater;
                    return;
                }
            }

            //Иначе падать вниз
            Position += Vector2.UnitY * realFallingSpeed;
            if (FallingSpeed < FallingMaxSpeed)
            {
                FallingSpeed += Gravity;
                if (FallingSpeed > FallingMaxSpeed) FallingSpeed = FallingMaxSpeed;
            }
        }

        //12 Взбирается с лестницы (Scripted)
        protected virtual void LadderLeaveUpUpdater(float ms)
        {
            uint X = (uint)Map.Round(Position.X);
            uint Y = (uint)Map.Round(Position.Y);
            while (World[X, Y] == Map.LADDER)
                Y--;
                Position = new Vector2(X, Y);
                CurrentUpdater = GoUpdater;
                return;
        }

        //13 Спускается с лестницы (Scripted)
        protected virtual void LadderLeaveDownUpdater(float ms)
        {
            CurrentUpdater = GoUpdater;
        }

        //14 Приземлиться (Scripted)
        protected virtual void LandUpdater(float ms)
        {
            float realFallingSpeed = FallingSpeed * ms;
            uint startX = (uint)Map.Round(Position.X);
            uint endX = (uint)Map.Round(Position.X + Size.X - 1);
            uint startY = (uint)Map.Round(Position.Y);
            uint endY = (uint)Map.Round(Position.Y + realFallingSpeed + COLLIZION_RANGE * 0.5f);
            for (uint j = startY; j <= endY; j++)
                for (uint i = startX; i <= endX; i = i + 1)
                    if (World[i, j] >= Map.WALL && World[i, j] < Map.LADDER)
                    {
                        FallingSpeed = 0;
                        Position = new Vector2(Position.X, j - COLLIZION_RANGE);
                        CurrentUpdater = GoUpdater;
                        return;
                    }
        }

        //15 Спрыгнуть (Scripted)
        protected virtual void ComeDownUpdater(float ms)
        {
            CurrentUpdater = FallUpdater;
        }

        //16 Коснуться потолка (Scripted)
        protected virtual void TouchRoofUpdater(float ms)
        {
            float realFallingSpeed = FallingSpeed * ms;
            uint startX = (uint)Map.Round(Position.X);
            uint endX = (uint)Map.Round(Position.X + Size.X - 1);
            uint startY = (uint)Map.Round(Position.Y - (Size.Y - 1));
            uint endY = (uint)Map.Round(Position.Y - (Size.Y - 1) + realFallingSpeed - COLLIZION_RANGE * 0.5f);
            for (uint j = startY; j >= endY; j--)
                for (uint i = startX; i <= endX; i = i + 1)
                    if (World[i, j] == Map.WALL)
                    {
                        FallingSpeed = 0;
                        Position = new Vector2(Position.X, j + (Size.Y - 1) + COLLIZION_RANGE);
                        CurrentUpdater = FallUpdater;
                        return;
                    }
        }

        #region HorisontalUpdaters
        protected virtual void CrouchUpdater(float ms)
        {
            crouching = !crouching;
            CurrentHorisontalUpdater = WalkUpdater;
        }

        protected virtual void StopUpdater(float ms)
        {
            LinearSpeed = 0;
            CurrentHorisontalUpdater = WalkUpdater;
        }

        // Коснуться стены (Scripted)
        protected virtual void StayWithWallUpdater(float ms)
        {
            if (toCrouch)//Если [Присесть] - присесть/встать
            {
                CurrentHorisontalUpdater = CrouchUpdater;
                return;
            }
            if (toLeft != toRight)
            {
                float realLinearSpeed = 0;
                if (toLeft)
                {
                    LinearSpeed -= LinearAxeleration * ms;
                    if (LinearSpeed < -LinearMaxSpeed)
                        LinearSpeed = -LinearMaxSpeed;
                }
                else if (toRight)
                {
                    LinearSpeed += LinearAxeleration * ms;
                    if (LinearSpeed > LinearMaxSpeed)
                        LinearSpeed = LinearMaxSpeed;
                }
                realLinearSpeed = LinearSpeed * ms;
                uint startX = realLinearSpeed < 0 ? (uint)Map.Round(Position.X) : (uint)Map.Round(Position.X + Size.X - 1);
                uint endX = realLinearSpeed < 0 ? (uint)Map.Round(Position.X + realLinearSpeed - COLLIZION_RANGE * 0.5f) : (uint)Map.Round(Position.X + Size.X - 1 + realLinearSpeed + COLLIZION_RANGE * 0.5f);
                uint startY = (uint)Map.Round(Position.Y - (Size.Y - 1));
                uint endY = (uint)Map.Round(Position.Y);
                for (uint i = startX; (realLinearSpeed < 0 ? i >= endX : i <= endX); i = realLinearSpeed < 0 ? i - 1 : i + 1)
                    for (uint j = startY; j <= endY; j++)
                        if (World[i, j] == Map.WALL)
                        {
                            LinearSpeed = 0;
                            Position = new Vector2(realLinearSpeed < 0 ? i + COLLIZION_RANGE : i + Size.X - 1 - COLLIZION_RANGE, Position.Y);
                            return;
                        }
                Position += Vector2.UnitX * realLinearSpeed;
                CurrentHorisontalUpdater = LeaveTheWallUpdater;
            }
        }

        protected virtual void TouchTheWallUpdater(float ms)
        {
            CurrentHorisontalUpdater = StayWithWallUpdater;
        }

        protected virtual void LeaveTheWallUpdater(float ms)
        {
            CurrentHorisontalUpdater = WalkUpdater;
        }

        protected virtual void WalkUpdater(float ms)
        {
            if (toCrouch)//Если [Присесть] - присесть/встать
            {
                CurrentHorisontalUpdater = CrouchUpdater;
                return;
            }
            if (toLeft != toRight)
            {
                float realLinearSpeed = 0;
                if (toLeft)
                {
                    LinearSpeed -= LinearAxeleration * ms;
                    if (LinearSpeed < -LinearMaxSpeed)
                        LinearSpeed = -LinearMaxSpeed;
                }
                else if (toRight)
                {
                    LinearSpeed += LinearAxeleration * ms;
                    if (LinearSpeed > LinearMaxSpeed)
                        LinearSpeed = LinearMaxSpeed;
                }
                realLinearSpeed = LinearSpeed * ms;
                uint startX = realLinearSpeed < 0 ? (uint)Map.Round(Position.X) : (uint)Map.Round(Position.X + Size.X - 1);
                uint endX = realLinearSpeed < 0 ? (uint)Map.Round(Position.X + realLinearSpeed - COLLIZION_RANGE * 0.5f) : (uint)Map.Round(Position.X + Size.X - 1 + realLinearSpeed + COLLIZION_RANGE * 0.5f);
                uint startY = (uint)Map.Round(Position.Y - (Size.Y - 1));
                uint endY = (uint)Map.Round(Position.Y);
                for (uint i = startX; (realLinearSpeed < 0 ? i >= endX : i <= endX); i = realLinearSpeed < 0 ? i - 1 : i + 1)
                    for (uint j = startY; j <= endY; j++)
                        if (World[i, j] == Map.WALL)
                        {
                            CurrentHorisontalUpdater = TouchTheWallUpdater;
                            return;
                        }
                Position += Vector2.UnitX * realLinearSpeed;
            }
            else
            {
                if (LinearSpeed != 0)
                    CurrentHorisontalUpdater = StopUpdater;
            }
        }
        #endregion

        public virtual void Update(GameTime gameTime)
        {
            if (CurrentUpdater != null)
                CurrentUpdater(gameTime.ElapsedGameTime.Milliseconds / 1000f);
            toJump = false;
            toCrouch = false;
            toLeft = false;
            toRight = false;
            toUse = false;
            toUp = false;
            toDown = false;
        }

        public virtual void Draw(SpriteBatch spriteBatch, Camera camera)
        {

        }
    }
}
