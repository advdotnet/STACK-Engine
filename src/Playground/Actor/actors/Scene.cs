﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using STACK;
using STACK.Components;
using System;

namespace Actor
{

	[Serializable]
	public class Scene : STACK.Scene
	{
		public Scene()
		{
			Enabled = true;
			Visible = true;

			InputDispatcher
				.Create(this)
				.SetOnMouseUpFn(OnMouseUp)
				.SetOnKeyDownFn(OnKeyDown);

			Push(new Ego(), new Mouse(), new ShadowEgo());
		}

		public void OnKeyDown(Keys key)
		{
			if (key == Keys.Escape)
			{
				StackGame.Engine.Exit();
			}
		}

		public void OnMouseUp(Vector2 position, MouseButton button)
		{
			var oum = World.Get<STACK.Components.Mouse>().ObjectUnderMouse;

			if (oum == null && button == MouseButton.Left)
			{
				ActorGame.Ego.Get<Scripts>().Remove(ActorScripts.GOTOSCRIPTID);
				ActorGame.Ego.GoTo(Vector2.Transform(new Vector2(position.X + 50, position.Y), ActorGame.Ego.DrawScene.Get<Camera>().TransformationInverse));

				ActorGame.ShadowEgo.Get<Scripts>().Remove(ActorScripts.GOTOSCRIPTID);
				ActorGame.ShadowEgo.GoTo(Vector2.Transform(new Vector2(position.X - 50, position.Y), ActorGame.ShadowEgo.DrawScene.Get<Camera>().TransformationInverse));
			}
		}
	}
}
