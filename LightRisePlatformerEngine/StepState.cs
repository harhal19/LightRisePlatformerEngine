using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightRise.Main {
    public class StepState {

        public readonly GameTime Time;
        public readonly KeyboardState Keyboard;
        public readonly MouseState Mouse;

        public StepState(GameTime time, KeyboardState keyboard_state, MouseState mouse_state) {
            Time = time;
            Keyboard = keyboard_state;
            Mouse = mouse_state;
        }

    }
}
