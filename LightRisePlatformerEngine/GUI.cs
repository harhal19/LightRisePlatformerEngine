using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using LightRise.Main;

namespace LightRise.BaseClasses {
    abstract class GUI : Instance {

        protected GraphicsDeviceManager Graphics;

        public GUI(Point position, GraphicsDevice device, GraphicsDeviceManager graphics) : base(position, device) {
            Graphics = graphics;
        }


        public override void Update(StepState state) {
            if (state.Keyboard.IsKeyDown(Keys.Escape)) {
                //Program.MainThread.GUIes.Remove(this);
                //Program.MainThread.Player.Locked = false;
            }
        }
    }
}
