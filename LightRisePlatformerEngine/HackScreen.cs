using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using LightRise.BaseClasses;
using LightRise.WinUtilsLib;
using System;
using System.Collections.Generic;

namespace LightRise.Main
{
    public abstract class HackScreen
    {
        public class TextContainer
        {
            public Rectangle Rect;
            Color color = Color.Pink;
            Vector2 offset;
            public TextObject textObject;

            public TextContainer(Rectangle Rect)
            {
                this.Rect = Rect;
            }

            public void Update(GameTime gameTime, Vector2 offset)
            {
                this.offset = offset;
                if (textObject != null)
                    textObject.Update(gameTime, Rect.Center.ToVector2() + offset);
            }

            public void Draw(SpriteBatch spriteBatch)
            {
                if (textObject == null)
                {
                    Rectangle rect = Rect;
                    rect.Offset(offset);
                    spriteBatch.Draw(SimpleUtils.WhiteRect, rect, color);
                }
                else
                {
                    textObject.DrawOnCenter(spriteBatch);
                }
            }
        }
        SpriteFont Font;
        Texture2D BG;
        public List<TextContainer> Words;
        public List<TextObject> Items;
        Rectangle savedItems;
        
        Tuple<TextObject, int, bool> DraggableText;
        Vector2 Size = new Vector2(800, 600);
        SpriteBatch spriteBatch;
        Vector2 Pos
        {
            get
            {
                return spriteBatch.GraphicsDevice.Viewport.Bounds.Size.ToVector2() / 2 -
                    Size / 2;
            }
        }

        public HackScreen(SpriteFont font, SpriteBatch spriteBatch, Texture2D terminal)
        {
            Font = font;
            this.spriteBatch = spriteBatch;
            Words = new List<TextContainer>();
            Items = new List<TextObject>();
            BG = terminal;
            //savedItems = new Rectangle(635, 130, 182, 375);
            savedItems = new Rectangle(530, 116, 182, 375);
        }

        public void Update(GameTime gameTime, StepState State)
        {
            if (Words != null)
                for (int i = 0; i < Words.Count; i++)
                    Words[i].Update(gameTime, Pos);
            if (Items != null)
                for (int i = 0; i < Items.Count; i++)
                {
                    Items[i].Pos = savedItems.Location.ToVector2() + Vector2.UnitX * (savedItems.Width / 2 - 5) + Vector2.UnitY * (i * 25 + 10);
                    Items[i].Update(gameTime, Pos);
                }
            Point mousePos = State.Mouse.Position;
            if (DraggableText == null)
            {
                if (State.Mouse.LeftButton == ButtonState.Pressed)
                {
                    for (int i = 0; i < Words.Count && DraggableText == null && Words[i].textObject != null; i++)
                    {
                        Rectangle item = Words[i].Rect;
                        item.Offset(Pos);
                        if (item.Contains(mousePos) && Words[i].textObject.Draggable)
                        {
                            DraggableText = new Tuple<TextObject, int, bool>(Words[i].textObject, i, true);
                            Words[i].textObject = null;
                        }
                    }
                    for (int i = 0; i < Items.Count && DraggableText == null; i++)
                    {
                        Rectangle item = Items[i].GetRectangle();
                        item.Offset(Pos);
                        if (item.Contains(mousePos))
                        {
                            DraggableText = new Tuple<TextObject, int, bool>(Items[i], i, false);
                            Items.RemoveAt(i);
                        }
                    }
                }
            }
            else
            {
                DraggableText.Item1.Update(gameTime, Pos);
                DraggableText.Item1.Pos = mousePos.ToVector2() - Pos;
                if (State.Mouse.LeftButton == ButtonState.Released)
                {
                    Rectangle obj = DraggableText.Item1.GetRectangle();
                    obj.Offset(Pos);
                    for (int i = 0; i < Words.Count; i++)
                    {
                        Rectangle word = Words[i].Rect;
                        word.Offset(Pos);
                        if (word.Intersects(obj) && (Words[i].textObject == null ? true : Words[i].textObject.Draggable) &&
                            i != DraggableText.Item2)
                        {
                            if (DraggableText.Item3)
                            {
                                DraggableText.Item1.Pos = Vector2.Zero;
                                if (Words[i].textObject != null)
                                    Words[i].textObject.Pos = Vector2.Zero;
                                Words[DraggableText.Item2].textObject = Words[i].textObject;
                                Words[i].textObject = DraggableText.Item1; 
                            }
                            else
                            {
                                DraggableText.Item1.Pos = Vector2.Zero;
                                if (Words[i].textObject != null)
                                {
                                    Items.Add(Words[i].textObject);
                                    Words[i].textObject.Pos = Vector2.Zero;
                                }
                                Words[i].textObject = DraggableText.Item1;
                            }
                            DraggableText = null;
                            return;
                        }
                    }
                    Rectangle savedObjs = savedItems;
                    savedObjs.Offset(Pos);
                    if (savedObjs.Intersects(obj))
                    DraggableText.Item1.Pos = Vector2.Zero;
                    {
                        DraggableText.Item1.Pos = Vector2.Zero;
                        Items.Add(DraggableText.Item1);
                        DraggableText = null;
                        return;
                    }
                    if (DraggableText.Item3)
                    {
                        Words[DraggableText.Item2].textObject = DraggableText.Item1;
                    }
                    else
                    {
                        Items.Add(DraggableText.Item1);
                    }
                    DraggableText = null;
                }
            }
        }

        public void Draw(Camera cam)
        {
            Rectangle ORR = spriteBatch.GraphicsDevice.ScissorRectangle;
            RasterizerState RS = new RasterizerState();
            RS.ScissorTestEnable = true;
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, RS);
            spriteBatch.Draw(BG, new Rectangle(Pos.ToPoint(), Size.ToPoint()), Color.White);
            Rectangle window = new Rectangle(Pos.ToPoint(), Size.ToPoint());
            spriteBatch.GraphicsDevice.ScissorRectangle = window;
            if (Words != null)
                for (int i = 0; i < Words.Count; i++)
                    Words[i].Draw(spriteBatch);
            spriteBatch.End();
            Rectangle rect = savedItems;
            rect.Offset(Pos);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, RS);
            spriteBatch.GraphicsDevice.ScissorRectangle = rect;
            if (Items != null)
                for (int i = 0; i < Items.Count; i++)
                    Items[i].DrawOnCenter(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, RS);
            spriteBatch.GraphicsDevice.ScissorRectangle = window;
            if (DraggableText != null)
                DraggableText.Item1.DrawOnCenter(spriteBatch);
            spriteBatch.End();
        }
    }
}