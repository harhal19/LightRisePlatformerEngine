using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LightRise.BaseClasses;

namespace LightRise.WinUtilsLib
{
    public class SpineObject
    {
        SkeletonMeshRenderer skeletonRenderer;
        Skeleton skeleton;
        public Skeleton Skeleton { get { return skeleton; } }
        public Slot headSlot;
        AnimationState state;
        public AnimationState State { get { return state; } }
        SkeletonBounds bounds = new SkeletonBounds();

        public Vector2 offset = Vector2.Zero;
        public Vector2 pos;
        public float ScaleX { set { Skeleton.RootBone.ScaleX = value; } get { return Skeleton.RootBone.ScaleX; } }
        public float ScaleY { set { Skeleton.RootBone.ScaleY = value; } get { return Skeleton.RootBone.ScaleY; } }
        public float Scale
        {
            set
            {
                Skeleton.RootBone.ScaleX = value;
                Skeleton.RootBone.ScaleY = value;
            }
            get
            {
                return Skeleton.RootBone.ScaleX;
            }
        }

        public SpineObject(GraphicsDevice Graphics, string filename, float scale, Vector2 pos, string headSlot = "head", bool AbsolutePath = false)
        {
            if (!AbsolutePath)
                filename = "Content/" + filename;
            skeletonRenderer = new SkeletonMeshRenderer(Graphics);
            skeletonRenderer.PremultipliedAlpha = true;
            Atlas atlas = new Atlas(filename + ".atlas", new XnaTextureLoader(Graphics));
            SkeletonData skeletonData;
            SkeletonJson json = new SkeletonJson(atlas);
            json.Scale = scale;
            skeletonData = json.ReadSkeletonData(filename + ".json");
            skeleton = new Skeleton(skeletonData);
            AnimationStateData stateData = new AnimationStateData(skeleton.Data);
            state = new AnimationState(stateData);
            this.pos = pos;
            this.headSlot = skeleton.FindSlot(headSlot);
        }

        public void Update(GameTime gameTime)
        {
            state.Update(gameTime.ElapsedGameTime.Milliseconds / 1000f);
            float Scale = this.Scale;
            state.Apply(skeleton);
            this.Scale = Scale;
            skeleton.UpdateWorldTransform();
            bounds.Update(skeleton, true);
        }

        public void Draw(Camera Cam)
        {
            skeleton.Pos = Cam.WorldToWindow(pos).ToVector2() + offset;
            Scale = Cam.Scale.X;
            //skeleton.scale = Cam.Scale.X / 551f;
            skeletonRenderer.Begin();
            skeletonRenderer.Draw(skeleton);
            skeletonRenderer.End();
        }
    }
}
