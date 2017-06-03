using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightRise.BaseClasses {
    public abstract class Interaction
    {
        delegate void Act(Character interactor);

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch, Camera camera);
    }

    public class InteractivePoint : Interaction
    {
        public Vector2 Position { get; set; }
        public float InteractionRadius { get; set; }
        public event Action ToInteract;
        public event Action ToBreakInteraction;

        public InteractivePoint(Vector2 position, float radius) {
            Position = position;
            InteractionRadius = radius;
        }

        public virtual void Interact()
        {
            ToInteract();
        }

        public virtual void BreakInteraction()
        {
            ToBreakInteraction();
            ToBreakInteraction = null;
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(SpriteBatch spriteBatch, Camera camera)
        {
        }
    }

    public class InteractiveArea : Interaction
    {
        protected Level World;

        public Rectangle InteractionArea { get; set; }

        public event Action<KeyValuePair<string, GameObject>> Impact;

        public InteractiveArea(Level world, Rectangle area)
        {
            World = world;
            InteractionArea = area;
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var obj in World.Objects)
            {
                Rectangle r2 = new Rectangle((obj.Value.Position - new Vector2(0, obj.Value.Size.Y)).ToPoint(), obj.Value.Size.ToPoint());
                if (InteractionArea.Contains(r2))
                    Impact(obj);
            }
        }

        public override void Draw(SpriteBatch surface, Camera camera)
        {
#if DEBUG
            surface.Begin();
            Color color = Color.OrangeRed;
            surface.Draw(SimpleUtils.WhiteRect, new Rectangle(camera.WorldToWindow(InteractionArea.Location.ToVector2()), (InteractionArea.Size.ToVector2() * camera.Scale.X).ToPoint()), color);
            surface.End();
#endif
        }
    }
}
