using LightRise.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LightRise.Main {
    class SecondComp : Comp {
        public SecondComp(Point position, GraphicsDevice device) : base(position, device) {
        }

        public override void Complete( ) {
        }

        public override void Connect( ) {
            base.Connect( );
            //Program.MainThread.HackScreen = Program.MainThread.SecondHackScreen;//new FirstHack(Program.MainThread.HackFont, Program.MainThread.SpriteBatch, Program.MainThread.Terminal);

        }

        public override void Draw(SpriteBatch surface, Camera camera)
        {
            if (texture == null)
                surface.Draw(SimpleUtils.WhiteRect, new Rectangle(camera.WorldToWindow(GridPosition.ToVector2() + new Vector2(0.1f)), (camera.Scale * 0.8f).ToPoint()), Color.Purple);
            else
                surface.Draw(texture, new Rectangle(camera.WorldToWindow(GridPosition.ToVector2() + new Vector2(0.1f)), (camera.Scale).ToPoint()), Allowed ? Color.SpringGreen : Color.PaleVioletRed);
        }

        public override void Update(StepState state) {
        }
    }
}
