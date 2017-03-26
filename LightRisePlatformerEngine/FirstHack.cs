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
    class FirstHack : HackScreen
    {
        public FirstHack(SpriteFont font, SpriteBatch spriteBatch, Texture2D terminal) : base (font, spriteBatch, terminal)
        {
            HackScreen.TextContainer word = new HackScreen.TextContainer(new Rectangle(112, 50, 225, 93));
            TextObject textObj = new TextObject(font, "bool AllowToGo() {\n  return        ;\n }");
            word.textObject = textObj;
            Words.Add(word);
            word = new HackScreen.TextContainer(new Rectangle(93, 250, 343, 124));
            textObj = new TextObject(font, "void ActivateSignalization() {\n  if (        )\n    Alarm;\n }");
            word.textObject = textObj;
            Words.Add(word);
            word = new HackScreen.TextContainer(new Rectangle(204, 83, 57, 31));
            textObj = new TextObject(font, "false");
            textObj.color = Color.Red;
            word.textObject = textObj;
            textObj.Draggable = true;
            Words.Add(word);
            word = new HackScreen.TextContainer(new Rectangle(142, 282, 57, 31));
            textObj = new TextObject(font, "true");
            textObj.color = Color.Blue;
            word.textObject = textObj;
            textObj.Draggable = true;
            Words.Add(word);
        }

        public void Update(GameTime gameTime, StepState State)
        {
        }

        public void Draw(SpriteBatch spriteBatch, Camera cam)
        {
        }
    }
}
