using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightRise.BaseClasses
{
    public class Character : GameObject
    {
        public new Vector2 Size
        {
            get
            {
                if (crouching)
                    return new Vector2(base.Size.X, base.Size.Y * 0.5f);
                else
                    return base.Size;
            }
            protected set { base.Size = value; }
        }
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
        protected Vector2 VectorView;
        protected float delay;
        protected float delayCounter;

        public Character(Level world, Vector2 position, Vector2 size, float linearAxeleration, float linearMaxSpeed, float gravity, float fallingMaxSpeed, float jumpAxeleration) : 
            base (world, position, size)
        {
            LinearAxeleration = linearAxeleration;
            LinearMaxSpeed = linearMaxSpeed;
            Gravity = gravity;
            FallingMaxSpeed = fallingMaxSpeed;
            JumpAxeleration = jumpAxeleration;
            CurrentUpdater = GoUpdater;
            CurrentHorisontalUpdater = WalkUpdater;
            VectorView = Vector2.UnitX;
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
        protected Updater CurrentUpdater;
        protected Updater CurrentHorisontalUpdater;

        protected bool crouching;

        private bool isTakeRoof(float realFallingSpeed)
        {
            uint startX = (uint)Map.Round(Position.X);
            uint endX = (uint)Map.Round(Position.X + Size.X - 1);
            uint startY = (uint)Map.Round(Position.Y - (Size.Y - 1));
            uint endY = (uint)Map.Round(Position.Y - (Size.Y - 1) + realFallingSpeed - COLLIZION_RANGE * 0.5f);
            for (uint j = startY; j >= endY; j--)
                for (uint i = startX; i <= endX; i = i + 1)
                    if (World.Map[i, j] == Map.WALL && World.Map[i, j + 1] != Map.WALL)
                        return true;
            return false;
        }

        private bool isTakeGround(float realFallingSpeed)
        {
            uint startX = (uint)Map.Round(Position.X);
            uint endX = (uint)Map.Round(Position.X + Size.X - 1);
            uint startY = (uint)Map.Round(Position.Y);
            uint endY = (uint)Map.Round(Position.Y + realFallingSpeed + COLLIZION_RANGE * 0.5f);
            for (uint j = startY; j <= endY; j++)
                for (uint i = startX; i <= endX; i = i + 1)
                    if (World.Map[i, j] >= Map.WALL && World.Map[i, j] < Map.LADDER && World.Map[i, j - 1] != Map.WALL)
                        return true;
            return false;
        }

        private bool isTakeShelf()
        {
            uint startX = (uint)Map.Round(Position.X);
            uint endX = (uint)Map.Round(Position.X + Size.X - 1);
            uint startY = (uint)Map.Round(Position.Y - (Size.Y - 1) + COLLIZION_RANGE * -0.5f);
            uint endY = (uint)Map.Round(Position.Y - (Size.Y / 2 - 1) + COLLIZION_RANGE * 0.5f);
            for (uint j = startY; j <= endY; j++)
                for (uint i = startX; i <= endX; i = i + 1)
                    if ((World.Map[i, j] == Map.LEFT_SHELF && VectorView.X < 0) ||
                        World.Map[i, j] == Map.RIGHT_SHELF && VectorView.X > 0)
                        return true;
            return false;
        }

        private bool isTakeLadder()
        {
            uint startX = (uint)Map.Round(Position.X - COLLIZION_RANGE * 0.5f);
            uint endX = (uint)Map.Round(Position.X + (Size.X - 1) + COLLIZION_RANGE * 0.5f);
            uint startY = (uint)Map.Round(Position.Y - (Size.Y - 1));
            uint endY = (uint)Map.Round(Position.Y);
            for (uint j = startY; j <= endY; j++)
                for (uint i = startX; i <= endX; i = i + 1)
                    if (World.Map[i, j] == Map.LADDER)
                        return true;
            return false;
        }

        private bool isTakeLadderBottom()
        {
            uint startX = (uint)Map.Round(Position.X - COLLIZION_RANGE * 0.5f);
            uint endX = (uint)Map.Round(Position.X + (Size.X - 1) + COLLIZION_RANGE * 0.5f);
            uint Y = (uint)Map.Round(Position.Y + 1);
            for (uint i = startX; i <= endX; i = i + 1)
                if (World.Map[i, Y] == Map.LADDER)
                    return true;
            return false;
        }

        private bool isNoTakeLadderTop()
        {
            uint startX = (uint)Map.Round(Position.X);
            uint endX = (uint)Map.Round(Position.X + (Size.X - 1));
            uint Y = (uint)Map.Round(Position.Y - Size.Y);
            for (uint i = startX; i <= endX; i = i + 1)
                if (World.Map[i, Y] != Map.LADDER)
                    return true;
            return false;
        }

        //1 Падает
        protected virtual void FallUpdater(float ms)
        {
            float realFallingSpeed = FallingSpeed * ms;
            if (realFallingSpeed < 0) //Если скорость вниз отрицательна (прыжок, взлёт) искать потолок
            {
                if (isTakeRoof(realFallingSpeed)) //Если сверху потолок - падать
                {
                    CurrentUpdater = TouchRoofUpdater;
                    return;
                }

            }
            else
            {
                if (isTakeGround(realFallingSpeed)) //Если снизу пол - приземлится
                {
                    CurrentUpdater = LandUpdater;
                    return;
                }
                if (isTakeShelf()) //Если балкон - зацепится
                {
                    CurrentUpdater = ClingUpdater;
                    return;
                }
                if (isTakeLadder() && (toUp || toDown)) //Если лестница и [Вверх] или [Вниз] - хвататься за лестницу
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
            float realLinearSpeed = LinearSpeed * ms;
            if (Math.Abs(LinearSpeed) > 0.05f)
                LinearSpeed -= LinearSpeed * 0.01f;
            else
                LinearSpeed = 0;
            uint startX = realLinearSpeed < 0 ? (uint)Map.Round(Position.X) : (uint)Map.Round(Position.X + Size.X - 1);
            uint endX = realLinearSpeed < 0 ? (uint)Map.Round(Position.X + realLinearSpeed - COLLIZION_RANGE * 0.5f) : (uint)Map.Round(Position.X + (Size.X - 1) + realLinearSpeed - COLLIZION_RANGE * 0.5f);
            uint startY = (uint)Map.Round(Position.Y - (Size.Y - 1));
            uint endY = (uint)Map.Round(Position.Y);
            for (uint i = startX; (realLinearSpeed < 0 ? i >= endX : i <= endX); i = realLinearSpeed < 0 ? i - 1 : i + 1)
                for (uint j = startY; j <= endY; j++)
                    if (World.Map[i, j] == Map.WALL)
                    {
                        Position = new Vector2(realLinearSpeed < 0 ? i + COLLIZION_RANGE : i - (Size.X - 1), Position.Y);
                        LinearSpeed = 0;
                        return;
                    }
            Position += Vector2.UnitX * realLinearSpeed;
            //CurrentHorisontalUpdater(ms);
        }

        //2 Цепляется (Scripted)
        protected virtual void ClingUpdater(float ms)
        {
            if (delayCounter >= delay)
            {
                LinearSpeed = 0;
                CurrentUpdater = HangUpdater;
                delayCounter = 0;
                delay = 0;
            }
            else
            {
                delayCounter += ms;
                uint startX = (uint)Map.Round(Position.X);
                uint endX = (uint)Map.Round(Position.X + Size.X - 1);
                uint startY = (uint)Map.Round(Position.Y - (Size.Y - 1) - COLLIZION_RANGE * 0.5f);
                uint endY = (uint)Map.Round(Position.Y - (Size.Y / 2 - 1) + COLLIZION_RANGE * 0.5f);
                for (uint j = startY; j <= endY; j++)
                    for (uint i = startX; i <= endX; i = i + 1)
                        if ((World.Map[i, j] == Map.LEFT_SHELF && VectorView.X < 0) || 
                            World.Map[i, j] == Map.RIGHT_SHELF && VectorView.X > 0)
                        {
                            Position = new Vector2(World.Map[i, j] == Map.LEFT_SHELF ? i - COLLIZION_RANGE * 0.5f : i - (Size.X - 1) + COLLIZION_RANGE * 0.5f, j + (Size.Y - 1) + COLLIZION_RANGE);
                            return;
                        }
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
            if (toLeft != toRight)
            {
                if (toLeft)
                    LinearSpeed = -LinearAxeleration * ms * 20f;
                if (toRight)
                    LinearSpeed = LinearAxeleration * ms * 20f;
            }
            CurrentUpdater = FallUpdater;
        }

        //5 Висит
        protected virtual void HangUpdater(float ms)
        {
            if (toUp) //Если [Вверх] - залезть
            {
                CurrentUpdater = RiseUpdater;
                return;
            }
            if (toDown) //Если [Вниз] - спрыгнуть
            {
                CurrentUpdater = ComeDownUpdater;
                return;
            }
        }

        //6 Стоит
        protected virtual void GoUpdater(float ms)
        {
            if (toUse)//Если [Использовать] - использовать
            {
                foreach (var a in World.Interactives)
                {
                    Vector2 diff = a.Value.Position - Position;
                    if (diff.Length() <= a.Value.InteractionRadius)
                    {
                        CurrentUpdater = UseUpdater;
                        a.Value.ToBreakInteraction += () => CurrentUpdater = GoUpdater;
                        a.Value.Interact();
                        break;
                    }
                }
            }
            if (toJump) //Если [Прыжок] - прыгнуть
            {
                CurrentUpdater = JumpUpdater;
                return;
            }
            if (isTakeLadderBottom() && toDown) //Если [Вниз] и снизу лестница - спуститься на лестницу
            {
                CurrentUpdater = LadderEnterDownUpdater;
            }
            
            if (isTakeLadder() && toUp) //Если [Вверх] и рядом лестница - подняться на лестницу
            {
                CurrentUpdater = LadderEnterUpUpdater;
            }
            //Ходьба
            CurrentHorisontalUpdater(ms);
            //Падение
            if (!isTakeGround(ms) && !isTakeLadderBottom())
            {
                CurrentUpdater = ComeDownUpdater;
                return;
            }
            else 
                FallingSpeed = 0;

        }

        //7 Залазит (Scripted)
        protected virtual void RiseUpdater(float ms)
        {
            if (delayCounter >= delay)
            {
                delayCounter = 0;
                delay = 0;
                uint startX = (uint)Map.Round(Position.X);
                uint endX = (uint)Map.Round(Position.X + Size.X - 1);
                uint startY = (uint)Map.Round(Position.Y - (Size.Y - 1));
                uint endY = (uint)Map.Round(Position.Y + COLLIZION_RANGE * 0.5f);
                for (uint j = startY; j <= endY; j++)
                    for (uint i = startX; i <= endX; i = i + 1)
                        if (World.Map[i, j] >= Map.LEFT_SHELF && World.Map[i, j] <= Map.RIGHT_SHELF)
                        {
                            Position = new Vector2(Position.X, j - COLLIZION_RANGE);
                            CurrentUpdater = GoUpdater;
                            return;
                        }
            }
            else
            {
                Position -= Vector2.UnitY * Size.Y * (ms / delay) * 1.1f;
                delayCounter += ms;
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
                    if (World.Map[i, j] == Map.LADDER)
                    {
                        while (World.Map[i - 1, j] == Map.LADDER) i--;
                        Position = new Vector2(i + COLLIZION_RANGE * 0.49f, Position.Y);
                        CurrentUpdater = LadderGoUpdater;
                        return;
                    }
            CurrentUpdater = GoUpdater;
        }

        //9 Спускается на лестницу (Scripted)
        protected virtual void LadderEnterDownUpdater(float ms)
        {
            if (delayCounter >= delay)
            {
                delayCounter = 0;
                delay = 0;
                uint startX = (uint)Map.Round(Position.X - COLLIZION_RANGE * 0.5f);
                uint endX = (uint)Map.Round(Position.X + (Size.X - 1) + COLLIZION_RANGE * 0.5f);
                uint Y = (uint)Map.Round(Position.Y);
                for (uint i = startX; i <= endX; i = i + 1)
                    if (World.Map[i, Y] == Map.LADDER)
                    {
                        while (World.Map[i - 1, Y] == Map.LADDER) i--;
                        Position = new Vector2(i + COLLIZION_RANGE * 0.49f, Y);
                        CurrentUpdater = LadderGoUpdater;
                        return;
                    }
                CurrentUpdater = LadderGoUpdater;
            }
            else
            {
                uint startX = (uint)Map.Round(Position.X - COLLIZION_RANGE * 0.5f);
                uint endX = (uint)Map.Round(Position.X + (Size.X - 1) + COLLIZION_RANGE * 0.5f);
                uint Y = (uint)Map.Round(Position.Y + Size.Y);
                for (uint i = startX; i <= endX; i = i + 1)
                    if (World.Map[i, Y] == Map.LADDER)
                    {
                        delayCounter += ms;
                        while (World.Map[i - 1, Y] == Map.LADDER) i--;
                        Position = new Vector2(i + COLLIZION_RANGE * 0.49f, Position.Y + Size.Y * (ms / delay));
                        return;
                    }
                delayCounter += ms;
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
                    if (World.Map[i, j] == Map.LADDER)
                    {
                        while (World.Map[i - 1, j] == Map.LADDER) i--;
                        Position = new Vector2(i + COLLIZION_RANGE * 0.49f, Position.Y);
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
                if (FallingSpeed < LinearMaxSpeed * 0.5f)
                    FallingSpeed = -LinearMaxSpeed * 0.5f;
            }
            else if (toDown) //Если [Низ]
            {
                FallingSpeed += LinearAxeleration;
                if (FallingSpeed > LinearMaxSpeed * 0.5f)
                    FallingSpeed = LinearMaxSpeed * 0.5f;
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
                if (isTakeRoof(realFallingSpeed)) //Если сверху потолок - остановиться
                {
                    FallingSpeed = 0;
                    return;
                }
                if (isNoTakeLadderTop()) //Если нет лестницы - подняться
                {
                    CurrentUpdater = LadderLeaveUpUpdater;
                    return;
                }
            }
            else if (realFallingSpeed > 0)
            {
                if (isTakeGround(realFallingSpeed)) //Если снизу пол - приземлится
                {
                    CurrentUpdater = LadderLeaveDownUpdater;
                    return;
                }
                if (!isTakeLadder()) //Если нет лестницы - падать
                {
                    CurrentUpdater = LadderLeaveDownUpdater;
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
            if (delayCounter >= delay)
            {
                delayCounter = 0;
                delay = 0;
                uint X = (uint)Map.Round(Position.X);
                uint Y = (uint)Map.Round(Position.Y);
                while (World.Map[X - 1, Y] == Map.LADDER) X--;
                while (World.Map[X, Y] == Map.LADDER) Y--;
                Position = new Vector2(X + COLLIZION_RANGE * 0.49f, Y);
                CurrentUpdater = GoUpdater;
                return;
            }
            else
            {
                uint X = (uint)Map.Round(Position.X);
                uint Y = (uint)Map.Round(Position.Y);
                while (World.Map[X - 1, Y] == Map.LADDER) X--;
                while (World.Map[X, Y] == Map.LADDER) Y--;
                Position = new Vector2(X + COLLIZION_RANGE * 0.49f, Y + Size.Y * (1 - (delayCounter / delay)));
                delayCounter += ms;
            }
        }

        //13 Спускается с лестницы (Scripted)
        protected virtual void LadderLeaveDownUpdater(float ms)
        {
            if (delayCounter >= delay)
            {
                delayCounter = 0;
                delay = 0;
                Position = new Vector2(Position.X, Map.Round(Position.Y));
                CurrentUpdater = GoUpdater;
            }
            else
            {
                delayCounter += ms;
            }
        }

        //14 Приземлиться (Scripted)
        protected virtual void LandUpdater(float ms)
        {
            if (toLeft != toRight)
            {
                float realFallingSpeed = FallingSpeed * ms;
                uint startX = (uint)Map.Round(Position.X);
                uint endX = (uint)Map.Round(Position.X + Size.X - 1);
                uint startY = (uint)Map.Round(Position.Y);
                uint endY = (uint)Map.Round(Position.Y + realFallingSpeed + COLLIZION_RANGE * 0.5f);
                for (uint j = startY; j <= endY; j++)
                    for (uint i = startX; i <= endX; i = i + 1)
                        if (World.Map[i, j] >= Map.WALL && World.Map[i, j] < Map.LADDER && World.Map[i, j - 1] != Map.WALL)
                        {
                            FallingSpeed = 0;
                            Position = new Vector2(Position.X, j - COLLIZION_RANGE);
                            CurrentUpdater = GoUpdater;
                            delayCounter = 0;
                            delay = 0;
                            return;
                        }
            }
            if (delayCounter >= delay)
            {
                CurrentUpdater = GoUpdater;
                delayCounter = 0;
                delay = 0;
            }
            else
            {
                delayCounter += ms;
                LinearSpeed = 0;
                float realFallingSpeed = FallingSpeed * ms;
                uint startX = (uint)Map.Round(Position.X);
                uint endX = (uint)Map.Round(Position.X + Size.X - 1);
                uint startY = (uint)Map.Round(Position.Y);
                uint endY = (uint)Map.Round(Position.Y + realFallingSpeed + COLLIZION_RANGE * 0.5f);
                for (uint j = startY; j <= endY; j++)
                    for (uint i = startX; i <= endX; i = i + 1)
                        if (World.Map[i, j] >= Map.WALL && World.Map[i, j] < Map.LADDER && World.Map[i, j - 1] != Map.WALL)
                        {
                            FallingSpeed = 0;
                            Position = new Vector2(Position.X, j - COLLIZION_RANGE);
                            return;
                        }
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
                    if (World.Map[i, j] == Map.WALL)
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
            if (delayCounter >= delay)
            {
                delayCounter = 0;
                delay = 0;
                crouching = !crouching;
                CurrentHorisontalUpdater = WalkUpdater;
                LinearSpeed = 0;
            }
            else
            {
                delayCounter += ms;
            }
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
                uint startX = realLinearSpeed < 0 ? (uint)Map.Round(Position.X) : (uint)Map.Round(Position.X + (Size.X - 1));
                uint endX = realLinearSpeed < 0 ? (uint)Map.Round(Position.X + realLinearSpeed - COLLIZION_RANGE * 0.5f) : (uint)Map.Round(Position.X + (Size.X - 1) + realLinearSpeed - COLLIZION_RANGE * 0.5f);
                uint startY = (uint)Map.Round(Position.Y - (Size.Y - 1));
                uint endY = (uint)Map.Round(Position.Y);
                for (uint i = startX; (realLinearSpeed < 0 ? i >= endX : i <= endX); i = realLinearSpeed < 0 ? i - 1 : i + 1)
                    for (uint j = startY; j <= endY; j++)
                        if (World.Map[i, j] == Map.WALL)
                        {
                            LinearSpeed = 0;
                            Position = new Vector2(realLinearSpeed < 0 ? i + COLLIZION_RANGE : i - (Size.X - 1), Position.Y);
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
            if (toCrouch && !isTakeRoof(-1))//Если [Присесть] - присесть/встать
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
                uint startX = realLinearSpeed < 0 ? (uint)Map.Round(Position.X) : (uint)Map.Round(Position.X + (Size.X - 1));
                uint endX = realLinearSpeed < 0 ? (uint)Map.Round(Position.X + realLinearSpeed - COLLIZION_RANGE * 0.5f) : (uint)Map.Round(Position.X + (Size.X - 1) + realLinearSpeed - COLLIZION_RANGE * 0.5f);
                uint startY = (uint)Map.Round(Position.Y - (Size.Y - 1));
                uint endY = (uint)Map.Round(Position.Y);
                for (uint i = startX; (realLinearSpeed < 0 ? i >= endX : i <= endX); i = realLinearSpeed < 0 ? i - 1 : i + 1)
                    for (uint j = startY; j <= endY; j++)
                        if (World.Map[i, j] == Map.WALL)
                        {
                            CurrentHorisontalUpdater = TouchTheWallUpdater;
                            return;
                        }
                Position += Vector2.UnitX * realLinearSpeed;
                if (realLinearSpeed > 0)
                    VectorView = Vector2.UnitX;
                else
                    VectorView = -Vector2.UnitX;
            }
            else
            {
                if (LinearSpeed != 0)
                    CurrentHorisontalUpdater = StopUpdater;
            }
        }
        #endregion

        public override void Update(GameTime gameTime)
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

        public override void Draw(SpriteBatch spriteBatch, Camera camera)
        {

        }
    }
}
