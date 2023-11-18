using Microsoft.Xna.Framework;
using STACK;
using STACK.Components;
using System;
using System.Collections;

namespace Actor
{

	[Serializable]
	public class ShadowEgo : Entity
	{
		public ShadowEgo()
		{

			Transform
				.Create(this)
				.SetPosition(250, 250)
				.SetSpeed(150f / 0.375f)
				.SetOrientation(-Vector2.UnitX)
				.SetUpdateZWithPosition(true)
				.SetCalculateEffectiveSpeedFn(CalculateSpeed)
				.SetScale(0.5f);

			Sprite
				.Create(this)
				.SetEnableNormalMap(true)
				.SetImage("characters/ego/sprite", 9, 4);

			SpriteTransformAnimation
				.Create(this)
				.SetSetFrameFn(SetFrame);

			SpriteData
				.Create(this)
				.SetOrientationFlip(false)
				.SetOffset(-40, -310);

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

		private int SetFrame(Transform transform, int step, int lastFrame)
		{
			var scaledStep = step / 7;
			var row = 0;

			switch (transform.Direction4)
			{
				case Directions4.Down: row = 1; break;
				case Directions4.Right: row = 3; break;
				case Directions4.Up: row = 4; break;
				case Directions4.Left: row = 2; break;
			}

			if (transform.State == State.Idle)
			{
				return 1 + (row - 1) * 9;
			}

			return (scaledStep % 8) + ((row - 1) * 8) + 2 + (row - 1);
		}

		public void Turn(Directions8 direction)
		{
			Get<Transform>().Turn(direction);
		}

		public Script StartScript(IEnumerator script, string name = "")
		{
			return Get<Scripts>().Start(script, name);
		}

		public Script GoTo(int x, int y, Directions8 direction = Directions8.None, Action cb = null)
		{
			return GoTo(new Vector2(x, y), direction, cb);
		}

		public Script GoTo(Vector2 position, Directions8 direction = Directions8.None, Action cb = null)
		{
			return Get<Scripts>().GoTo(position, direction, cb);
		}

		public Script GoTo(Interaction interaction, Action cb = null)
		{
			return Get<Scripts>().GoTo(interaction.Position, interaction.Direction, cb);
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
			Get<Lightning>().LightPosition = new Vector3(position, 5);

			base.OnUpdate();
		}
	}
}
