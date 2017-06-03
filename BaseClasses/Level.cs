using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightRise.BaseClasses
{
    public abstract class Level
    {
        public Map Map;
        protected Camera Cam;
        protected RenderTarget2D[ ] Renders;
        public Dictionary<string, GameObject> Objects;
        public Dictionary<string, InteractivePoint> Interactives;
        public Dictionary<string, InteractiveArea> ActiveZones;
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime, SpriteBatch SpriteBatch);

        public Level()
        {
            Objects = new Dictionary<string, GameObject>();
            Interactives = new Dictionary<string, InteractivePoint>();
            ActiveZones = new Dictionary<string, InteractiveArea>();
        }
    }
}
