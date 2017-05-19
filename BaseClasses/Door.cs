using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace LightRise.BaseClasses
{
    public class Door
    {
        Comp Controller;
        Texture2D Texture;
        Point Pos;
        public Door(Texture2D texture, Point pos, Comp controller, Map map)
        {
            Controller = controller;
            map[pos] = Map.WALL;
            map[new Point(pos.X, pos.Y + 1)] = Map.WALL;
            map[new Point(pos.X, pos.Y + 2)] = Map.WALL;
            controller.AccessAllowed += delegate ()
            {
                map[pos] = Map.EMPTY;
                map[new Point(pos.X, pos.Y + 1)] = Map.EMPTY;
                map[new Point(pos.X, pos.Y + 2)] = Map.EMPTY;
            };
            controller.AccessDenied += delegate ()
            {
                map[pos] = Map.WALL;
                map[new Point(pos.X, pos.Y + 1)] = Map.WALL;
                map[new Point(pos.X, pos.Y + 2)] = Map.WALL;
            };
            Texture = texture;
            Pos = pos;
        }

        public void Draw(SpriteBatch surface, Camera camera)
        {
            if (!Controller.Allowed)
                surface.Draw(Texture, new Rectangle(camera.WorldToWindow(Pos.ToVector2()), new Point((int)camera.Scale.X, (int)camera.Scale.Y * 3)), Color.White);
        }
    }
}
