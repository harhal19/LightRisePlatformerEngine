using LightRise.BaseClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightRise.Main {
    class FirstHack : HackScreen {

        public FirstHack(SpriteFont font, SpriteBatch spriteBatch, Texture2D terminal, Comp comp) : base(font, spriteBatch, terminal) {
            Computer = comp;
            HackScreen.TextContainer word = new HackScreen.TextContainer(new Rectangle(112, 52, 225, 93));
            TextObject textObj = new TextObject(font, "bool AllowToGo( ) {\n    ActivateSignalization( );\n    return        ;\n}");
            word.textObject = textObj;
            Words.Add(word);
            word = new HackScreen.TextContainer(new Rectangle(93, 250, 343, 124));
            textObj = new TextObject(font, "void ActivateSignalization( ) {\n  if (       )\n    Alarm( );\n}");
            word.textObject = textObj;
            Words.Add(word);
            word = new HackScreen.TextContainer(new Rectangle(173, 99, 57, 31));
            textObj = new TextObject(font, "false");
            textObj.color = Color.Red;
            word.textObject = textObj;
            textObj.Draggable = true;
            Words.Add(word);
            word = new HackScreen.TextContainer(new Rectangle(134, 282, 57, 31));
            textObj = new TextObject(font, "true");
            textObj.color = Color.Blue;
            word.textObject = textObj;
            textObj.Draggable = true;
            Words.Add(word);
        }

        public override void Update(GameTime gameTime, StepState State) {
            base.Update(gameTime, State);

            if (State.Keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape)) {
                Computer.Allowed = (Words[2].textObject != null) ? Words[2].textObject.Text == "true" : false;
                //Program.MainThread.HackScreen = null;
                Comp.inUse = false;
                //Program.MainThread.Player.Locked = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Camera cam) {
        }
    }
}
