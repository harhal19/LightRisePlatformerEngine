using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightRise.BaseClasses
{
    public class TextObject {
        public SpriteFont Font;
        float AnimationTimer;
        float Speed = 3f;
        public string Text;
        public string FutureText;
        public Vector2 Pos = Vector2.Zero;
        public float Scale = 1f;
        public Color color = Color.Green;
        public bool Draggable = false;
        public Vector2 offset;

        public Rectangle GetRectangle()
        {
            return new Rectangle((Pos - Font.MeasureString(Text) * Scale / 2).ToPoint(), (Pos + Font.MeasureString(Text) * Scale / 2).ToPoint());
        }

        public TextObject(SpriteFont font, string text) : this(font, text, text) { }

        public TextObject(SpriteFont font, string text, string futureText) {
            Font = font;
            Text = text;
            FutureText = futureText;
            AnimationTimer = (text.Length - futureText.Length) / Speed;
        }

        public void Update(GameTime gameTime, Vector2 offset) {
            this.offset = offset;
            if (AnimationTimer > 0) {
                AnimationTimer -= gameTime.ElapsedGameTime.Milliseconds / 1000f;
                Text = FutureText.Substring(0, FutureText.Length - (int)(AnimationTimer * Speed));
            }

        }

        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.DrawString(Font, Text, Pos + offset, color, 0, Vector2.Zero, Scale, SpriteEffects.None, 0);
        }

        public void DrawOnCenter(SpriteBatch spriteBatch)
        {
            try
            {
                spriteBatch.DrawString(Font, Text, Pos - (Font.MeasureString(Text) * Scale / 2) + offset, color, 0, Vector2.Zero, Scale, SpriteEffects.None, 0);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
