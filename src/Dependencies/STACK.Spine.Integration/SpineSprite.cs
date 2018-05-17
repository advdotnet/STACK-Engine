using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spine;
using STACK.Components;
using STACK.Graphics;
using STACK.Logging;
using System;

namespace STACK.Spine
{
    /// <summary>
    /// Renders a spine animation.
    /// </summary>
    [Serializable]
    public class SpineSprite : Component, IPlayAnimation, IContent, IDraw, IUpdate, INotify
    {
        public bool Enabled { get; set; }
        public float UpdateOrder { get; set; }
        string AnimationName = "";
        public string Animation { get { return AnimationName; } }
        public bool Visible { get; set; }
        public float DrawOrder { get; set; }
        public float AnimationTime = 0;
        public bool AnimationLooped = false;
        public bool Playing { get; private set; }
        public string Image { get; private set; }
        public RenderStage RenderStage { get; private set; }
        public Action<AnimationStateData> AnimationMixFn;
        public Action<AnimationState> OnSpineAnimationEnd;
        public Action<Event> OnSpineEvent;

        [NonSerialized]
        public Texture2D NormalMap;
        [NonSerialized]
        public Skeleton Skeleton;
        [NonSerialized]
        public SkeletonBounds SkeletonBounds;
        [NonSerialized]
        public AnimationState AnimationState;
        [NonSerialized]
        public AnimationStateData AnimationStateData;

        public SpineSprite()
        {
            RenderStage = RenderStage.Bloom;
            Enabled = true;
            Visible = true;
        }

        void UpdateSkeletonEffect()
        {
            if (Skeleton == null)
            {
                return;
            }

            Skeleton.FlipX = ((Data.Effects & SpriteEffects.FlipHorizontally) == SpriteEffects.FlipHorizontally);
            Skeleton.FlipY = ((Data.Effects & SpriteEffects.FlipVertically) == SpriteEffects.FlipVertically);
        }

        public SpriteData Data
        {
            get
            {
                return Get<SpriteData>();
            }
        }

        public void LoadContent(ContentLoader content)
        {
            SkeletonBounds = new SkeletonBounds();

            var Loader = Entity.World.Get<SpineTextureLoader>();
            Skeleton = Loader.Load(Image);

            var NormalsImage = Image + "_normals.png";

            if (System.IO.File.Exists(content.RootDirectory + "\\" + NormalsImage))
            {
                NormalMap = content.Load<Texture2D>(NormalsImage);
            }

            UpdateSkeletonEffect();
            SetSkin(Data.Skin);

            AnimationStateData = AnimationStateData ?? new AnimationStateData(Skeleton.Data);

            AnimationMixFn?.Invoke(AnimationStateData);

            if (AnimationState == null)
            {
                AnimationState = new AnimationState(AnimationStateData);
                AnimationState.Complete += AnimationComplete;
                AnimationState.Event += AnimationState_Event;
            }

            if (!string.IsNullOrEmpty(AnimationName))
            {
                AnimationState.SetAnimation(0, AnimationName, true);
                AnimationState.Update(AnimationTime);
                AnimationState.Apply(Skeleton);
            }
            AnimationState.End += AnimationState_End;
        }

        public void UnloadContent() { }

        void AnimationState_End(AnimationState state, int trackIndex)
        {
            OnSpineAnimationEnd?.Invoke(state);
        }

        void AnimationState_Event(AnimationState state, int trackIndex, Event e)
        {
            OnSpineEvent?.Invoke(e);
        }

        public void SetSkin(string skin)
        {
            if (!string.IsNullOrEmpty(skin))
            {
                Skeleton.SetSkin(skin);
            }
        }

        public bool IsRectangleHit(Vector2 point)
        {
            return SkeletonBounds.AabbContainsPoint(point.X, point.Y);
        }

        public bool IsPixelHit(Vector2 point)
        {
            return SkeletonBounds.ContainsPoint(point.X, point.Y) != null;
        }

        public void Draw(Renderer renderer)
        {
            if (RenderStage != renderer.Stage)
            {
                return;
            }

            var Lightning = Entity.Get<Lightning>();

            var Position = Vector2.Zero;

            var Transform = Entity.Get<Transform>();
            if (Transform != null)
            {
                Position = Transform.Position;
            }

            renderer.End();

            Skeleton.FlipX = Data.Effects.Has(SpriteEffects.FlipHorizontally);
            Skeleton.FlipY = Data.Effects.Has(SpriteEffects.FlipVertically);

            Skeleton.X = Position.X;
            Skeleton.Y = Position.Y;

            Skeleton.RootBone.ScaleX = Data.Scale.X;
            Skeleton.RootBone.ScaleY = Data.Scale.Y;

            if (Transform != null)
            {
                Skeleton.RootBone.ScaleX *= Transform.Scale;
                Skeleton.RootBone.ScaleY *= Transform.Scale;
            }

            Skeleton.UpdateWorldTransform();

            var SpineRenderer = Entity.World.Get<SpineRenderer>();

            SpineRenderer.SkeletonRenderer.Begin(renderer.Projection);
            Skeleton.R = Data.Color.R / 255f;
            Skeleton.G = Data.Color.G / 255f;
            Skeleton.B = Data.Color.B / 255f;
            Skeleton.A = Data.Color.A / 255f;
            SpineRenderer.SkeletonRenderer.Draw(Skeleton);

            if (NormalMap != null && Lightning != null)
            {
                renderer.ApplyNormalmapEffectParameter(Lightning, NormalMap);
                SpineRenderer.SkeletonRenderer.End(renderer.NormalmapEffect);
            }
            else
            {
                SpineRenderer.SkeletonRenderer.End(null);
            }

            renderer.Begin(renderer.Projection);
        }

        public void Update()
        {
            SkeletonBounds.Update(Skeleton, true);
            if (!Playing)
            {
                return;
            }
            AnimationState.Update(GameSpeed.TickDuration);

            AnimationState.Apply(Skeleton);
            AnimationTime += GameSpeed.TickDuration;
        }

        void AnimationComplete(AnimationState state, int trackIndex, int loopCount)
        {
            if (!AnimationLooped)
            {
                AnimationState.ClearTracks();
                AnimationName = "";
                AnimationTime = 0;
                Data.Animation = "";
                AnimationLooped = false;
                Playing = false;

                Log.WriteLine("Finished Animation" + state.ToString() + " " + loopCount);
            }
        }

        public void PlayAnimation(string animation, bool looped)
        {
            if (!string.IsNullOrEmpty(animation) && AnimationExists(animation))
            {
                if (Animation != animation || !Playing)
                {
                    Playing = true;
                    Skeleton.SetSlotsToSetupPose();
                    AnimationState.SetAnimation(0, animation, looped);
                    AnimationName = animation;
                    Data.Animation = animation;
                    AnimationLooped = looped;
                }
            }
            else
            {
                Reset();
            }
        }

        private bool AnimationExists(string name)
        {
            foreach (var Animation in AnimationStateData.SkeletonData.Animations)
            {
                if (Animation.Name == name)
                {
                    return true;
                }
            }
            return false;
        }

        public void Reset()
        {
            AnimationState.ClearTrack(0);
            Skeleton.SetToSetupPose();
            AnimationName = "";
            AnimationTime = 0;
            Data.Animation = "";
            AnimationLooped = false;
            Playing = false;
        }

        public float GetHeight()
        {
            return SkeletonBounds.Height;
        }

        public void Notify<T>(string message, T data)
        {
            if (message == Messages.AnimationStateChanged)
            {
                var NewState = (STACK.Components.State)(object)data;

                PlayAnimation(NewState.ToAnimationName(), true);
            }
        }

        public void LoadSprite(string image)
        {
            Image = image;
            LoadContent(Entity.UpdateScene.Content);
        }

        public static SpineSprite Create(Entity addTo)
        {
            return addTo.Add<SpineSprite>();
        }

        public SpineSprite SetImage(string value) { Image = value; return this; }
        public SpineSprite SetAnimationMixFn(Action<AnimationStateData> value) { AnimationMixFn = value; return this; }
        public SpineSprite SetOnSpineEvent(Action<Event> value) { OnSpineEvent = value; return this; }
        public SpineSprite SetOnSpineAnimationEnd(Action<AnimationState> value) { OnSpineAnimationEnd = value; return this; }
        public SpineSprite SetRenderStage(RenderStage value) { RenderStage = value; return this; }
    }
}
