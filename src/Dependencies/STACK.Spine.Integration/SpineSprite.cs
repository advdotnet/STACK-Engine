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

		private string _animationName = string.Empty;
		public string Animation => _animationName;
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

		private void UpdateSkeletonEffect()
		{
			if (Skeleton == null)
			{
				return;
			}

			Skeleton.FlipX = (Data.Effects & SpriteEffects.FlipHorizontally) == SpriteEffects.FlipHorizontally;
			Skeleton.FlipY = (Data.Effects & SpriteEffects.FlipVertically) == SpriteEffects.FlipVertically;
		}

		public SpriteData Data => Get<SpriteData>();

		public void LoadContent(ContentLoader content)
		{
			SkeletonBounds = new SkeletonBounds();

			var loader = Entity.World.Get<SpineTextureLoader>();
			Skeleton = loader.Load(Image);

			var normalsImage = $"{Image}_normals.png";

			if (System.IO.File.Exists($"{content.RootDirectory}\\{normalsImage}"))
			{
				NormalMap = content.Load<Texture2D>(normalsImage);
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

			if (!string.IsNullOrEmpty(_animationName))
			{
				AnimationState.SetAnimation(0, _animationName, true);
				AnimationState.Update(AnimationTime);
				AnimationState.Apply(Skeleton);
			}
			AnimationState.End += AnimationState_End;
		}

		public void UnloadContent() { }

		private void AnimationState_End(AnimationState state, int trackIndex)
		{
			OnSpineAnimationEnd?.Invoke(state);
		}

		private void AnimationState_Event(AnimationState state, int trackIndex, Event e)
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

			var lightning = Entity.Get<Lightning>();

			var position = Vector2.Zero;

			var transform = Entity.Get<Transform>();
			if (transform != null)
			{
				position = transform.Position;
			}

			renderer.End();

			Skeleton.FlipX = Data.Effects.Has(SpriteEffects.FlipHorizontally);
			Skeleton.FlipY = Data.Effects.Has(SpriteEffects.FlipVertically);

			Skeleton.X = position.X;
			Skeleton.Y = position.Y;

			Skeleton.RootBone.ScaleX = Data.Scale.X;
			Skeleton.RootBone.ScaleY = Data.Scale.Y;

			if (transform != null)
			{
				Skeleton.RootBone.ScaleX *= transform.Scale;
				Skeleton.RootBone.ScaleY *= transform.Scale;
			}

			Skeleton.UpdateWorldTransform();

			var spineRenderer = Entity.World.Get<SpineRenderer>();

			spineRenderer.SkeletonRenderer.Begin(renderer.Projection);
			Skeleton.R = Data.Color.R / 255f;
			Skeleton.G = Data.Color.G / 255f;
			Skeleton.B = Data.Color.B / 255f;
			Skeleton.A = Data.Color.A / 255f;
			spineRenderer.SkeletonRenderer.Draw(Skeleton);

			if (NormalMap != null && lightning != null)
			{
				renderer.ApplyNormalmapEffectParameter(lightning, NormalMap);
				spineRenderer.SkeletonRenderer.End(renderer.NormalmapEffect);
			}
			else
			{
				spineRenderer.SkeletonRenderer.End(null);
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

		private void AnimationComplete(AnimationState state, int trackIndex, int loopCount)
		{
			if (!AnimationLooped)
			{
				AnimationState.ClearTracks();
				_animationName = string.Empty;
				AnimationTime = 0;
				Data.Animation = string.Empty;
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
					_animationName = animation;
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
			foreach (var animation in AnimationStateData.SkeletonData.Animations)
			{
				if (animation.Name == name)
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
			_animationName = string.Empty;
			AnimationTime = 0;
			Data.Animation = string.Empty;
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
				var newState = (Components.State)(object)data;

				PlayAnimation(newState.ToAnimationName(), true);
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
