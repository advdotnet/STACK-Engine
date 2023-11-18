using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spine;
using STACK;
using STACK.Components;
using STACK.Spine;
using System;
using System.Collections;

namespace Actor
{

	[Serializable]
	public class Ego : Entity
	{
		public Ego()
		{
			CameraLocked
				.Create(this);

			Transform
				.Create(this)
				.SetPosition(350, 250)
				.SetSpeed(150f / 0.375f)
				.SetOrientation(-Vector2.UnitX)
				.SetUpdateZWithPosition(true)
				.SetCalculateEffectiveSpeedFn(CalculateSpeed)
				.SetScale(0.24f);

			SpineSprite
				.Create(this)
				.SetImage("characters/ego/ego")
				.SetAnimationMixFn(SetAnimationMix)
				.SetOnSpineEvent(SpineEvent)
				.SetOnSpineAnimationEnd(SpineAnimationEnd);

			SpriteData
				.Create(this)
				.SetOrientationFlip(true);

			Scripts
				.Create(this);

			Lightning
				.Create(this)
				.SetLightColor(new Vector3(1.5f, 0.25f, 0.25f));

			Visible = true;
			Enabled = true;
		}

		private float CalculateSpeed()
		{
			return 150;
		}

		public void SpineEvent(Event e)
		{
			if (e.Data.Name == "footstep" && DrawScene.Visible)
			{
				//PlayStepSound();
			}
		}

		public void SpineAnimationEnd(AnimationState e)
		{
			if (e.ToString() == "walk" && DrawScene.Visible)
			{
				//PlayStepSound();
			}
		}

		public void Turn(Directions8 direction)
		{
			Get<Transform>().Turn(direction);
		}

		public Script StartScript(IEnumerator script, string name = "")
		{
			return Get<Scripts>().Start(script, name);
		}

		public void SetAnimationMix(AnimationStateData animationStateData)
		{
			animationStateData.SetMix("walk", "idle", 0.2f);
			animationStateData.SetMix("walk", "talk", 0.2f);
			animationStateData.SetMix("talk", "walk", 0.2f);
			animationStateData.SetMix("idle", "walk", 0.2f);
			animationStateData.SetMix("talk", "idle", 0.2f);
			animationStateData.SetMix("idle", "talk", 0.2f);
			animationStateData.SetMix("jump", "idle", 0.2f);
			animationStateData.SetMix("idle", "jump", 0.2f);
		}

		public Script GoTo(Vector2 position, Directions8 direction = Directions8.None, Action cb = null)
		{
			return Get<Scripts>().GoTo(position, direction, cb);
		}

		public void Stop()
		{
			Get<Scripts>().Clear();
			Get<Text>().Clear();
			Get<Transform>().State = State.Idle;
		}

		public override void OnUpdate()
		{
			var position = World.Get<STACK.Components.Mouse>().Position;
			Get<Lightning>().LightPosition = new Vector3(position, 2);

			if (Get<SpineSprite>().Data.Effects.HasFlag(SpriteEffects.FlipHorizontally))
			{
				Get<Lightning>().LightPosition.X -= (Get<Lightning>().LightPosition.X - Get<Transform>().Position.X) * 2;
			}

			base.OnUpdate();
		}
	}
}
