using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightRise.BaseClasses
{
    public class GameObject
    {
        protected Level World;
        public Vector2 Position { get; protected set; }
        public Vector2 Size { get; protected set; }

        public GameObject(Level World, Vector2 Position, Vector2 Size)
        {
            this.World = World;
            this.Position = Position;
            this.Size = Size;
        }

        public virtual void Update(GameTime gameTime) { }

        public virtual void Draw(SpriteBatch surface, Camera camera) { }
    }
}
