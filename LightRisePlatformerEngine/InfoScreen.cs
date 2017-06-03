using LightRise.BaseClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using static LightRise.Main.HackScreen;

namespace LightRise.Main
{
    class InfoScreen
    {
        Texture2D BG;
        TextObject Text;
        Vector2 Size = new Vector2(800, 600);
        SpriteFont Font;
        public Action close;
        GraphicsDevice GraphicsDevice;

        Vector2 Pos
        {
            get
            {
                return GraphicsDevice.Viewport.Bounds.Size.ToVector2() / 2 -
                    Size / 2;
            }
        }

        public InfoScreen(SpriteFont font, string text, GraphicsDevice graphicsDevice, Texture2D terminal)
        {
            Font = font;
            this.GraphicsDevice = graphicsDevice;
            Text = new TextObject(font, text);
            BG = terminal;
        }

        public virtual void Update(GameTime gameTime, StepState State)
        {
            Text.Update(gameTime, Pos + Vector2.One * 100);
            if (State.Mouse.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                //Microsoft.VisualBasic.Interaction.InputBox("MemberName"); 
                close();
            }
        }

        public void Draw(Camera cam, SpriteBatch spriteBatch)
        {
            Rectangle ORR = spriteBatch.GraphicsDevice.ScissorRectangle;
            RasterizerState RS = new RasterizerState();
            RS.ScissorTestEnable = true;
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, RS);
            spriteBatch.Draw(BG, new Rectangle(Pos.ToPoint(), Size.ToPoint()), Color.White);
            Rectangle window = new Rectangle(Pos.ToPoint(), Size.ToPoint());
            spriteBatch.GraphicsDevice.ScissorRectangle = window;
            if (Text != null)
            {
                Text.Draw(spriteBatch);
            }
            spriteBatch.End();
        }
    }
}
