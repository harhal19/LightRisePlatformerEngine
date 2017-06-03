using LightRise.BaseClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightRise.Main
{
    class SecondHack : HackScreen
    {
        public SecondHack(SpriteFont font, SpriteBatch spriteBatch, Texture2D terminal, Comp comp) : base (font, spriteBatch, terminal)
        {
            Computer = comp;
            HackScreen.TextContainer word = new HackScreen.TextContainer(new Rectangle(50, 50, 358, 124));
            TextObject textObj = new TextObject(font, "void OpenDoor() {\n  if (AccessAllowed ==          )\n    Door.Open;\n }");
            word.textObject = textObj;
            Words.Add(word);
            word = new HackScreen.TextContainer(new Rectangle(330, 80, 57, 31));
            textObj = new TextObject(font, "true");
            textObj.color = Color.Red;
            word.textObject = textObj;
            textObj.Draggable = true;
            Words.Add(word);
            //this.Items = items;
        }

        public override void Update(GameTime gameTime, StepState State)
        {
            base.Update(gameTime, State);

            if (State.Keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                Computer.Allowed = (Words[1].textObject != null) ? Words[1].textObject.Text == "false" : false;
                //if (Computer.Allowed) Program.MainThread.script2();
                //Program.MainThread.HackScreen = null;
                Comp.inUse = false;
                //Program.MainThread.Player.Locked = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Camera cam)
        {
        }
    }
}
