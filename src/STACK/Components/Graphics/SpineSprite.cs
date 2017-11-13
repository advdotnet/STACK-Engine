using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spine;
using STACK.Graphics;
using System;

namespace STACK.Components
{
    /// <summary>
    /// Renders a spine animation.
    /// </summary>
    [Serializable]
    public class SpineSprite : Component, IPlayAnimation
    {
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

        string AnimationName = "";
        public float AnimationTime = 0;
        public bool AnimationLooped = false;
        public bool Playing { get; private set; }
        public string Image { get; private set; }
        public RenderStage RenderStage { get; private set; }
        public Action<AnimationStateData> AnimationMixFn;
        public Action<AnimationState> OnSpineAnimationEnd;
        public Action<Event> OnSpineEvent;

        public SpineSprite()
        {
            RenderStage = RenderStage.Bloom;
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

        public override void OnLoadContent(ContentLoader content)
        {
            SkeletonBounds = new SkeletonBounds();

            Skeleton = content.Load<Skeleton>(Image);

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

        public override void OnDraw(Renderer renderer)
        {
            if (!RenderStage.HasFlag(renderer.Stage))
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

            Skeleton.FlipX = Data.Effects.HasFlag(SpriteEffects.FlipHorizontally);
            Skeleton.FlipY = Data.Effects.HasFlag(SpriteEffects.FlipVertically);

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

            renderer.SkeletonRenderer.Begin(renderer.Projection);
            Skeleton.R = Data.Color.R / 255f;
            Skeleton.G = Data.Color.G / 255f;
            Skeleton.B = Data.Color.B / 255f;
            Skeleton.A = Data.Color.A / 255f;
            renderer.SkeletonRenderer.Draw(Skeleton);

            if (NormalMap != null && Lightning != null)
            {
                renderer.ApplyNormalmapEffectParameter(Lightning, NormalMap);
                renderer.SkeletonRenderer.End(renderer.NormalmapEffect);
            }
            else
            {
                renderer.SkeletonRenderer.End(null);
            }

            renderer.Begin(renderer.Projection);
        }

        public override void OnUpdate()
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

        public string Animation { get { return AnimationName; } }

        public float GetHeight()
        {
            return SkeletonBounds.Height;
        }

        public override void OnNotify<T>(string message, T data)
        {
            if (message == Messages.AnimationStateChanged)
            {
                var NewState = (State)(object)data;

                PlayAnimation(NewState.ToAnimationName(), true);
            }
        }

        public void LoadSprite(string image)
        {
            Image = image;
            OnLoadContent(Entity.UpdateScene.Content);
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
